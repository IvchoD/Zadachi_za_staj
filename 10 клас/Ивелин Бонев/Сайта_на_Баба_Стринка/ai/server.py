# -*- coding: utf-8 -*-
"""HTTP сървър за „Електронната изба на баба Стринка" + AI чат ендпойнт.

Пускане:  python3 ai/server.py
После отвори  http://localhost:8000

Сервира статичните файлове на сайта и POST /api/chat, който подава въпроса
на спайковия трансформър (baba_ai.BabaAI). Само стандартна библиотека.
"""

from __future__ import annotations

import json
import os
import sys
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from baba_ai import BabaAI  # noqa: E402

SITE_ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
BASE_PORT = 8000


class ChatHandler(SimpleHTTPRequestHandler):
    ai: BabaAI | None = None

    def do_POST(self) -> None:
        if self.path != "/api/chat":
            self.send_error(404)
            return
        try:
            length = int(self.headers.get("Content-Length", 0))
            data = json.loads(self.rfile.read(length) or b"{}")
            message = str(data.get("message", "")).strip()[:300]
        except (ValueError, UnicodeDecodeError):
            self._send_json(400, {"error": "невалидна заявка"})
            return
        if not message:
            self._send_json(400, {"error": "празно съобщение"})
            return

        result = ChatHandler.ai.reply(message)
        print(f'💬 „{message}“ → {result["intent"]} '
              f'({result["confidence"]:.0%}, ⚡{result["spikes"]} спайка, {result["ms"]} ms)',
              flush=True)
        self._send_json(200, result)

    def _send_json(self, code: int, obj: dict) -> None:
        body = json.dumps(obj, ensure_ascii=False).encode("utf-8")
        self.send_response(code)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(body)))
        self.send_header("Cache-Control", "no-store")
        self.end_headers()
        self.wfile.write(body)

    def log_message(self, fmt: str, *args) -> None:
        pass  # тихо за статичните файлове; чатът се логва отделно


def main() -> None:
    print("🫙 Електронната изба на баба Стринка — AI сървър")
    ChatHandler.ai = BabaAI(verbose=True)

    handler = partial(ChatHandler, directory=SITE_ROOT)
    server = None
    port = BASE_PORT
    for port in range(BASE_PORT, BASE_PORT + 10):
        try:
            server = ThreadingHTTPServer(("127.0.0.1", port), handler)
            break
        except OSError:
            continue
    if server is None:
        print(f"❌ Няма свободен порт в диапазона {BASE_PORT}–{BASE_PORT + 9}.")
        sys.exit(1)

    print(f"🌐 Сайтът върви на  http://localhost:{port}")
    print("   Спри ме с Ctrl+C. Баба чака въпроси…\n")
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        print("\n🫙 Баба затваря избата. Довиждане!")
    finally:
        server.server_close()


if __name__ == "__main__":
    main()
