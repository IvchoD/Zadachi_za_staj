// Чат джаджата на баба Стринка — говори с ai/server.py (POST /api/chat).
// Само vanilla JS: сама си инжектира стиловете и DOM-а, пази историята
// в sessionStorage, за да оцелява между страниците.
(function () {
    'use strict';

    const API_URL = '/api/chat';
    const HISTORY_KEY = 'bs-chat-history';
    const OPEN_KEY = 'bs-chat-open';

    const WELCOME =
        'Здравей, чедо! 🫙 Аз съм спайковият помощник на баба Стринка. ' +
        'Питай ме за поръчки, доставки, цени, плащане или компоти!';

    const SUGGESTIONS = [
        'Къде е поръчка BS-48213?',
        'Колко струва доставката?',
        'Какво ми препоръчваш?',
    ];

    const OFFLINE_TEXT =
        'Ех, чедо… не мога да стигна до бабиния AI сървър. 🔌\n' +
        'Пусни го от папката на сайта с:\n' +
        'python3 ai/server.py\n' +
        'и отвори сайта през http://localhost:8000';

    const CSS = `
#bs-chat-launcher {
    position: fixed; right: 20px; bottom: 20px; z-index: 9998;
    width: 62px; height: 62px; border-radius: 50%;
    border: 3px solid #00966e; cursor: pointer;
    background: linear-gradient(180deg, #c8102e 0%, #a50f26 100%);
    color: #fff; font-size: 28px; line-height: 1;
    box-shadow: 0 4px 14px rgba(0, 0, 0, 0.25);
    transition: transform 0.2s ease;
}
#bs-chat-launcher:hover { transform: scale(1.08); }
#bs-chat-panel {
    position: fixed; right: 16px; bottom: 94px; z-index: 9999;
    width: min(380px, calc(100vw - 32px));
    height: min(540px, calc(100vh - 120px));
    display: none; flex-direction: column; overflow: hidden;
    background: #f5f0e6; border: 2px dashed #c8102e;
    border-radius: 12px; box-shadow: 0 8px 28px rgba(0, 0, 0, 0.3);
    font-family: 'Georgia', serif; color: #3a2f28;
}
#bs-chat-panel.bs-open { display: flex; }
.bs-chat-header {
    background: linear-gradient(180deg, #c8102e 0%, #a50f26 100%);
    color: #fff; padding: 12px 16px;
    border-bottom: 3px solid #00966e;
    display: flex; align-items: center; justify-content: space-between;
}
.bs-chat-header h3 { margin: 0; font-size: 1.05rem; }
.bs-chat-header small { display: block; font-weight: normal; opacity: 0.85; font-size: 0.72rem; }
.bs-chat-close {
    background: none; border: none; color: #fff; font-size: 1.4rem;
    cursor: pointer; line-height: 1; padding: 0 4px; font-family: inherit;
}
.bs-chat-messages {
    flex: 1; overflow-y: auto; padding: 14px;
    display: flex; flex-direction: column; gap: 10px;
}
.bs-msg { max-width: 85%; padding: 10px 13px; border-radius: 12px;
    font-size: 0.92rem; line-height: 1.45; white-space: pre-line;
    overflow-wrap: break-word; }
.bs-msg.bs-bot {
    align-self: flex-start; background: #fff;
    border: 2px dashed #c8102e; border-left: 4px solid #00966e;
    border-bottom-left-radius: 4px;
}
.bs-msg.bs-user {
    align-self: flex-end; background: #00966e; color: #fff;
    border-bottom-right-radius: 4px;
}
.bs-msg-meta {
    align-self: flex-start; font-size: 0.7rem; color: #8a7f72;
    margin: -6px 0 0 6px;
}
.bs-typing span {
    display: inline-block; width: 7px; height: 7px; margin: 0 2px;
    background: #c8102e; border-radius: 50%; opacity: 0.3;
    animation: bs-blink 1.2s infinite;
}
.bs-typing span:nth-child(2) { animation-delay: 0.2s; }
.bs-typing span:nth-child(3) { animation-delay: 0.4s; }
@keyframes bs-blink { 0%, 80%, 100% { opacity: 0.3; } 40% { opacity: 1; } }
.bs-chat-suggestions {
    display: flex; flex-wrap: wrap; gap: 6px; padding: 0 14px 8px;
}
.bs-chip {
    background: #fff; border: 2px solid #00966e; color: #00966e;
    border-radius: 16px; padding: 5px 11px; font-size: 0.78rem;
    cursor: pointer; font-family: inherit; font-weight: bold;
}
.bs-chip:hover { background: #e8f5f0; }
.bs-chat-input-row {
    display: flex; gap: 8px; padding: 10px;
    background: #fff; border-top: 3px solid #00966e;
}
.bs-chat-input-row input {
    flex: 1; padding: 10px 12px; font-family: inherit; font-size: 0.92rem;
    border: 2px solid #d8cbb8; border-radius: 8px; color: #3a2f28;
    background: #f5f0e6; outline: none;
}
.bs-chat-input-row input:focus { border-color: #00966e; }
.bs-chat-input-row button {
    background: #c8102e; color: #fff; border: none; border-radius: 8px;
    padding: 0 16px; font-family: inherit; font-weight: bold;
    font-size: 0.92rem; cursor: pointer;
}
.bs-chat-input-row button:hover { background: #a50f26; }
.bs-chat-input-row button:disabled { background: #ccc; cursor: not-allowed; }
@media (max-width: 480px) {
    #bs-chat-panel { right: 8px; bottom: 84px; }
}
`;

    let history = [];
    try {
        history = JSON.parse(sessionStorage.getItem(HISTORY_KEY)) || [];
    } catch (e) { history = []; }

    function saveHistory() {
        try { sessionStorage.setItem(HISTORY_KEY, JSON.stringify(history)); } catch (e) { /* няма място — нищо */ }
    }

    // --- DOM ---
    const style = document.createElement('style');
    style.textContent = CSS;
    document.head.appendChild(style);

    const launcher = document.createElement('button');
    launcher.id = 'bs-chat-launcher';
    launcher.type = 'button';
    launcher.title = 'Попитай баба Стринка';
    launcher.textContent = '🫙';

    const panel = document.createElement('div');
    panel.id = 'bs-chat-panel';
    panel.innerHTML =
        '<div class="bs-chat-header">' +
        '  <h3>Баба Стринка AI<small>спайков трансформър · демо без истински бекенд</small></h3>' +
        '  <button type="button" class="bs-chat-close" title="Затвори">×</button>' +
        '</div>' +
        '<div class="bs-chat-messages"></div>' +
        '<div class="bs-chat-suggestions"></div>' +
        '<div class="bs-chat-input-row">' +
        '  <input type="text" maxlength="300" placeholder="Питай баба…" aria-label="Съобщение">' +
        '  <button type="button">➤</button>' +
        '</div>';

    document.body.appendChild(launcher);
    document.body.appendChild(panel);

    const messagesEl = panel.querySelector('.bs-chat-messages');
    const suggestionsEl = panel.querySelector('.bs-chat-suggestions');
    const inputEl = panel.querySelector('input');
    const sendBtn = panel.querySelector('.bs-chat-input-row button');
    const closeBtn = panel.querySelector('.bs-chat-close');

    function scrollDown() {
        messagesEl.scrollTop = messagesEl.scrollHeight;
    }

    function addMessage(who, text, meta, remember) {
        const msg = document.createElement('div');
        msg.className = 'bs-msg bs-' + who;
        msg.textContent = text; // textContent → никакъв HTML от потребителя
        messagesEl.appendChild(msg);
        if (meta) {
            const m = document.createElement('div');
            m.className = 'bs-msg-meta';
            m.textContent = meta;
            messagesEl.appendChild(m);
        }
        if (remember !== false) {
            history.push({ who: who, text: text, meta: meta || null });
            saveHistory();
        }
        scrollDown();
    }

    function showTyping() {
        const t = document.createElement('div');
        t.className = 'bs-msg bs-bot bs-typing';
        t.innerHTML = '<span></span><span></span><span></span>';
        messagesEl.appendChild(t);
        scrollDown();
        return t;
    }

    function renderSuggestions() {
        suggestionsEl.innerHTML = '';
        if (history.some(function (m) { return m.who === 'user'; })) return;
        SUGGESTIONS.forEach(function (text) {
            const chip = document.createElement('button');
            chip.type = 'button';
            chip.className = 'bs-chip';
            chip.textContent = text;
            chip.addEventListener('click', function () { send(text); });
            suggestionsEl.appendChild(chip);
        });
    }

    function send(text) {
        text = (text || '').trim();
        if (!text || sendBtn.disabled) return;
        addMessage('user', text);
        renderSuggestions();
        inputEl.value = '';
        sendBtn.disabled = true;

        const typing = showTyping();
        fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ message: text })
        })
            .then(function (res) {
                if (!res.ok) throw new Error('HTTP ' + res.status);
                return res.json();
            })
            .then(function (data) {
                typing.remove();
                const meta = '⚡ ' + data.spikes + ' спайка · ' +
                    Math.round(data.confidence * 100) + '% увереност · ' +
                    data.ms + ' ms';
                addMessage('bot', data.reply, meta);
            })
            .catch(function () {
                typing.remove();
                addMessage('bot', OFFLINE_TEXT);
            })
            .finally(function () {
                sendBtn.disabled = false;
                inputEl.focus();
            });
    }

    function setOpen(open) {
        panel.classList.toggle('bs-open', open);
        try { sessionStorage.setItem(OPEN_KEY, open ? '1' : '0'); } catch (e) { /* нищо */ }
        if (open) {
            scrollDown();
            inputEl.focus();
        }
    }

    launcher.addEventListener('click', function () {
        setOpen(!panel.classList.contains('bs-open'));
    });
    closeBtn.addEventListener('click', function () { setOpen(false); });
    sendBtn.addEventListener('click', function () { send(inputEl.value); });
    inputEl.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') send(inputEl.value);
    });

    // --- начално състояние ---
    if (history.length === 0) {
        history.push({ who: 'bot', text: WELCOME, meta: null });
        saveHistory();
    }
    history.forEach(function (m) { addMessage(m.who, m.text, m.meta, false); });
    renderSuggestions();
    if (sessionStorage.getItem(OPEN_KEY) === '1') setOpen(true);
})();
