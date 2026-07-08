using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ZhadniyatProgramist.Controls
{
    /// <summary>
    /// Кратка анимация при успешно плащане:
    ///  - златни монети излитат нагоре и падат с лека гравитация;
    ///  - в центъра се появява голям надпис „ПЛАТЕНО“;
    ///  - всичко избледнява плавно и контролът се скрива сам след ~2 сек.
    ///
    /// Използва се от MainForm: контролът стои скрит и при Play(rect)
    /// се позиционира върху панела с дълга, пуска анимацията и изчезва.
    /// </summary>
    public class CoinBurstControl : Control
    {
        private sealed class Coin
        {
            public float X, Y;    // позиция
            public float Vx, Vy;  // скорост
            public float Radius;
            public float Spin;
        }

        private enum BurstMode { Coins, OrderDrop }

        private const int DurationMs = 1900;
        private const int OrderDropDurationMs = 1250;

        private readonly Timer _timer;
        private readonly List<Coin> _coins = new List<Coin>();
        private readonly Random _random = new Random();
        private DateTime _startedAt;
        private BurstMode _mode = BurstMode.Coins;

        private int ActiveDuration
        {
            get { return _mode == BurstMode.Coins ? DurationMs : OrderDropDurationMs; }
        }

        public CoinBurstControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw, true);

            BackColor = Theme.Background;
            Visible = false;
            Enabled = false; // не пречи на кликове
            TabStop = false;

            _timer = new Timer { Interval = 30 };
            _timer.Tick += Timer_Tick;
        }

        /// <summary>Пуска анимацията с монети върху зададен правоъгълник (в координати на родителя).</summary>
        public void Play(Rectangle boundsInParent)
        {
            _mode = BurstMode.Coins;
            Bounds = boundsInParent;
            StartAnimation();
        }

        /// <summary>
        /// Малка анимация при добавена поръчка: бирена халба „пада“
        /// в тефтера и се появява „Записано!“. Показва се върху
        /// зададения правоъгълник (в координати на родителя).
        /// </summary>
        public void PlayOrderDrop(Rectangle boundsInParent)
        {
            _mode = BurstMode.OrderDrop;
            Bounds = boundsInParent;
            StartAnimation();
        }

        private void StartAnimation()
        {
            _coins.Clear();

            if (_mode == BurstMode.Coins)
            {
                int count = 14;
                float originX = Width / 2f;
                float originY = Height * 0.72f;

                for (int i = 0; i < count; i++)
                {
                    _coins.Add(new Coin
                    {
                        X = originX + (float)(_random.NextDouble() * 60 - 30),
                        Y = originY,
                        Vx = (float)(_random.NextDouble() * 7 - 3.5),
                        Vy = -(4.5f + (float)(_random.NextDouble() * 5.0)), // излитат нагоре
                        Radius = 6f + (float)(_random.NextDouble() * 5),
                        Spin = (float)(_random.NextDouble() * Math.PI)
                    });
                }
            }

            _startedAt = DateTime.Now;
            Visible = true;
            BringToFront();
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double elapsed = (DateTime.Now - _startedAt).TotalMilliseconds;

            foreach (Coin coin in _coins)
            {
                coin.X += coin.Vx;
                coin.Y += coin.Vy;
                coin.Vy += 0.28f; // лека гравитация
                coin.Spin += 0.2f;
            }

            if (elapsed >= ActiveDuration)
            {
                _timer.Stop();
                Visible = false;
            }
            else
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            double elapsed = (DateTime.Now - _startedAt).TotalMilliseconds;
            double t = Math.Min(1.0, elapsed / ActiveDuration);

            // Плавно избледняване в последната третина
            float fade = t < 0.66 ? 1f : (float)(1.0 - (t - 0.66) / 0.34);
            fade = Math.Max(0f, Math.Min(1f, fade));

            // Рамка, за да прилича на панела, който временно покрива
            var bounds = new Rectangle(1, 1, Width - 3, Height - 3);
            using (GraphicsPath path = Theme.RoundedRect(bounds, 16))
            using (var pen = new Pen(Theme.WithAlpha(Theme.NeonGreen, (int)(160 * fade)), 1.6f))
            {
                g.DrawPath(pen, path);
            }

            if (_mode == BurstMode.Coins)
            {
                DrawCoins(g, fade);
                DrawPaidStamp(g, elapsed, fade);
            }
            else
            {
                DrawOrderDrop(g, elapsed, fade);
            }
        }

        /// <summary>Летящите монети.</summary>
        private void DrawCoins(Graphics g, float fade)
        {
            foreach (Coin coin in _coins)
            {
                int alpha = (int)(230 * fade);
                if (alpha <= 0) continue;

                float squeeze = 0.55f + 0.45f * (float)Math.Abs(Math.Cos(coin.Spin)); // "въртене"
                var rect = new RectangleF(
                    coin.X - coin.Radius * squeeze,
                    coin.Y - coin.Radius,
                    coin.Radius * 2 * squeeze,
                    coin.Radius * 2);

                using (var body = new SolidBrush(Color.FromArgb(alpha, 255, 200, 60)))
                {
                    g.FillEllipse(body, rect);
                }
                using (var rim = new Pen(Color.FromArgb(alpha, 200, 140, 20), 1.6f))
                {
                    g.DrawEllipse(rim, rect);
                }
                using (var shine = new SolidBrush(Color.FromArgb(alpha / 2, 255, 255, 255)))
                {
                    g.FillEllipse(shine,
                        rect.X + rect.Width * 0.2f, rect.Y + rect.Height * 0.15f,
                        rect.Width * 0.3f, rect.Height * 0.3f);
                }
            }
        }

        private static float EaseOutCubic(float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return 1f - (float)Math.Pow(1f - t, 3);
        }

        /// <summary>
        /// „ПЛАТЕНО“ като истински гумен печат: завъртян, с двойна рамка,
        /// който се „удря“ (scale 1.6 -> 1.0) и после избледнява.
        /// </summary>
        private void DrawPaidStamp(Graphics g, double elapsed, float fade)
        {
            float punchIn = (float)Math.Min(1.0, elapsed / 240.0);
            if (punchIn <= 0f) return;

            float scale = 1.6f - 0.6f * EaseOutCubic(punchIn);
            int alpha = (int)(235 * punchIn * fade);
            if (alpha <= 0) return;

            Color stamp = Theme.NeonGreen;

            var state = g.Save();
            g.TranslateTransform(Width / 2f, Height / 2f);
            g.RotateTransform(-12f);          // леко накриво – като истински печат
            g.ScaleTransform(scale, scale);

            using (var font = new Font("Segoe UI", 22f, FontStyle.Bold))
            {
                Size textSize = TextRenderer.MeasureText(g, "ПЛАТЕНО", font,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);

                int w = textSize.Width + 44;
                int h = textSize.Height + 22;
                var box = new Rectangle(-w / 2, -h / 2, w, h);

                // Двойна рамка на печата
                using (GraphicsPath outer = Theme.RoundedRect(box, 10))
                using (var outerPen = new Pen(Color.FromArgb(alpha, stamp), 3f))
                {
                    g.DrawPath(outerPen, outer);
                }
                var innerBox = new Rectangle(box.X + 7, box.Y + 7, box.Width - 14, box.Height - 14);
                using (GraphicsPath inner = Theme.RoundedRect(innerBox, 7))
                using (var innerPen = new Pen(Color.FromArgb((int)(alpha * 0.7f), stamp), 1.6f))
                {
                    g.DrawPath(innerPen, inner);
                }

                TextRenderer.DrawText(g, "ПЛАТЕНО", font, box,
                    Color.FromArgb(alpha, stamp),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            }

            g.Restore(state);
        }

        /// <summary>
        /// Анимация при нова поръчка: малка бирена халба пада отгоре,
        /// „тупва“ с леко сплескване в тефтера и се появява „Записано!“.
        /// </summary>
        private void DrawOrderDrop(Graphics g, double elapsed, float fade)
        {
            float cx = Width / 2f;
            float floorY = Height * 0.62f;

            // Позиция на халбата: пада с ускорение, после кратък отскок
            double dropT = Math.Min(1.0, elapsed / 480.0);
            float y;
            float squash = 1f;

            if (dropT < 1.0)
            {
                y = (float)(-40 + (floorY + 40) * dropT * dropT); // свободно падане
            }
            else
            {
                double bounceT = Math.Min(1.0, (elapsed - 480.0) / 260.0);
                float bounce = (float)(Math.Sin(bounceT * Math.PI) * 14);
                y = floorY - bounce;
                squash = bounceT < 0.25 ? 0.82f : 1f; // кратко сплескване при удара
            }

            int alpha = (int)(255 * fade);
            float mugW = 30 * (2f - squash) * 0.92f;
            float mugH = 34 * squash;

            // Тяло на халбата
            using (var beer = new SolidBrush(Color.FromArgb((int)(alpha * 0.85f), 235, 160, 40)))
            {
                g.FillRectangle(beer, cx - mugW / 2, y - mugH + 8, mugW, mugH - 8);
            }
            using (var foam = new SolidBrush(Color.FromArgb(alpha, 248, 244, 230)))
            {
                g.FillEllipse(foam, cx - mugW / 2 - 2, y - mugH, mugW + 4, 12);
            }
            using (var handlePen = new Pen(Color.FromArgb((int)(alpha * 0.8f), 220, 220, 228), 3f))
            {
                g.DrawArc(handlePen, cx + mugW / 2 - 2, y - mugH + 12, 12, 16, 300, 120);
            }

            // Линия на "тефтера", върху която пада халбата
            using (var ledgerPen = new Pen(Theme.WithAlpha(Theme.Amber, (int)(alpha * 0.5f)), 2f))
            {
                g.DrawLine(ledgerPen, cx - 60, floorY + 6, cx + 60, floorY + 6);
            }

            // „Записано!“ – появява се след удара
            if (elapsed > 480)
            {
                float textIn = (float)Math.Min(1.0, (elapsed - 480) / 200.0);
                int textAlpha = (int)(255 * textIn * fade);

                using (var font = new Font("Segoe UI", 13f, FontStyle.Bold))
                {
                    var rect = new Rectangle(0, (int)(floorY + 12), Width, 30);
                    TextRenderer.DrawText(g, "✓ Записано!", font, rect,
                        Color.FromArgb(textAlpha, Theme.NeonGreen),
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.SingleLine);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
