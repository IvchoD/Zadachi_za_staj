# -*- coding: utf-8 -*-
"""Мини спайков трансформър (Spikformer-стил) на чист Python — нула зависимости.

Защо "спайков"? Невроните комуникират с бинарни спайкове (0/1) във времето,
а не с плаващи числа. Атенцията работи върху спайкови Q/K/V без softmax,
както в Spikformer (Zhou et al., 2022). Понеже спайковете са редки,
изчисленията са event-driven — сумираме само редовете на активните входове,
което държи всичко леко дори на чист Python.

Обучение: блокът на трансформъра е фиксирана произволна проекция (reservoir
подход — класика при спайковите мрежи), а се обучава само линеен readout
слой с averaged perceptron. Като skip връзка към readout-а подаваме и
bag-of-trigrams на цялото изречение.
"""

from __future__ import annotations

import hashlib
import json
import math
import os
import random
import re
import time

# ----- хиперпараметри -----
SEED = 20260708
L_TOKENS = 8          # макс. брой токени (позиции) в трансформъра
EMB = 16              # размер на токен-ембединга
HEADS = 2
D_HEAD = EMB // HEADS
T_STEPS = 6           # времеви стъпки на спайковата симулация
FFN_DIM = 32
BAG_DIM = 128         # bag-of-trigrams skip връзка към readout-а
LEAK = 0.75           # изтичане на мембранния потенциал
THRESHOLD = 0.6       # праг на LIF невроните
ATTN_SCALE = 0.30
RESIDUAL_GAIN = 0.5
RATE_GAIN = 1.1
BAG_WEIGHT = 1.25
MAX_EPOCHS = 40
SOFTMAX_SHARPNESS = 1.6
VERSION = "spikformer-mini-1"

N_SPIKE_FEATURES = EMB + FFN_DIM + EMB
N_FEATURES = N_SPIKE_FEATURES + BAG_DIM

_WORD_RE = re.compile(r"[а-яa-z0-9]+")


def tokenize(text: str) -> list[str]:
    return _WORD_RE.findall(text.lower())


def _h(s: str) -> int:
    return int.from_bytes(hashlib.md5(s.encode("utf-8")).digest()[:8], "big")


def _trigrams(word: str) -> list[str]:
    w = "^" + word + "$"
    if len(w) <= 3:
        return [w]
    return [w[i:i + 3] for i in range(len(w) - 2)]


def _l2(vec: list[float]) -> list[float]:
    n = math.sqrt(sum(x * x for x in vec))
    if n == 0.0:
        return vec
    return [x / n for x in vec]


def embed_token(word: str) -> list[float]:
    """Токен → плътен вектор чрез хеширани символни триграми."""
    vec = [0.0] * EMB
    for tri in _trigrams(word):
        vec[_h("emb:" + tri) % EMB] += 1.0
    return _l2(vec)


def bag_features(tokens: list[str]) -> list[float]:
    """Bag-of-trigrams за цялото изречение (skip връзка към readout-а)."""
    vec = [0.0] * BAG_DIM
    for word in tokens:
        for tri in _trigrams(word):
            vec[_h("bag:" + tri) % BAG_DIM] += 1.0
    return _l2(vec)


def _rand_matrix(rng: random.Random, n_in: int, n_out: int) -> list[list[float]]:
    scale = 1.0 / math.sqrt(n_in)
    return [[rng.gauss(0.0, scale) for _ in range(n_out)] for _ in range(n_in)]


def _accumulate(rows: list[list[float]], active: list[int], out: list[float]) -> None:
    """Event-driven матрично умножение: спайковете са бинарни и редки,
    затова просто сумираме редовете на тежестите за активните входове."""
    for j in active:
        row = rows[j]
        for k, w in enumerate(row):
            out[k] += w


def _lif_step(v: list[float], current: list[float]) -> list[int]:
    """Една стъпка на leaky integrate-and-fire неврони. Връща кой гръмна."""
    fired = []
    for k in range(len(v)):
        m = v[k] * LEAK + current[k]
        if m >= THRESHOLD:
            fired.append(k)
            m -= THRESHOLD
        v[k] = m
    return fired


class SpikingTransformer:
    """Един Spikformer-блок: спайкова self-attention + спайков FFN."""

    def __init__(self) -> None:
        rng = random.Random(SEED)
        self.Wq = _rand_matrix(rng, EMB, EMB)
        self.Wk = _rand_matrix(rng, EMB, EMB)
        self.Wv = _rand_matrix(rng, EMB, EMB)
        self.Wo = _rand_matrix(rng, EMB, EMB)
        self.W1 = _rand_matrix(rng, EMB, FFN_DIM)
        self.W2 = _rand_matrix(rng, FFN_DIM, EMB)

    def forward(self, text: str) -> tuple[list[float], int]:
        """Текст → (вектор с характеристики за readout-а, общ брой спайкове)."""
        tokens = tokenize(text)
        positions = [embed_token(w) for w in tokens[:L_TOKENS]] or [[0.0] * EMB]
        n_pos = len(positions)

        # мембранни потенциали по позиция и слой
        v_q = [[0.0] * EMB for _ in range(n_pos)]
        v_k = [[0.0] * EMB for _ in range(n_pos)]
        v_v = [[0.0] * EMB for _ in range(n_pos)]
        v_a = [[0.0] * EMB for _ in range(n_pos)]
        v_o = [[0.0] * EMB for _ in range(n_pos)]
        v_h = [[0.0] * FFN_DIM for _ in range(n_pos)]
        v_f = [[0.0] * EMB for _ in range(n_pos)]
        phase = [[0.0] * EMB for _ in range(n_pos)]

        cnt_o = [0] * EMB
        cnt_h = [0] * FFN_DIM
        cnt_f = [0] * EMB
        total_spikes = 0

        for _t in range(T_STEPS):
            # 1) вход → спайкове (фазов акумулатор = детерминистично rate coding)
            in_spk: list[list[int]] = []
            for p in range(n_pos):
                fired = []
                acc = phase[p]
                rate = positions[p]
                for j in range(EMB):
                    a = acc[j] + min(0.95, rate[j] * RATE_GAIN)
                    if a >= 1.0:
                        fired.append(j)
                        a -= 1.0
                    acc[j] = a
                in_spk.append(fired)
                total_spikes += len(fired)

            # 2) спайкови Q, K, V
            q_spk: list[list[int]] = []
            k_spk: list[list[int]] = []
            v_spk: list[list[int]] = []
            for p in range(n_pos):
                for W, v_mem, dest in ((self.Wq, v_q, q_spk),
                                       (self.Wk, v_k, k_spk),
                                       (self.Wv, v_v, v_spk)):
                    cur = [0.0] * EMB
                    _accumulate(W, in_spk[p], cur)
                    fired = _lif_step(v_mem[p], cur)
                    dest.append(fired)
                    total_spikes += len(fired)

            # 3) спайкова атенция: score(i,p) = брой съвпадащи спайкове в главата,
            #    после score * V като входен ток (QKᵀV без softmax — само целичисления)
            attn_cur = [[0.0] * EMB for _ in range(n_pos)]
            for h in range(HEADS):
                lo, hi = h * D_HEAD, (h + 1) * D_HEAD
                k_sets = [{d for d in k_spk[p] if lo <= d < hi} for p in range(n_pos)]
                v_lists = [[d for d in v_spk[p] if lo <= d < hi] for p in range(n_pos)]
                for i in range(n_pos):
                    q_set = {d for d in q_spk[i] if lo <= d < hi}
                    if not q_set:
                        continue
                    cur_i = attn_cur[i]
                    for p in range(n_pos):
                        score = len(q_set & k_sets[p])
                        if score:
                            w = score * ATTN_SCALE
                            for d in v_lists[p]:
                                cur_i[d] += w

            a_spk: list[list[int]] = []
            for p in range(n_pos):
                fired = _lif_step(v_a[p], attn_cur[p])
                a_spk.append(fired)
                total_spikes += len(fired)

            # 4) изходна проекция на атенцията
            o_spk: list[list[int]] = []
            for p in range(n_pos):
                cur = [0.0] * EMB
                _accumulate(self.Wo, a_spk[p], cur)
                fired = _lif_step(v_o[p], cur)
                o_spk.append(fired)
                total_spikes += len(fired)
                for d in fired:
                    cnt_o[d] += 1

            # 5) спайков FFN с residual ток от входа и от атенцията
            for p in range(n_pos):
                cur = [0.0] * FFN_DIM
                _accumulate(self.W1, o_spk[p], cur)
                for j in in_spk[p]:
                    row = self.W1[j]
                    for k2 in range(FFN_DIM):
                        cur[k2] += row[k2] * RESIDUAL_GAIN
                h_fired = _lif_step(v_h[p], cur)
                total_spikes += len(h_fired)
                for d in h_fired:
                    cnt_h[d] += 1

                cur2 = [0.0] * EMB
                _accumulate(self.W2, h_fired, cur2)
                for d in o_spk[p]:
                    cur2[d] += RESIDUAL_GAIN
                f_fired = _lif_step(v_f[p], cur2)
                total_spikes += len(f_fired)
                for d in f_fired:
                    cnt_f[d] += 1

        norm = float(n_pos * T_STEPS)
        spike_feats = [c / norm for c in (cnt_o + cnt_h + cnt_f)]
        feats = _l2(spike_feats) + [BAG_WEIGHT * x for x in bag_features(tokens)]
        return feats, total_spikes


def _sparse(feats: list[float]) -> list[tuple[int, float]]:
    return [(i, v) for i, v in enumerate(feats) if v]


class Readout:
    """Линеен readout, обучен с averaged perceptron върху спайковите характеристики."""

    def __init__(self, classes: list[str], weights: list[list[float]] | None = None) -> None:
        self.classes = list(classes)
        self.W = weights if weights is not None else [[0.0] * N_FEATURES for _ in self.classes]

    def scores(self, pairs: list[tuple[int, float]]) -> list[float]:
        return [sum(row[k] * v for k, v in pairs) for row in self.W]

    def train(self, X: list[list[tuple[int, float]]], y: list[int], verbose: bool = False) -> float:
        rng = random.Random(SEED + 1)
        order = list(range(len(X)))
        avg = [[0.0] * N_FEATURES for _ in self.classes]
        epochs_run = 0
        for epoch in range(1, MAX_EPOCHS + 1):
            epochs_run = epoch
            rng.shuffle(order)
            mistakes = 0
            for i in order:
                s = self.scores(X[i])
                pred = max(range(len(s)), key=s.__getitem__)
                if pred != y[i]:
                    mistakes += 1
                    w_true, w_pred = self.W[y[i]], self.W[pred]
                    for k, v in X[i]:
                        w_true[k] += v
                        w_pred[k] -= v
            for c in range(len(self.classes)):
                row_a, row_w = avg[c], self.W[c]
                for k in range(N_FEATURES):
                    row_a[k] += row_w[k]
            if verbose:
                print(f"   епоха {epoch:2d}: {mistakes} грешки от {len(X)}")
            if mistakes == 0:
                break
        self.W = [[a / epochs_run for a in row] for row in avg]
        correct = sum(
            1 for i in range(len(X))
            if max(range(len(self.classes)), key=self.scores(X[i]).__getitem__) == y[i]
        )
        return correct / max(1, len(X))


def _softmax_std(scores: list[float]) -> list[float]:
    """Стандартизираме суровите резултати и вадим псевдо-вероятности."""
    m = sum(scores) / len(scores)
    sd = math.sqrt(sum((s - m) ** 2 for s in scores) / len(scores)) or 1.0
    z = [SOFTMAX_SHARPNESS * (s - m) / sd for s in scores]
    mx = max(z)
    exps = [math.exp(v - mx) for v in z]
    tot = sum(exps)
    return [e / tot for e in exps]


class IntentModel:
    """Спайков трансформър + обучен readout = класификатор на намерения."""

    def __init__(self, transformer: SpikingTransformer, readout: Readout) -> None:
        self.transformer = transformer
        self.readout = readout

    def classify(self, text: str) -> dict:
        feats, spikes = self.transformer.forward(text)
        scores = self.readout.scores(_sparse(feats))
        probs = _softmax_std(scores)
        ranked = sorted(zip(self.readout.classes, probs), key=lambda x: -x[1])
        return {
            "intent": ranked[0][0],
            "confidence": ranked[0][1],
            "spikes": spikes,
            "top": ranked[:3],
        }


def _cache_key(training_data: dict[str, list[str]]) -> str:
    payload = VERSION + json.dumps(training_data, sort_keys=True, ensure_ascii=False)
    payload += repr((L_TOKENS, EMB, HEADS, T_STEPS, FFN_DIM, BAG_DIM,
                     LEAK, THRESHOLD, ATTN_SCALE, RESIDUAL_GAIN, RATE_GAIN, BAG_WEIGHT))
    return hashlib.md5(payload.encode("utf-8")).hexdigest()


def build_model(training_data: dict[str, list[str]],
                cache_path: str | None = None,
                verbose: bool = True) -> IntentModel:
    """Строи (или зарежда от кеш) обучен модел върху дадените примери."""
    transformer = SpikingTransformer()
    classes = sorted(training_data.keys())
    key = _cache_key(training_data)

    if cache_path and os.path.exists(cache_path):
        try:
            with open(cache_path, "r", encoding="utf-8") as f:
                cached = json.load(f)
            if cached.get("key") == key:
                if verbose:
                    print("⚡ Зареден обучен спайков трансформър от кеша.")
                return IntentModel(transformer, Readout(cached["classes"], cached["W"]))
        except (OSError, ValueError, KeyError):
            pass  # развален кеш → обучаваме наново

    t0 = time.time()
    if verbose:
        n_total = sum(len(v) for v in training_data.values())
        print(f"⚡ Обучение на спайковия трансформър ({n_total} примера, "
              f"{len(classes)} намерения)…")

    X: list[list[tuple[int, float]]] = []
    y: list[int] = []
    for ci, cls in enumerate(classes):
        for utterance in training_data[cls]:
            feats, _ = transformer.forward(utterance)
            X.append(_sparse(feats))
            y.append(ci)

    readout = Readout(classes)
    acc = readout.train(X, y)
    if verbose:
        print(f"⚡ Готово за {time.time() - t0:.1f} s — точност върху "
              f"обучаващите примери: {acc:.0%}")

    if cache_path:
        try:
            with open(cache_path, "w", encoding="utf-8") as f:
                json.dump({"key": key, "classes": classes, "W": readout.W}, f)
        except OSError:
            pass  # кешът е само оптимизация

    return IntentModel(transformer, readout)
