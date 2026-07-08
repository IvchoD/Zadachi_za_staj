using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ZhadniyatProgramist.Controls
{
    /// <summary>
    /// Декоративен „жив“ слой с атмосферата на кръчмата:
    ///  - топли лампи, които пулсират съвсем леко;
    ///  - малки NPC силуети на клиенти на бара, които се полюшват;
    ///  - неонов glow по ръбовете на прозореца (по-силен нощем);
    ///  - случайни отблясъци по бутилките.
    ///
    /// Както при BubbleEffectControl, контролът може да се ползва
    /// самостоятелно ИЛИ чрез вградения двигател (AmbientScene),
    /// който AnimatedBackgroundPanel композира директно във фона –
    /// без допълнителни прозрачни контроли и без натоварване.
    /// Всичко е нарочно subtle: ниска прозрачност, бавни фази.
    /// </summary>
    public class TavernAmbientLayer : Control
    {
        private readonly Timer _timer;
        private readonly AmbientScene _scene = new AmbientScene();

        public TavernAmbientLayer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw, true);

            BackColor = Theme.Background;
            Enabled = false; // не пречи на кликове
            TabStop = false;

            _timer = new Timer { Interval = 100 };
            _timer.Tick += (s, e) =>
            {
                _scene.Step(ClientSize);
                Invalidate();
            };
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
            base.OnPaint(e);
            _scene.Draw(e.Graphics, ClientRectangle, dayMode: !Theme.IsNight);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
            }
            base.Dispose(disposing);
        }

        // =====================================================================
        //  Двигателят на атмосферата – използваем и от AnimatedBackgroundPanel
        // =====================================================================

        /// <summary>Логика и рисуване на лампи, NPC-та, ръбов неон и отблясъци.</summary>
        public class AmbientScene
        {
            private sealed class Npc
            {
                public float RelX;        // 0..1 – позиция по ширината
                public float Scale;       // размер на силуета
                public float SwayPhase;   // полюшване наляво/надясно
                public float BobPhase;    // леко кимане
                public float SwaySpeed;
            }

            private sealed class Sparkle
            {
                public float X, Y;
                public float Life;   // 1 -> 0
            }

            private readonly Random _random = new Random();
            private readonly List<Npc> _npcs = new List<Npc>();
            private readonly List<Sparkle> _sparkles = new List<Sparkle>();
            private double _lampPhase;
            private double _edgePhase;

            public AmbientScene()
            {
                // Трима редовни клиенти на бара (позициите са относителни,
                // за да работят при всякакъв размер на прозореца)
                float[] positions = { 0.16f, 0.45f, 0.71f };
                foreach (float relX in positions)
                {
                    _npcs.Add(new Npc
                    {
                        RelX = relX,
                        Scale = 0.85f + (float)(_random.NextDouble() * 0.3),
                        SwayPhase = (float)(_random.NextDouble() * Math.PI * 2),
                        BobPhase = (float)(_random.NextDouble() * Math.PI * 2),
                        SwaySpeed = 0.018f + (float)(_random.NextDouble() * 0.012)
                    });
                }
            }

            /// <summary>Една стъпка от анимацията (вика се от Timer, ~10 fps).</summary>
            public void Step(Size area)
            {
                _lampPhase += 0.045;   // много бавен пулс на лампите
                _edgePhase += 0.06;

                foreach (Npc npc in _npcs)
                {
                    npc.SwayPhase += npc.SwaySpeed;
                    npc.BobPhase += npc.SwaySpeed * 1.6f;
                }

                // Случайни къси отблясъци по бутилките на бара
                if (area.Width > 0 && _sparkles.Count < 3 && _random.NextDouble() < 0.06)
                {
                    // Бутилките са на всеки 230 px от x=90 (виж AnimatedBackgroundPanel)
                    int bottleCount = Math.Max(1, (area.Width - 150) / 230);
                    int index = _random.Next(bottleCount);
                    _sparkles.Add(new Sparkle
                    {
                        X = 90 + index * 230 + 7,
                        Y = area.Height - 64 - 20 - _random.Next(14),
                        Life = 1f
                    });
                }

                for (int i = _sparkles.Count - 1; i >= 0; i--)
                {
                    _sparkles[i].Life -= 0.08f;
                    if (_sparkles[i].Life <= 0)
                    {
                        _sparkles.RemoveAt(i);
                    }
                }
            }

            /// <summary>Рисува целия декоративен слой (subtle, ниска прозрачност).</summary>
            public void Draw(Graphics g, Rectangle bounds, bool dayMode)
            {
                if (bounds.Width <= 0 || bounds.Height <= 0) return;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                DrawLamps(g, bounds, dayMode);
                DrawNpcs(g, bounds, dayMode);
                DrawEdgeNeon(g, bounds, dayMode);
                DrawSparkles(g);
            }

            /// <summary>Топли кръчмарски лампи отгоре, пулсиращи много леко.</summary>
            private void DrawLamps(Graphics g, Rectangle bounds, bool dayMode)
            {
                float[] lampX = { 0.22f, 0.52f, 0.80f };
                Color warm = Color.FromArgb(255, 200, 120);

                for (int i = 0; i < lampX.Length; i++)
                {
                    // Всяка лампа пулсира с леко различна фаза
                    double pulse = (Math.Sin(_lampPhase + i * 1.7) + 1.0) * 0.5;
                    int alpha = (int)((dayMode ? 7 : 14) + (dayMode ? 4 : 7) * pulse);

                    float cx = bounds.Width * lampX[i];
                    float r = Math.Min(bounds.Width, bounds.Height) * 0.30f;

                    using (var path = new GraphicsPath())
                    {
                        path.AddEllipse(cx - r, -r * 0.8f, r * 2, r * 2);
                        using (var glow = new PathGradientBrush(path))
                        {
                            glow.CenterColor = Color.FromArgb(alpha, warm);
                            glow.SurroundColors = new[] { Color.FromArgb(0, warm) };
                            g.FillPath(glow, path);
                        }
                    }
                }
            }

            /// <summary>Тъмни силуети на клиенти, седнали на бара, леко полюшващи се.</summary>
            private void DrawNpcs(Graphics g, Rectangle bounds, bool dayMode)
            {
                int barTop = bounds.Height - 64; // горният ръб на дървения бар
                int alpha = dayMode ? 28 : 44;
                Color silhouette = Color.FromArgb(alpha, 8, 10, 14);

                foreach (Npc npc in _npcs)
                {
                    float sway = (float)Math.Sin(npc.SwayPhase) * 3f;
                    float bob = (float)Math.Sin(npc.BobPhase) * 1.6f;

                    float baseW = 46 * npc.Scale;
                    float headR = 11 * npc.Scale;
                    float cx = bounds.Width * npc.RelX + sway;
                    float shouldersTop = barTop - 34 * npc.Scale + bob;

                    using (var brush = new SolidBrush(silhouette))
                    {
                        // Рамене/гръб (полуелипса над бара)
                        g.FillPie(brush,
                            cx - baseW / 2, shouldersTop,
                            baseW, 46 * npc.Scale, 180, 180);

                        // Глава
                        g.FillEllipse(brush,
                            cx - headR + sway * 0.4f,
                            shouldersTop - headR * 1.6f + bob * 0.6f,
                            headR * 2, headR * 2);

                        // Халба до клиента (мъничък детайл)
                        g.FillRectangle(brush,
                            cx + baseW / 2 + 6, barTop - 12 * npc.Scale,
                            8 * npc.Scale, 12 * npc.Scale);
                    }
                }
            }

            /// <summary>Неонов glow по ръбовете на прозореца – по-осезаем нощем.</summary>
            private void DrawEdgeNeon(Graphics g, Rectangle bounds, bool dayMode)
            {
                double pulse = (Math.Sin(_edgePhase) + 1.0) * 0.5;
                int alpha = (int)((dayMode ? 8 : 20) + (dayMode ? 4 : 10) * pulse);
                if (alpha <= 0) return;

                int glowSize = 26;

                // Ляв и десен ръб – зелен неон
                using (var left = new LinearGradientBrush(
                           new Rectangle(0, 0, glowSize, bounds.Height),
                           Theme.WithAlpha(Theme.NeonGreen, alpha),
                           Color.FromArgb(0, Theme.NeonGreen),
                           LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(left, 0, 0, glowSize, bounds.Height);
                }
                using (var right = new LinearGradientBrush(
                           new Rectangle(bounds.Width - glowSize, 0, glowSize, bounds.Height),
                           Color.FromArgb(0, Theme.NeonGreen),
                           Theme.WithAlpha(Theme.NeonGreen, alpha),
                           LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(right, bounds.Width - glowSize, 0, glowSize, bounds.Height);
                }

                // Долен ръб – кехлибарен неон (над бара)
                using (var bottom = new LinearGradientBrush(
                           new Rectangle(0, bounds.Height - glowSize, bounds.Width, glowSize),
                           Color.FromArgb(0, Theme.Amber),
                           Theme.WithAlpha(Theme.Amber, alpha),
                           LinearGradientMode.Vertical))
                {
                    g.FillRectangle(bottom, 0, bounds.Height - glowSize, bounds.Width, glowSize);
                }
            }

            /// <summary>Къси звездички-отблясъци по бутилките.</summary>
            private void DrawSparkles(Graphics g)
            {
                foreach (Sparkle s in _sparkles)
                {
                    int alpha = (int)(120 * s.Life);
                    float r = 2.2f + 2.5f * (1f - s.Life);

                    using (var pen = new Pen(Color.FromArgb(alpha, 255, 240, 200), 1.3f))
                    {
                        g.DrawLine(pen, s.X - r, s.Y, s.X + r, s.Y);
                        g.DrawLine(pen, s.X, s.Y - r, s.X, s.Y + r);
                    }
                }
            }
        }
    }
}
