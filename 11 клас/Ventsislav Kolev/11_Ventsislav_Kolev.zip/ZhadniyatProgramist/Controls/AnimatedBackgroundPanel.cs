using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ZhadniyatProgramist.Controls
{
    /// <summary>
    /// „Жив“ фон на приложението.
    ///
    /// Наследява TableLayoutPanel и служи едновременно като:
    ///  - коренна решетка на MainForm (редове/колони на layout-а);
    ///  - анимиран фон: бавно движеща се мека светлина, едва доловимо
    ///    премигване на неоновия акцент и плаващи бирени мехурчета
    ///    (двигателят от BubbleEffectControl.BubbleField).
    ///
    /// Дъщерните панели са непрозрачни и Windows ги изрязва от фона
    /// (WS_CLIPCHILDREN), затова на всеки тик се прерисуват САМО
    /// празнините между картите – анимацията не товари приложението.
    /// Поддържа ден/нощ режим чрез свойството DayMode.
    /// </summary>
    public class AnimatedBackgroundPanel : TableLayoutPanel
    {
        private readonly Timer _timer;
        private readonly BubbleEffectControl.BubbleField _bubbles =
            new BubbleEffectControl.BubbleField();
        private readonly TavernAmbientLayer.AmbientScene _ambient =
            new TavernAmbientLayer.AmbientScene();

        private double _lightPhase;   // движение на светлото петно
        private double _flickerPhase; // премигване на неоновата линия
        private readonly Random _random = new Random();
        private float _flickerDip = 1f; // случайни къси "трепвания" на неона

        /// <summary>true = дневна смяна (по-светъл фон, по-меки неонови ефекти).</summary>
        [Category("Behavior")]
        public bool DayMode { get; set; } = true;

        public AnimatedBackgroundPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw, true);

            BackColor = Theme.Background;

            _timer = new Timer { Interval = 100 };
            _timer.Tick += Timer_Tick;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                _timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _lightPhase += 0.012;   // много бавно движение на светлината
            _flickerPhase += 0.25;

            // Рядко, кратко "трепване" на неона – като истинска табела
            if (_random.NextDouble() < 0.02)
            {
                _flickerDip = 0.55f;
            }
            _flickerDip += (1f - _flickerDip) * 0.3f;

            _bubbles.Step(ClientSize);
            _ambient.Step(ClientSize);
            Invalidate(); // дъщерните контроли се изрязват – рисуват се само празнините

            // Прозрачните контейнери (решетките със статистиката, формулярите
            // и статус бара) показват този фон – опресняваме и тях, иначе
            // в техните празнини би останал "замръзнал" кадър.
            foreach (Control child in Controls)
            {
                if (child is TableLayoutPanel && child.BackColor == Color.Transparent)
                {
                    child.Invalidate();
                }
            }
        }

        /// <summary>
        /// Прозрачните дъщерни решетки получават двойно буфериране
        /// (вътрешно свойство), за да няма трептене при анимацията на фона.
        /// </summary>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control is TableLayoutPanel && e.Control.BackColor == Color.Transparent)
            {
                try
                {
                    typeof(Control).InvokeMember(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.SetProperty,
                        null, e.Control, new object[] { true });
                }
                catch
                {
                    // Чисто оптимизационно – без него всичко пак работи.
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = ClientRectangle;
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // 1) Основен фон: през деня една идея по-светъл
            Color baseColor = DayMode ? Theme.Lighten(Theme.Background, 6) : Theme.Background;
            Color bottomColor = DayMode
                ? Theme.Lighten(Theme.Background, 2)
                : Color.FromArgb(14, 17, 22);

            using (var backBrush = new LinearGradientBrush(
                       bounds, baseColor, bottomColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(backBrush, bounds);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 2) Бавно движещо се меко светло петно
            float lightX = bounds.Width * (0.5f + 0.35f * (float)Math.Sin(_lightPhase));
            float lightY = bounds.Height * (0.30f + 0.10f * (float)Math.Cos(_lightPhase * 0.7));
            float lightR = Math.Max(bounds.Width, bounds.Height) * 0.55f;

            using (var lightPath = new GraphicsPath())
            {
                lightPath.AddEllipse(lightX - lightR, lightY - lightR, lightR * 2, lightR * 2);

                using (var lightBrush = new PathGradientBrush(lightPath))
                {
                    int strength = DayMode ? 10 : 7; // денем светлината е малко по-осезаема
                    lightBrush.CenterColor = Theme.WithAlpha(Color.White, strength);
                    lightBrush.SurroundColors = new[] { Color.FromArgb(0, 255, 255, 255) };
                    g.FillPath(lightBrush, lightPath);
                }
            }

            // 3) Тънка неонова линия горе, която леко "диша"/премигва.
            //    Нощем неонът свети по-силно.
            double pulse = (Math.Sin(_flickerPhase) + 1.0) * 0.5;
            int neonBase = DayMode ? 26 : 60;
            int neonAlpha = (int)((neonBase + 30 * pulse) * _flickerDip);

            using (var neonBrush = new LinearGradientBrush(
                       new Rectangle(0, 0, bounds.Width, 4),
                       Theme.WithAlpha(Theme.NeonGreen, 0),
                       Theme.WithAlpha(Theme.NeonGreen, neonAlpha),
                       LinearGradientMode.Horizontal))
            {
                neonBrush.SetBlendTriangularShape(0.5f);
                g.FillRectangle(neonBrush, 0, 0, bounds.Width, 3);
            }

            // 4) Атмосфера на кръчма: винетка + дървен бар с бутилки (дискретно)
            DrawTavernDecor(g, bounds);

            // 5) Жив декоративен слой: лампи, NPC силуети, ръбов неон, отблясъци
            _ambient.Draw(g, bounds, DayMode);

            // 6) Плаващи бирени мехурчета в празнините между панелите
            _bubbles.Draw(g, bounds);
        }

        /// <summary>
        /// Дискретна кръчмарска атмосфера във фона: лека тъмна винетка
        /// по краищата, дървен „бар“ най-долу с дъски и силуети на бутилки
        /// с кехлибарени отблясъци. Всичко е с ниска прозрачност, така че
        /// НЕ пречи на четимостта – панелите отгоре са непрозрачни.
        /// </summary>
        private void DrawTavernDecor(Graphics g, Rectangle bounds)
        {
            // --- Винетка: леко притъмняване към ръбовете (атмосферично осветление) ---
            int vignetteAlpha = DayMode ? 26 : 46;
            int edge = Math.Max(40, bounds.Width / 12);

            using (var left = new LinearGradientBrush(
                       new Rectangle(0, 0, edge, bounds.Height),
                       Color.FromArgb(vignetteAlpha, 0, 0, 0), Color.FromArgb(0, 0, 0, 0),
                       LinearGradientMode.Horizontal))
            {
                g.FillRectangle(left, 0, 0, edge, bounds.Height);
            }
            using (var right = new LinearGradientBrush(
                       new Rectangle(bounds.Width - edge, 0, edge, bounds.Height),
                       Color.FromArgb(0, 0, 0, 0), Color.FromArgb(vignetteAlpha, 0, 0, 0),
                       LinearGradientMode.Horizontal))
            {
                g.FillRectangle(right, bounds.Width - edge, 0, edge, bounds.Height);
            }

            // --- Дървен бар най-долу ---
            int barHeight = 64;
            var barRect = new Rectangle(0, bounds.Height - barHeight, bounds.Width, barHeight);

            using (var wood = new LinearGradientBrush(
                       barRect,
                       Color.FromArgb(DayMode ? 34 : 48, 92, 62, 36),
                       Color.FromArgb(DayMode ? 22 : 34, 60, 40, 24),
                       LinearGradientMode.Vertical))
            {
                g.FillRectangle(wood, barRect);
            }

            // Фуги между дъските
            using (var plankPen = new Pen(Color.FromArgb(DayMode ? 16 : 26, 30, 20, 12), 1.4f))
            {
                for (int x = 60; x < bounds.Width; x += 170)
                {
                    g.DrawLine(plankPen, x, barRect.Top + 6, x, barRect.Bottom - 4);
                }
                g.DrawLine(plankPen, 0, barRect.Top + 2, bounds.Width, barRect.Top + 2);
            }

            // Силуети на бутилки върху бара с кехлибарени отблясъци
            int bottleAlpha = DayMode ? 22 : 38;
            int glintAlpha = DayMode ? 30 : 55;

            for (int x = 90; x < bounds.Width - 60; x += 230)
            {
                int bw = 14, bh = 40;
                int bx = x, by = barRect.Top - bh + 8;

                using (var bottle = new SolidBrush(Color.FromArgb(bottleAlpha, 18, 40, 26)))
                {
                    g.FillRectangle(bottle, bx, by + 12, bw, bh - 12);          // тяло
                    g.FillRectangle(bottle, bx + bw / 2 - 3, by, 6, 14);        // гърло
                }
                using (var glint = new SolidBrush(Theme.WithAlpha(Theme.Amber, glintAlpha)))
                {
                    g.FillRectangle(glint, bx + 2, by + 16, 3, bh - 20);        // отблясък
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
