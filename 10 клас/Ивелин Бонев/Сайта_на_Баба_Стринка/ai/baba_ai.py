# -*- coding: utf-8 -*-
"""Бабиният AI — мозъкът зад чата на „Електронната изба на баба Стринка".

Спайковият трансформър (spiking_transformer.py) разпознава НАМЕРЕНИЕТО на
въпроса, а този модул генерира отговора. Няма истински бекенд, затова
данните за доставки са симулирани: статусите, куриерите и датите се
теглят от random, но са ДЕТЕРМИНИСТИЧНИ за даден номер на поръчка —
попиташ ли два пъти за BS-48213, историята е една и съща.
"""

from __future__ import annotations

import math
import os
import random
import re
import sys
import time
from datetime import date, timedelta

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from spiking_transformer import build_model, tokenize, _trigrams  # noqa: E402

CACHE_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), ".model_cache.json")
CONFIDENCE_THRESHOLD = 0.30
DOMAIN_SIM_THRESHOLD = 0.30

EUR_RATE = 1.95583

PRODUCTS = [
    {"name": "Класическа джанка", "price": 12.50, "badge": "изборът на дядо"},
    {"name": "Бутикова череша с костилка", "price": 15.00, "badge": None},
    {"name": "Екзотична мушмула", "price": 18.99, "badge": "изборът на дядо"},
    {"name": "Прасковата на дядо ти Христо", "price": 22.00, "badge": None},
    {"name": "Носталгична дюля", "price": 14.00, "badge": None},
    {"name": "Специална ягода", "price": 16.50, "badge": None},
]

PRODUCT_KEYWORDS = {
    "джанк": 0, "череш": 1, "мушмул": 2, "прасков": 3, "дюл": 4, "ягод": 5,
}

MONTHS = ["януари", "февруари", "март", "април", "май", "юни",
          "юли", "август", "септември", "октомври", "ноември", "декември"]
WEEKDAYS = ["понеделник", "вторник", "сряда", "четвъртък", "петък", "събота", "неделя"]


# ---------------------------------------------------------------------------
# Обучаващи примери за спайковия трансформър (намерение → фрази)
# ---------------------------------------------------------------------------
TRAINING_DATA: dict[str, list[str]] = {
    "greeting": [
        "здравей", "здрасти", "добър ден", "добро утро", "добър вечер",
        "ало има ли някой", "ей здравей бабо", "здравей бабо стринке",
        "привет", "здравейте", "добър ден бабо", "hello", "hi",
    ],
    "capabilities": [
        "какво можеш", "какво умееш", "с какво можеш да ми помогнеш",
        "помощ", "какво знаеш", "как работиш", "какви въпроси мога да задам",
        "help", "what can you do", "инструкции", "за какво служиш",
        "какво правиш ти всъщност", "дай помощ",
    ],
    "order_status": [
        "къде е поръчката ми", "къде ми е поръчката", "статус на поръчка",
        "статус на поръчка bs 48213", "проследяване на пратка",
        "проследи поръчка", "искам да проверя поръчката си",
        "какво става с поръчката ми", "поръчах преди три дни къде е",
        "провери поръчка 12345", "къде е bs 55555", "пратката ми не идва",
        "track my order", "where is my order", "номер за проследяване",
        "къде ми е пратката", "искам да знам къде ми е пратката",
        "искам да проверя пратката си", "какво става с пратката",
    ],
    "delivery_time": [
        "колко време отнема доставката", "кога ще пристигне пратката",
        "за колко дни доставяте", "колко дни пътува пратката",
        "кога ще дойде компота", "срок на доставка",
        "след колко време ще получа поръчката", "бързо ли доставяте",
        "до колко дни е доставката", "how long does delivery take",
        "кога пристига доставката", "доставяте ли до утре",
    ],
    "delivery_cost": [
        "колко струва доставката", "каква е цената на доставката",
        "безплатна ли е доставката", "има ли безплатна доставка",
        "колко ще ми струва да ми го пратите", "цена за доставка до офис",
        "цена за доставка до адрес", "скъпа ли е доставката",
        "delivery cost", "shipping price", "колко е доставката",
        "такса доставка", "плаща ли се доставка",
    ],
    "courier": [
        "с кой куриер работите", "с какви куриери доставяте",
        "еконт или спиди", "може ли доставка до офис на еконт",
        "доставяте ли до адрес", "до офис ли доставяте",
        "кой носи пратките", "куриер", "каква фирма ползвате за доставка",
        "which courier do you use", "доставяте ли в чужбина",
        "до врата ли доставяте", "правите ли доставка до дома",
        "върви ли с еконт", "пращате ли по спиди", "по кой куриер пращате",
    ],
    "products": [
        "какви продукти предлагате", "какво продавате",
        "какви компоти имате", "какво има в каталога",
        "покажи ми продуктите", "какви буркани предлагате",
        "имате ли сладко", "какво мога да си купя", "асортимент",
        "what do you sell", "какви са ви стоките", "списък с продукти",
        "имате ли компот от ягоди", "що за буркани продавате",
        "що за компоти правите", "какви неща продавате", "с какво търгувате",
    ],
    "price": [
        "колко струва компота", "колко струва джанката",
        "каква е цената на черешата", "колко струва мушмулата",
        "цена на прасковата", "колко струва дюлята", "колко струва ягодата",
        "какви са цените", "ценоразпис", "колко струват бурканите",
        "how much does it cost", "цената на компот от джанки",
        "колко лева е компота",
    ],
    "recommend": [
        "какво ми препоръчваш", "кое е най-вкусното",
        "кой компот е най-хубав", "какво да си взема",
        "кое е най-купувано", "кой е хитът", "какво предпочиташ",
        "дай препоръка", "кое е специалитетът", "what do you recommend",
        "кое е любимото на дядо", "най-добрия компот", "кое си заслужава",
        "кое да пробвам", "какво да пробвам първо", "не знам какво да избера",
    ],
    "payment": [
        "как мога да платя", "какви методи на плащане приемате",
        "приемате ли карта", "мога ли да платя с биткойн",
        "приемате ли paypal", "наложен платеж имате ли",
        "плащане с google pay", "може ли с apple pay", "как се плаща",
        "payment methods", "приемате ли кеш", "с карта ли се плаща",
    ],
    "returns": [
        "мога ли да върна продукт", "как да върна поръчка",
        "счупи ми се буркана", "пратката дойде счупена", "рекламация",
        "не съм доволен от компота", "връщане на пари",
        "гаранция имате ли", "какъв е срокът за връщане",
        "can i return my order", "развален компот", "буркана е повреден",
    ],
    "about": [
        "кои сте вие", "коя е баба стринка", "разкажи ми за вас",
        "откъде сте", "каква е историята ви", "кой прави компотите",
        "вие истински ли сте", "за фирмата", "who are you",
        "откъде са плодовете", "как започнахте", "кой е дядо христо",
        "има ли консерванти в компотите",
    ],
    "contact": [
        "как да се свържа с вас", "имате ли телефон", "какъв е имейлът ви",
        "работно време", "кога работите", "къде се намирате",
        "адрес на магазина", "имате ли физически магазин", "contact",
        "телефонен номер", "до кога сте отворени", "къде е избата",
    ],
    "thanks_bye": [
        "благодаря", "мерси", "много благодаря", "супер благодаря ти",
        "чао", "довиждане", "лека вечер", "приятен ден", "до скоро",
        "thanks", "bye", "мерси бабо", "благодарско", "ти си върхът",
    ],
}


# Служебни думи, които сами по себе си не значат, че въпросът е "наш".
STOPWORDS = {
    "и", "в", "на", "за", "да", "се", "ли", "е", "с", "от", "до", "по",
    "ми", "ти", "си", "той", "тя", "то", "ние", "вие", "те", "аз", "а",
    "но", "или", "при", "след", "преди", "между", "това", "тези", "този",
    "тази", "кой", "коя", "кое", "кои", "какво", "какви", "каква", "какъв",
    "как", "къде", "кога", "колко", "що", "има", "имат", "не", "съм",
    "сте", "сме", "са", "ще", "би", "и", "the", "a", "is", "of", "to",
}


class DomainGate:
    """Пазач срещу въпроси извън домейна (случаен текст, обща култура…).

    Мери IDF-претеглено триграмно сходство до най-близката обучаваща фраза
    и проверява дали въпросът съдържа поне една позната смислова дума.
    Падне ли и двете — отговаряме честно „не те разбрах" вместо да си
    измисляме.
    """

    def __init__(self, training_data: dict[str, list[str]]) -> None:
        utterances = [u for us in training_data.values() for u in us]
        tri_sets = []
        vocab_words: set[str] = set()
        for u in utterances:
            words = tokenize(u)
            vocab_words.update(words)
            tri_sets.append({tri for w in words for tri in _trigrams(w)})

        df: dict[str, int] = {}
        for s in tri_sets:
            for tri in s:
                df[tri] = df.get(tri, 0) + 1
        n = len(tri_sets)
        self._idf = {tri: math.log(n / c) for tri, c in df.items()}
        self._max_idf = math.log(n)
        self._train = [(s, math.sqrt(sum(self._idf[t] ** 2 for t in s)) or 1.0)
                       for s in tri_sets]
        self._vocab_exact = {w for w in vocab_words if w not in STOPWORDS}
        self._vocab_prefixes = {w[:4] for w in vocab_words
                                if len(w) >= 4 and w not in STOPWORDS}

    def _w(self, tri: str) -> float:
        return self._idf.get(tri, self._max_idf)

    def similarity(self, text: str) -> float:
        q = {tri for w in tokenize(text) for tri in _trigrams(w)}
        if not q:
            return 0.0
        qn = math.sqrt(sum(self._w(t) ** 2 for t in q)) or 1.0
        best = 0.0
        for s, n in self._train:
            inter = sum(self._w(t) ** 2 for t in q & s)
            if inter:
                best = max(best, inter / (qn * n))
        return best

    def has_known_word(self, text: str) -> bool:
        for w in tokenize(text):
            if w in STOPWORDS:
                continue
            if w in self._vocab_exact:
                return True
            if len(w) >= 4 and w[:4] in self._vocab_prefixes:
                return True
        return False

    def in_domain(self, text: str) -> bool:
        return self.similarity(text) >= DOMAIN_SIM_THRESHOLD and self.has_known_word(text)


def fmt_price(bgn: float) -> str:
    return f"{bgn:.2f} лв. / {bgn / EUR_RATE:.2f} €"


def fmt_date(d: date) -> str:
    return f"{d.day} {MONTHS[d.month - 1]} ({WEEKDAYS[d.weekday()]})"


def extract_order_number(message: str) -> str | None:
    """Търси номер на поръчка: „BS-48213", „bs 48213" или самостоятелно 4+ цифри."""
    low = message.lower()
    m = re.search(r"bs[\s\-№#:]*(\d{2,10})", low)
    if m:
        return m.group(1)
    m = re.search(r"\b(\d{4,10})\b", low)
    return m.group(1) if m else None


def find_product(message: str) -> dict | None:
    low = message.lower()
    for key, idx in PRODUCT_KEYWORDS.items():
        if key in low:
            return PRODUCTS[idx]
    return None


# ---------------------------------------------------------------------------
# Генератори на отговори — симулирана логистика, без истински бекенд
# ---------------------------------------------------------------------------
ORDER_STAGES = [
    ("📥", "приета е и чака преглед от баба Стринка"),
    ("🫙", "баба Стринка лично опакова бурканите с вестник и любов"),
    ("🤝", "предадена е на куриера"),
    ("🚚", "пътува към теб"),
    ("🏤", "пристигна в офиса на куриера и те чака"),
]

COURIERS = ["Еконт", "Спиди"]

FLOURISHES = [
    "Баба каза да не бързаш — хубавият компот си заслужава чакането. 🫙",
    "Бурканите са увити като бебета, нищо няма да им стане.",
    "Съвет от баба: приготви място в мазето отсега.",
    "Дядо Христо лично провери капачките преди тръгване.",
]


def answer_greeting(message: str, order_no: str | None) -> str:
    return random.choice([
        "Здравей, чедо! 🫙 Аз съм бабиният дигитален помощник. Питай ме за "
        "доставки, поръчки, цени или компоти — за всичко съм насреща!",
        "Добре дошъл в избата, чедо! Баба Стринка е при бурканите, но аз ще "
        "ти помогна — питай за доставки, продукти или плащане.",
        "Здравей-здравей! ⚡ Спайковите ми неврони светнаха от радост. "
        "С какво да помогна — доставка, поръчка, компот?",
    ])


def answer_capabilities(message: str, order_no: str | None) -> str:
    return ("Ето какво умея, чедо:\n"
            "📦 проверявам статус на поръчка — кажи ми номера ѝ (напр. BS-48213)\n"
            "🚚 казвам срокове, цени и куриери за доставка\n"
            "🫙 разказвам за продуктите и цените им\n"
            "💳 обяснявам начините на плащане\n"
            "↩️ помагам с връщане и рекламации\n"
            "Питай смело!")


def answer_order_status(message: str, order_no: str | None) -> str:
    if not order_no:
        sample = random.randint(10000, 99999)
        return random.choice([
            "Дай ми номера на поръчката, чедо — пише го в имейла за "
            f"потвърждение (изглежда така: BS-{sample}). Ще я проследя веднага! 🔍",
            "Кажи ми номера на поръчката (например BS-"
            f"{sample}) и ще питам куриерските гълъби къде е. 🕊️",
        ])

    # Един и същ номер → една и съща история, все едно има истинска система.
    rng = random.Random("BS" + order_no)
    courier = rng.choice(COURIERS)
    waybill = rng.randint(10 ** 9, 10 ** 10 - 1)
    today = date.today()
    stage_idx = rng.choices(range(6), weights=[1, 2, 2, 3, 2, 2])[0]

    if stage_idx == 5:
        delivered = today - timedelta(days=rng.randint(0, 2))
        return (f"📦 Поръчка BS-{order_no}:\n"
                f"✅ Доставена на {fmt_date(delivered)} чрез {courier} "
                f"(товарителница {waybill}).\n"
                "Ако не е при теб — провери при съседите, те също обичат компот. 😄")

    icon, text = ORDER_STAGES[stage_idx]
    eta = today + timedelta(days=rng.randint(1, 4))
    lines = [f"📦 Поръчка BS-{order_no}:",
             f"{icon} Статус: {text}."]
    if stage_idx >= 2:
        lines.append(f"🚚 Куриер: {courier}, товарителница {waybill}.")
    lines.append(f"📅 Очаквана доставка: {fmt_date(eta)}.")
    lines.append(random.choice(FLOURISHES))
    return "\n".join(lines)


def answer_delivery_time(message: str, order_no: str | None) -> str:
    days = random.randint(2, 4)
    eta = date.today() + timedelta(days=days)
    return random.choice([
        f"🚚 Обикновено доставката отнема {days} работни дни. Поръчаш ли "
        f"днес, очаквай бурканите около {fmt_date(eta)}. Бабините компоти "
        "пътуват бавно, но пристигат тържествено!",
        f"За твоя край куриерът обещава {days} работни дни — тоест около "
        f"{fmt_date(eta)}. Дотогава баба щe е затворила още една тенджера. 🫙",
    ])


def answer_delivery_cost(message: str, order_no: str | None) -> str:
    return ("🚚 Цени за доставка:\n"
            f"• до офис на куриер — {fmt_price(4.90)}\n"
            f"• до адрес — {fmt_price(6.90)}\n"
            "🎁 Поръчки над 60 лв. пътуват безплатно — баба черпи!")


def answer_courier(message: str, order_no: str | None) -> str:
    return random.choice([
        "Работим с Еконт и Спиди — до офис или до адрес, ти избираш при "
        "поръчка. 🚚 А в нашето село доставя лично дядо Христо с колелото, "
        "но само до третата къща.",
        "Куриерите ни са Еконт и Спиди, с опция до врата. За чужбина баба "
        "още се пазари с DHL — казва, че ѝ карали бурканите „без душа“. 😄",
    ])


def answer_products(message: str, order_no: str | None) -> str:
    lines = ["🫙 Ето какво има в бабината изба в момента:"]
    for p in PRODUCTS:
        badge = f" — {p['badge']}! 🏅" if p["badge"] else ""
        lines.append(f"• {p['name']} — {fmt_price(p['price'])}{badge}")
    lines.append("Всичко е ръчно брано рано сутрин и без консерванти!")
    return "\n".join(lines)


def answer_price(message: str, order_no: str | None) -> str:
    product = find_product(message)
    if product:
        extra = " Плюс това е изборът на дядо! 🏅" if product["badge"] else ""
        return (f"🫙 {product['name']} струва {fmt_price(product['price'])}.{extra} "
                "Ще го намериш в продуктовия каталог.")
    return answer_products(message, order_no) + "\nКой буркан те интересува?"


def answer_recommend(message: str, order_no: str | None) -> str:
    product = find_product(message) or random.choice(PRODUCTS)
    reason = random.choice([
        "тазгодишната реколта е невероятна",
        "клиентите се редят на опашка за него",
        "баба Стринка си го крие за специални гости",
        "дядо Христо се кълне в него",
        "мирише на детство и лятна ваканция",
    ])
    return (f"Пробвай „{product['name']}“ ({fmt_price(product['price'])}) — "
            f"{reason}! 🫙 А ако не ти хареса… няма такъв случай досега.")


def answer_payment(message: str, order_no: str | None) -> str:
    return ("💳 Приемаме: дебитна/кредитна карта, Google Pay, Apple Pay, "
            "PayPal и дори Bitcoin ₿ — баба е модерна жена.\n"
            "Наложен платеж? Само след личен пазарлък с баба Стринка. 😄")


def answer_returns(message: str, order_no: str | None) -> str:
    return ("↩️ Можеш да върнеш неотворен буркан до 14 дни — без въпроси.\n"
            "Ако пратката е пристигнала счупена: снимай я и пиши на "
            "baba@strinka.bg — пращаме нов буркан веднага и безплатно.\n"
            "Бабината дума е по-яка от гаранция!")


def answer_about(message: str, order_no: str | None) -> str:
    return ("Баба Стринка е неуморната шефка на нашата малка семейна изба. "
            "Мисията ѝ: да съсипе конкуренцията на местния супермаркет. 😄\n"
            "Плодовете се берат на ръка рано сутрин от селото — включително "
            "от спонсорираната градина на дядо ти Христо. В бурканите "
            "затваряме спомени, детство и щипка селска идилия — без "
            "консерванти и подсладители. 🫙")


def answer_contact(message: str, order_no: str | None) -> str:
    return ("📞 Телефон: 0888 БУРКАНИ (0888 287 526)\n"
            "📧 Имейл: baba@strinka.bg\n"
            "🕗 Работно време: от първи петли до вечерната доилка "
            "(08:00–18:00, пон–съб)\n"
            "📍 Село Компотово, втората къща след голямата круша.")


def answer_thanks_bye(message: str, order_no: str | None) -> str:
    return random.choice([
        "Пак заповядай, чедо! Баба праща поздрави и буркан с усмивка. 🫙",
        "Със здраве! И помни — компотът се пие студен, а баба се слуша "
        "винаги. 😄",
        "Довиждане, чедо! Избата е отворена по всяко време — е, дигиталната. ⚡",
    ])


def answer_fallback(message: str, order_no: str | None) -> str:
    return ("Хм, не съм сигурна, че те разбрах, чедо. 🤔 Спайковете ми не "
            "светнаха достатъчно. Пробвай да ме питаш за:\n"
            "📦 статус на поръчка · 🚚 доставка и цени · 🫙 продукти · "
            "💳 плащане · ↩️ връщане")


ANSWER_BUILDERS = {
    "greeting": answer_greeting,
    "capabilities": answer_capabilities,
    "order_status": answer_order_status,
    "delivery_time": answer_delivery_time,
    "delivery_cost": answer_delivery_cost,
    "courier": answer_courier,
    "products": answer_products,
    "price": answer_price,
    "recommend": answer_recommend,
    "payment": answer_payment,
    "returns": answer_returns,
    "about": answer_about,
    "contact": answer_contact,
    "thanks_bye": answer_thanks_bye,
    "fallback": answer_fallback,
}


class BabaAI:
    """Публичният интерфейс: BabaAI().reply("къде ми е поръчката?")"""

    def __init__(self, verbose: bool = True) -> None:
        self.model = build_model(TRAINING_DATA, cache_path=CACHE_PATH, verbose=verbose)
        self.gate = DomainGate(TRAINING_DATA)

    def reply(self, message: str) -> dict:
        t0 = time.time()
        message = message.strip()[:300]
        result = self.model.classify(message)
        intent = result["intent"]
        confidence = result["confidence"]

        if confidence < CONFIDENCE_THRESHOLD or not self.gate.in_domain(message):
            intent = "fallback"

        order_no = extract_order_number(message)
        # Ако има номер на поръчка, въпросът почти сигурно е „къде ми е пратката".
        if order_no and intent in ("order_status", "delivery_time", "courier", "fallback"):
            intent = "order_status"

        text = ANSWER_BUILDERS[intent](message, order_no)
        return {
            "reply": text,
            "intent": intent,
            "confidence": round(confidence, 3),
            "spikes": result["spikes"],
            "ms": round((time.time() - t0) * 1000, 1),
        }


# ---------------------------------------------------------------------------
# CLI: python3 ai/baba_ai.py [--test] ["въпрос"]
# ---------------------------------------------------------------------------
TEST_CASES = [
    ("здрасти бабо", "greeting"),
    ("искам да знам къде ми е пратката", "order_status"),
    ("поръчка BS-77412 кога идва", "order_status"),
    ("за колко време пристига поръчка до софия", "delivery_time"),
    ("плаща ли се доставката нещо", "delivery_cost"),
    ("върви ли с еконт пратката", "courier"),
    ("що за буркани продавате", "products"),
    ("каква е цената на компота от праскови", "price"),
    ("кое да пробвам първо", "recommend"),
    ("може ли наложен платеж", "payment"),
    ("искам да върна един буркан", "returns"),
    ("разкажи за баба стринка", "about"),
    ("кога сте отворени", "contact"),
    ("мерси много бабо", "thanks_bye"),
]


def _run_tests(ai: "BabaAI") -> None:
    print("\n— Тест с невиждани въпроси —")
    ok = 0
    for text, expected in TEST_CASES:
        r = ai.reply(text)
        got = r["intent"]
        mark = "✅" if got == expected else "❌"
        ok += got == expected
        print(f"{mark} „{text}“ → {got} ({r['confidence']:.0%}, ⚡{r['spikes']}, {r['ms']} ms)"
              + ("" if got == expected else f"   [очаквано: {expected}]"))
    print(f"Резултат: {ok}/{len(TEST_CASES)}")


if __name__ == "__main__":
    args = [a for a in sys.argv[1:]]
    ai = BabaAI(verbose=True)
    if "--test" in args:
        _run_tests(ai)
    elif args:
        r = ai.reply(" ".join(args))
        print(f"\n[{r['intent']} · {r['confidence']:.0%} · ⚡{r['spikes']} спайка · {r['ms']} ms]")
        print(r["reply"])
    else:
        print("Интерактивен режим — пиши въпрос (празен ред за изход).")
        while True:
            try:
                q = input("ти> ").strip()
            except (EOFError, KeyboardInterrupt):
                break
            if not q:
                break
            r = ai.reply(q)
            print(f"баба> {r['reply']}")
            print(f"      [{r['intent']} · {r['confidence']:.0%} · ⚡{r['spikes']} · {r['ms']} ms]")
