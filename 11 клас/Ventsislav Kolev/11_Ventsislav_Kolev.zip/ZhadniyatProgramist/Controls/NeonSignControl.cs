using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ZhadniyatProgramist.Services;

namespace ZhadniyatProgramist.Controls
{
    /// <summary>
    /// Малка декоративна неонова табела (по подразбиране „ОТВОРЕНО“):
    ///  - премигва леко (плавен синусоидален пулс + рядко късо трепване);
    ///  - от време на време ЕДНА буква „изгасва“ за миг – като истински неон;
    ///  - вечер (Theme.IsNight) свети значително по-силно;
    ///  - текстът и цветът се задават (Text / SignColor), затова същият
    ///    контрол рисува и голямата табела „ЖАДНИЯТ ПРОГРАМИСТ“.
    /// </summary>
    public class NeonSignControl : Control
    {
        private readonly Timer _timer;
        private readonly Random _random = new Random();
        private double _phase;
        private float _flickerDip = 1f;
        private int _dimLetterIndex = -1;            // коя буква е "изгаснала"
        private DateTime _dimLetterUntil = DateTime.MinValue;

        /// <summary>Основен цвят на неона (кехлибарен по подразбиране).</summary>
        public Color SignColor { get; set; } = Theme.Amber;

        public NeonSignControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw, true);

            BackColor = Theme.Surface;
            Size = new Size(118, 34);
            Text = "ОТВОРЕНО";
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            Enabled = false;
            TabStop = false;

            _timer = new Timer { Interval = 70 };
            _timer.Tick += (s, e) =>
            {
                _phase += 0.18;

                if (_random.NextDouble() < 0.015)
                {
                    _flickerDip = 0.4f; // късо реалистично трепване на неона
                }
                _flickerDip += (1f - _flickerDip) * 0.25f;

                // Рядко една случайна буква "изгасва" за ~200 ms
                if (_dimLetterIndex < 0 && Text.Length > 0 && _random.NextDouble() < 0.02)
                {
                    _dimLetterIndex = _random.Next(Text.Length);
                    _dimLetterUntil = DateTime.Now.AddMilliseconds(200);
                }
                if (_dimLetterIndex >= 0 && DateTime.Now >= _dimLetterUntil)
                {
                    _dimLetterIndex = -1;
                }

                Invalidate();
            };
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            _dimLetterIndex = -1;
            Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                _timer.Start();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var behind = new SolidBrush(Parent?.BackColor ?? Theme.Surface))
            {
                g.FillRectangle(behind, ClientRectangle);
            }

            var bounds = new Rectangle(1, 1, Width - 3, Height - 3);
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Вечер табелата свети по-силно
            double pulse = AnimationService.Pulse(_phase);
            float baseGlow = Theme.IsNight ? 1f : 0.55f;
            float glow = (float)((0.65 + 0.35 * pulse) * baseGlow * _flickerDip);

            using (GraphicsPath path = Theme.RoundedRect(bounds, bounds.Height / 2))
            {
                using (var fill = new SolidBrush(Color.FromArgb(26, 20, 8)))
                {
                    g.FillPath(fill, path);
                }

                // Ореол около рамката
                using (var glowPen = new Pen(Theme.WithAlpha(SignColor, (int)(110 * glow)), 4f))
                {
                    g.DrawPath(glowPen, path);
                }
                using (var borderPen = new Pen(Theme.WithAlpha(SignColor, (int)(120 + 130 * glow)), 1.6f))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // Самият надпис – буква по буква, с леко "нажежаване".
            // Отделните букви позволяват ефекта "изгаснала буква".
            Color litColor = Theme.Lerp(
                Theme.Lerp(SignColor, Color.Black, 0.35f),
                Theme.Lerp(SignColor, Color.White, 0.45f),
                glow);
            Color dimColor = Theme.Lerp(SignColor, Color.Black, 0.72f);

            string text = Text ?? string.Empty;

            // Общата ширина, за да центрираме реда от букви
            int totalWidth = 0;
            var widths = new int[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                widths[i] = TextRenderer.MeasureText(g, text[i].ToString(), Font,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;
                totalWidth += widths[i];
            }

            int x = bounds.X + Math.Max(2, (bounds.Width - totalWidth) / 2);

            for (int i = 0; i < text.Length; i++)
            {
                Color letterColor = (i == _dimLetterIndex) ? dimColor : litColor;
                var letterRect = new Rectangle(x, bounds.Y, widths[i] + 4, bounds.Height);

                TextRenderer.DrawText(g, text[i].ToString(), Font, letterRect, letterColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);

                x += widths[i];
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
