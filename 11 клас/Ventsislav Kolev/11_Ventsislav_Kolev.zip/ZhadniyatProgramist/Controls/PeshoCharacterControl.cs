using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using ZhadniyatProgramist.Services;

namespace ZhadniyatProgramist.Controls
{
    /// <summary>
    /// Бай Пешо – анимираният стопанин на кръчмата и център на приложението.
    ///
    /// Състояния според дълга на избрания програмист:
    ///  - Idle    : чака си клиенти;
    ///  - Happy   : 0 лв. – усмивка, намигване, вдигнат палец („Браво!“);
    ///  - Wary    : 1–20 лв. – леко намръщен, почуква по бара;
    ///  - Warning : 20–50 лв. – кръстосва ръце, клати глава;
    ///  - Angry   : 50–100 лв. – размахва пръст („Айде стига бе!“);
    ///  - Furious : 100+ лв. – бесен, трепери силно, червен glow („Изчезвай!“).
    ///
    /// Постоянни „живи“ ефекти: дишане, мигане на 4–6 сек, движение на
    /// очите, потрепване на мустака. Комикс балонът се появява със
    /// scale + bounce + fade и сменя текста и цвета си.
    ///
    /// Всичко е с ЕДИН Timer и GDI+ рисуване – без 15–20 PNG кадъра.
    /// Ако в Assets/Pesho има pesho_&lt;състояние&gt;.png, ползва се картинката;
    /// липсата на файлове НЕ предизвиква краш.
    /// </summary>
    public class PeshoCharacterControl : Control
    {
        private enum Mood { Idle, Happy, Wary, Warning, Angry, Furious }

        private readonly Timer _timer;
        private readonly Random _random = new Random();

        private Mood _mood = Mood.Idle;
        private string _bubbleText = "Ела да те запиша в тефтера…";
        private Color _bubbleTextColor = Theme.TextMuted;

        // --- Дишане, мигане, очи, мустак ---
        private double _breathPhase;
        private double _mustachePhase;
        private DateTime _nextBlinkAt;
        private DateTime _blinkEndsAt = DateTime.MinValue;
        private bool _winkOnly;                    // при щастлив – намига с едно око
        private float _pupilX, _pupilTargetX;      // движение на очите (-1..1)
        private DateTime _nextGazeChangeAt;

        // --- Подскачане при радост ---
        private DateTime _hopEndsAt = DateTime.MinValue;
        private double _hopPhase;

        // --- Ефект „почукване по бара“ (тук-тук) ---
        private DateTime _knockEndsAt = DateTime.MinValue;
        private int _knockJerk;

        // --- Размахване на пръст / клатене на глава ---
        private double _gesturePhase;

        // --- Случайни кратки реакции на всеки 10–15 сек (кръчмата е жива) ---
        private DateTime _nextQuirkAt;
        private DateTime _browRaiseUntil = DateTime.MinValue; // вдигната вежда

        // --- Доволно кимане при плащане ---
        private DateTime _nodEndsAt = DateTime.MinValue;
        private double _nodPhase;

        // --- Пара над бирената халба на бара ---
        private double _steamPhase;

        // --- Мърморене под мустак (случайни реплики) ---
        private static readonly string[] GrumbleLines =
            { "Ех, младежи…", "Кой ще плати това…", "Пак на вересия…", "Мърмори под мустак…" };

        // --- Комикс балон: scale + bounce + fade при смяна на текста ---
        private DateTime _bubbleShownAt = DateTime.MinValue;

        // --- Временна реплика (връща се към репликата на настроението) ---
        private DateTime _tempBubbleUntil = DateTime.MinValue;
        private string _moodBubbleText;
        private Color _moodBubbleColor;

        // --- Реплики по нива на дълга ---
        private static readonly string[] HappyLines =
            { "Браво!", "Такъв клиент се уважава.", "Ела пак!" };
        private static readonly string[] WaryLines =
            { "Айде, няма проблем.", "Записвам всичко, да знаеш…" };
        private static readonly string[] WarningLines =
            { "Доста си натрупал…", "Хм… сметката взе да расте!" };
        private static readonly string[] AngryLines =
            { "Айде стига бе!", "Плати си!" };
        private static readonly string[] FuriousLines =
            { "Изчезвай!", "Първо плащай!" };

        public PeshoCharacterControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.UserPaint
                   | ControlStyles.ResizeRedraw, true);

            BackColor = Theme.Surface;
            _moodBubbleText = _bubbleText;
            _moodBubbleColor = _bubbleTextColor;
            _bubbleShownAt = DateTime.Now;

            _timer = new Timer { Interval = 40 };
            _timer.Tick += Timer_Tick;

            ScheduleNextBlink();
            _nextGazeChangeAt = DateTime.Now.AddSeconds(2);
            _nextQuirkAt = DateTime.Now.AddSeconds(8); // първата случайна реакция
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                _timer.Start();
            }
        }

        // =====================================================================
        //  Публично API (използва се от MainForm)
        // =====================================================================

        /// <summary>Пешо чака спокойно (няма избран програмист).</summary>
        public void ShowIdle()
        {
            SetMood(Mood.Idle, "Ела да те запиша в тефтера…", Theme.TextMuted);
        }

        /// <summary>Настроението на Пешо според текущия дълг на избрания човек.</summary>
        public void UpdateMood(decimal debt)
        {
            if (debt <= 0)
            {
                SetMood(Mood.Happy, Pick(HappyLines), Theme.NeonGreen);
            }
            else if (debt < 20)
            {
                SetMood(Mood.Wary, Pick(WaryLines), Theme.Amber);
            }
            else if (debt <= 50)
            {
                SetMood(Mood.Warning, Pick(WarningLines), Theme.Amber);
            }
            else if (debt <= 100)
            {
                SetMood(Mood.Angry, Pick(AngryLines), Theme.RedBright);
            }
            else
            {
                SetMood(Mood.Furious, Pick(FuriousLines), Theme.RedBright);
            }
        }

        /// <summary>Радост при платена сметка – Пешо се смее, намига, кима и подскача.</summary>
        public void Celebrate()
        {
            SetMood(Mood.Happy, "Наздраве! Ей така те искам!", Theme.NeonGreen);
            _hopEndsAt = DateTime.Now.AddMilliseconds(1500);
            _hopPhase = 0;
            _winkOnly = true;
            _blinkEndsAt = DateTime.Now.AddMilliseconds(450); // дълго намигане
            _nodEndsAt = DateTime.Now.AddMilliseconds(1600);  // доволно кимане
            _nodPhase = 0;
        }

        /// <summary>
        /// Реакция при добавена нова поръчка.
        /// Над 50 лв. – строго „почукване по бара“ и реплика
        /// „Първо плащаш, после поръчваш!“.
        /// </summary>
        public void ReactToNewOrder(decimal newDebt)
        {
            UpdateMood(newDebt);

            if (newDebt > 50)
            {
                _knockEndsAt = DateTime.Now.AddMilliseconds(1600);
                _knockJerk = 6; // 3 бързи разклащания (по 2 тика всяко)
                ShowTemporaryBubble("Първо плащаш, после поръчваш!", Theme.RedBright, 2600);
            }
            else
            {
                ShowTemporaryBubble("Записах го в тефтера! 🍺", Theme.TextPrimary, 1800);
            }
        }

        // =====================================================================
        //  Вътрешна логика
        // =====================================================================

        private string Pick(string[] lines)
        {
            return lines[_random.Next(lines.Length)];
        }

        private void SetMood(Mood mood, string bubbleText, Color bubbleColor)
        {
            bool changed = _mood != mood;
            _mood = mood;
            _moodBubbleText = bubbleText;
            _moodBubbleColor = bubbleColor;

            // Временната реплика (ако има) остава с приоритет до изтичането си.
            if (DateTime.Now >= _tempBubbleUntil)
            {
                if (changed || _bubbleText != bubbleText)
                {
                    _bubbleShownAt = DateTime.Now; // рестарт на scale/bounce анимацията
                }
                _bubbleText = bubbleText;
                _bubbleTextColor = bubbleColor;
            }

            // Леко почукване по бара при "леко намръщен" (1–20 лв.)
            if (changed && mood == Mood.Wary)
            {
                _knockEndsAt = DateTime.Now.AddMilliseconds(700);
                _knockJerk = 2;
            }

            Invalidate();
        }

        private void ShowTemporaryBubble(string text, Color color, int milliseconds)
        {
            _bubbleText = text;
            _bubbleTextColor = color;
            _bubbleShownAt = DateTime.Now;
            _tempBubbleUntil = DateTime.Now.AddMilliseconds(milliseconds);
            Invalidate();
        }

        private void ScheduleNextBlink()
        {
            _nextBlinkAt = DateTime.Now.AddMilliseconds(4000 + _random.Next(2000)); // 4–6 сек
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            _breathPhase += 0.07;      // дишане
            _mustachePhase += 0.11;    // потрепване на мустака
            _gesturePhase += 0.16;     // жестове (пръст, глава)
            _steamPhase += 0.09;       // пара над халбата

            if (now < _nodEndsAt)
            {
                _nodPhase += 0.22;     // доволно кимане при плащане
            }

            // Случайна кратка реакция на всеки 10–15 секунди –
            // само ако в момента не тече друга сценка (реплика/почукване).
            if (now >= _nextQuirkAt)
            {
                _nextQuirkAt = now.AddMilliseconds(10000 + _random.Next(5000));

                bool busy = now < _tempBubbleUntil || now < _knockEndsAt || now < _hopEndsAt;
                if (!busy)
                {
                    switch (_random.Next(4))
                    {
                        case 0: // поглежда встрани и задържа погледа
                            _pupilTargetX = _random.NextDouble() < 0.5 ? -1f : 1f;
                            _nextGazeChangeAt = now.AddMilliseconds(2600);
                            break;
                        case 1: // почуква по бара
                            _knockEndsAt = now.AddMilliseconds(600);
                            _knockJerk = 2;
                            break;
                        case 2: // мрънка в балончето
                            ShowTemporaryBubble(Pick(GrumbleLines), Theme.TextMuted, 2000);
                            break;
                        default: // вдига вежда
                            _browRaiseUntil = now.AddMilliseconds(900);
                            break;
                    }
                }
            }

            // Мигане (при щастлив понякога е намигане с едно око)
            if (now >= _nextBlinkAt)
            {
                _winkOnly = _mood == Mood.Happy && _random.NextDouble() < 0.5;
                _blinkEndsAt = now.AddMilliseconds(_winkOnly ? 320 : 140);
                ScheduleNextBlink();
            }

            // Движение на очите – поглед към случайна посока на всеки 2–4 сек
            if (now >= _nextGazeChangeAt)
            {
                _pupilTargetX = (float)(_random.NextDouble() * 2 - 1);
                _nextGazeChangeAt = now.AddMilliseconds(2000 + _random.Next(2000));
            }
            _pupilX += (_pupilTargetX - _pupilX) * 0.08f;

            // Подскачане
            if (now < _hopEndsAt)
            {
                _hopPhase += 0.24;
            }

            // Почукване – намаляващи резки движения
            if (now < _knockEndsAt && _knockJerk > 0 && _random.NextDouble() < 0.5)
            {
                _knockJerk--;
            }

            // Изтичане на временната реплика -> връщаме репликата на настроението
            if (_tempBubbleUntil != DateTime.MinValue && now >= _tempBubbleUntil)
            {
                _tempBubbleUntil = DateTime.MinValue;
                _bubbleText = _moodBubbleText;
                _bubbleTextColor = _moodBubbleColor;
                _bubbleShownAt = now;
            }

            Invalidate();
        }

        // =====================================================================
        //  Рисуване
        // =====================================================================

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var back = new SolidBrush(BackColor))
            {
                g.FillRectangle(back, ClientRectangle);
            }

            if (Width < 60 || Height < 80) return;

            DateTime now = DateTime.Now;

            // --- Общи отмествания (живот) ---
            float breath = (float)Math.Sin(_breathPhase) * 2f;   // дишане
            float offsetX = 0, offsetY = breath;

            if (_mood == Mood.Angry)
            {
                offsetX += _random.Next(-1, 2);                   // леко треперене
            }
            else if (_mood == Mood.Furious)
            {
                offsetX += _random.Next(-2, 3);                   // силно треперене
                offsetY += _random.Next(-1, 2);
            }

            if (now < _hopEndsAt)
            {
                offsetY -= (float)Math.Abs(Math.Sin(_hopPhase)) * 9f; // подскачане
            }

            if (now < _knockEndsAt)
            {
                offsetX += (_knockJerk % 2 == 0) ? 4 : -4;        // „тук-тук“ разклащане
            }

            // Клатене на глава при 20–50 лв.
            float headShake = _mood == Mood.Warning
                ? (float)Math.Sin(_gesturePhase * 0.7) * 3f
                : 0f;

            // Доволно кимане (вертикално поклащане на главата) при плащане
            float headNod = now < _nodEndsAt
                ? (float)Math.Sin(_nodPhase * 2.0) * 3.5f
                : 0f;

            // --- Разпределение: балонче горе, Пешо под него, надпис най-долу ---
            var bubbleArea = MeasureBubble(g, out string wrappedText, out float bubbleScale);
            int captionHeight = 24;

            var peshoArea = new Rectangle(
                0,
                bubbleArea.Bottom + 8,
                Width,
                Height - bubbleArea.Bottom - 8 - captionHeight);

            // Червен glow зад Пешо при 100+ лв.
            if (_mood == Mood.Furious && peshoArea.Width > 0 && peshoArea.Height > 0)
            {
                DrawFuriousGlow(g, peshoArea);
            }

            DrawBubble(g, bubbleArea, wrappedText, bubbleScale);

            // Ако има картинка за състоянието – ползваме нея, иначе рисуваме
            Image sprite = PeshoSprites.Get(_mood.ToString());
            if (sprite != null)
            {
                DrawSprite(g, sprite, peshoArea, offsetX, offsetY);
            }
            else
            {
                DrawPesho(g, peshoArea, offsetX, offsetY, headShake, headNod, now);
            }

            // Бирена халба на бара с лека „пара“ (и при sprite, и при рисуване)
            DrawMugWithSteam(g, peshoArea);

            // Иконка „тук-тук“ до Пешо
            if (now < _knockEndsAt)
            {
                using (var font = new Font("Segoe UI", 10.5f, FontStyle.Bold))
                {
                    // Иконката се ограничава така, че текстът ѝ винаги да се вижда цял
                    int knockX = Math.Min(
                        peshoArea.X + peshoArea.Width / 2 + (int)(peshoArea.Height * 0.28f),
                        Width - 114);
                    var knockPoint = new Rectangle(
                        Math.Max(4, knockX),
                        peshoArea.Y + peshoArea.Height / 3,
                        110, 26);
                    TextRenderer.DrawText(g, "👊 тук-тук!", font, knockPoint, Theme.Amber,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.SingleLine);
                }
            }

            // Надпис под Пешо
            using (var captionFont = new Font("Segoe UI", 9.5f, FontStyle.Italic))
            {
                var captionRect = new Rectangle(0, Height - captionHeight, Width, captionHeight);
                TextRenderer.DrawText(g, "Бай Пешо следи тефтера", captionFont, captionRect,
                    Theme.TextMuted,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine);
            }
        }

        /// <summary>Пулсиращ червен ореол зад Пешо, когато е бесен (100+ лв.).</summary>
        private void DrawFuriousGlow(Graphics g, Rectangle area)
        {
            float r = Math.Min(area.Width, area.Height) * 0.62f;
            float cx = area.X + area.Width / 2f;
            float cy = area.Y + area.Height / 2f;
            int alpha = 26 + (int)(20 * AnimationService.Pulse(_gesturePhase));

            using (var glowPath = new GraphicsPath())
            {
                glowPath.AddEllipse(cx - r, cy - r, r * 2, r * 2);
                using (var glow = new PathGradientBrush(glowPath))
                {
                    glow.CenterColor = Theme.WithAlpha(Theme.RedBright, alpha);
                    glow.SurroundColors = new[] { Color.FromArgb(0, Theme.RedBright) };
                    g.FillPath(glow, glowPath);
                }
            }
        }

        /// <summary>
        /// Изчислява размера на балончето така, че текстът да се вижда ЦЕЛИЯТ.
        /// Връща и текущия scale (комикс ефект: 0.75 -> 1.06 -> 1.0).
        /// </summary>
        private Rectangle MeasureBubble(Graphics g, out string text, out float scale)
        {
            text = _bubbleText ?? string.Empty;

            // Bounce-in анимация на балона при нова реплика
            double t = (DateTime.Now - _bubbleShownAt).TotalMilliseconds / 320.0;
            if (t >= 1.0)
            {
                scale = 1f;
            }
            else
            {
                double eased = AnimationService.EaseOutCubic(t);
                scale = (float)(0.75 + 0.31 * eased - 0.06 * Math.Max(0, eased - 0.8) * 5);
                scale = Math.Min(Math.Max(scale, 0.75f), 1.06f);
            }

            using (var font = new Font("Segoe UI Semibold", 10.5f))
            {
                int maxTextWidth = Math.Max(120, Width - 48);

                Size measured = TextRenderer.MeasureText(g, text, font,
                    new Size(maxTextWidth, int.MaxValue),
                    TextFormatFlags.WordBreak | TextFormatFlags.HorizontalCenter);

                int bubbleWidth = Math.Min(Width - 16, measured.Width + 30);
                int bubbleHeight = measured.Height + 18;

                int x = (Width - bubbleWidth) / 2;
                return new Rectangle(x, 4, bubbleWidth, bubbleHeight);
            }
        }

        private void DrawBubble(Graphics g, Rectangle bubble, string text, float scale)
        {
            // Scale около центъра на балона (комикс появяване)
            var scaled = new Rectangle(
                bubble.X + (int)(bubble.Width * (1 - scale) / 2),
                bubble.Y + (int)(bubble.Height * (1 - scale) / 2),
                Math.Max(10, (int)(bubble.Width * scale)),
                Math.Max(10, (int)(bubble.Height * scale)));

            int alpha = (int)(255 * Math.Min(1f, scale / 0.9f));

            using (GraphicsPath path = Theme.RoundedRect(scaled, 12))
            {
                using (var fill = new SolidBrush(Theme.WithAlpha(Theme.SurfaceLight, alpha)))
                {
                    g.FillPath(fill, path);
                }
                // Рамката на балона носи цвета на настроението – комикс усещане
                using (var pen = new Pen(Theme.WithAlpha(_bubbleTextColor, (int)(alpha * 0.55f)), 1.4f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Опашчица към Пешо (той е точно отдолу, близо до балончето)
            var tail = new[]
            {
                new Point(scaled.X + scaled.Width / 2 - 7, scaled.Bottom - 1),
                new Point(scaled.X + scaled.Width / 2 + 7, scaled.Bottom - 1),
                new Point(scaled.X + scaled.Width / 2,     scaled.Bottom + 8)
            };
            using (var tailBrush = new SolidBrush(Theme.WithAlpha(Theme.SurfaceLight, alpha)))
            {
                g.FillPolygon(tailBrush, tail);
            }

            using (var font = new Font("Segoe UI Semibold", 10.5f))
            {
                var textRect = new Rectangle(
                    scaled.X + 6, scaled.Y + 4, scaled.Width - 12, scaled.Height - 8);
                TextRenderer.DrawText(g, text, font, textRect,
                    Theme.WithAlpha(_bubbleTextColor, alpha),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.WordBreak);
            }
        }

        private static void DrawSprite(Graphics g, Image sprite, Rectangle area,
            float offsetX, float offsetY)
        {
            if (area.Width <= 0 || area.Height <= 0) return;

            float scale = Math.Min(
                (float)area.Width / sprite.Width,
                (float)area.Height / sprite.Height);

            float w = sprite.Width * scale;
            float h = sprite.Height * scale;

            g.DrawImage(sprite,
                area.X + (area.Width - w) / 2 + offsetX,
                area.Y + (area.Height - h) / 2 + offsetY,
                w, h);
        }

        /// <summary>Вграденото GDI+ рисуване на Пешо (без външни файлове).</summary>
        private void DrawPesho(Graphics g, Rectangle area, float dx, float dy,
            float headShake, float headNod, DateTime now)
        {
            if (area.Width <= 0 || area.Height <= 0) return;

            // Пешо запълва почти целия панел (уголемен с ~35%)
            float size = Math.Min(area.Width, area.Height) * 0.96f;
            float cx = area.X + area.Width / 2f + dx;
            float top = area.Y + (area.Height - size) / 2f + dy;

            float headR = size * 0.30f;              // радиус на главата
            float headCx = cx + headShake;           // клатене на глава при 20–50 лв.
            float headCy = top + headR + size * 0.10f + headNod; // + доволно кимане

            bool blinking = now < _blinkEndsAt;

            Color skin = Color.FromArgb(233, 150, 122);
            Color skinDark = Color.FromArgb(205, 120, 95);
            Color hat = Color.FromArgb(110, 82, 55);
            Color hatDark = Color.FromArgb(88, 64, 42);
            Color mustache = Color.FromArgb(120, 120, 125);
            Color vest = Color.FromArgb(96, 70, 46);
            Color shirt = Color.FromArgb(70, 78, 88);

            // --- Тяло (елек + риза) ---
            float bodyW = size * 0.72f;
            float bodyH = size * 0.42f;
            float bodyY = headCy + headR * 0.78f;

            using (GraphicsPath body = Theme.RoundedRect(
                       new Rectangle((int)(cx - bodyW / 2), (int)bodyY, (int)bodyW, (int)bodyH),
                       (int)(size * 0.09f)))
            {
                using (var shirtBrush = new SolidBrush(shirt))
                {
                    g.FillPath(shirtBrush, body);
                }
            }

            using (var vestBrush = new SolidBrush(vest))
            {
                g.FillRectangle(vestBrush, cx - bodyW / 2, bodyY, bodyW * 0.30f, bodyH);
                g.FillRectangle(vestBrush, cx + bodyW / 2 - bodyW * 0.30f, bodyY, bodyW * 0.30f, bodyH);
            }

            // --- Ръце според настроението ---
            float fistR = size * 0.075f;

            if (_mood == Mood.Warning)
            {
                // Кръстосани ръце – лента през гърдите + два юмрука
                using (var armBrush = new SolidBrush(Theme.Lighten(shirt, 12)))
                using (GraphicsPath arms = Theme.RoundedRect(
                           new Rectangle((int)(cx - bodyW * 0.42f), (int)(bodyY + bodyH * 0.30f),
                                         (int)(bodyW * 0.84f), (int)(bodyH * 0.30f)),
                           (int)(size * 0.05f)))
                {
                    g.FillPath(armBrush, arms);
                }
                using (var fist = new SolidBrush(skin))
                {
                    g.FillEllipse(fist, cx - bodyW * 0.40f, bodyY + bodyH * 0.30f, fistR * 2, fistR * 2);
                    g.FillEllipse(fist, cx + bodyW * 0.40f - fistR * 2, bodyY + bodyH * 0.30f, fistR * 2, fistR * 2);
                }
            }
            else if (_mood == Mood.Happy)
            {
                // Лява ръка – юмрук, дясна – вдигнат палец
                using (var fist = new SolidBrush(skin))
                {
                    g.FillEllipse(fist, cx - bodyW * 0.28f - fistR, bodyY + bodyH * 0.55f, fistR * 2, fistR * 2);

                    float thumbX = cx + bodyW * 0.34f;
                    float thumbY = bodyY + bodyH * 0.18f;
                    g.FillEllipse(fist, thumbX - fistR, thumbY, fistR * 2, fistR * 2);          // юмрук
                    g.FillEllipse(fist, thumbX - fistR * 0.35f, thumbY - fistR * 1.5f,
                        fistR * 0.9f, fistR * 1.7f);                                            // палецът нагоре
                }
            }
            else if (_mood == Mood.Angry)
            {
                // Лява ръка – юмрук, дясна – размахан показалец
                float wag = (float)Math.Sin(_gesturePhase) * size * 0.045f;
                using (var fist = new SolidBrush(skin))
                {
                    g.FillEllipse(fist, cx - bodyW * 0.28f - fistR, bodyY + bodyH * 0.55f, fistR * 2, fistR * 2);

                    float handX = cx + bodyW * 0.36f + wag;
                    float handY = bodyY + bodyH * 0.10f;
                    g.FillEllipse(fist, handX - fistR, handY, fistR * 2, fistR * 2);            // длан
                    g.FillEllipse(fist, handX - fistR * 0.3f + wag * 0.4f, handY - fistR * 1.7f,
                        fistR * 0.7f, fistR * 1.9f);                                            // показалец
                }
            }
            else
            {
                // Стандартно: два юмрука на бара
                using (var fist = new SolidBrush(skin))
                {
                    g.FillEllipse(fist, cx - bodyW * 0.28f - fistR, bodyY + bodyH * 0.55f, fistR * 2, fistR * 2);
                    g.FillEllipse(fist, cx + bodyW * 0.28f - fistR, bodyY + bodyH * 0.55f, fistR * 2, fistR * 2);
                }
            }

            // --- Глава ---
            using (var skinBrush = new SolidBrush(skin))
            {
                g.FillEllipse(skinBrush, headCx - headR, headCy - headR, headR * 2, headR * 2);
            }

            // Уши
            using (var earBrush = new SolidBrush(skinDark))
            {
                float earR = headR * 0.22f;
                g.FillEllipse(earBrush, headCx - headR - earR * 0.7f, headCy - earR / 2, earR * 1.4f, earR * 1.6f);
                g.FillEllipse(earBrush, headCx + headR - earR * 0.7f, headCy - earR / 2, earR * 1.4f, earR * 1.6f);
            }

            // --- Каскет ---
            using (var hatBrush = new SolidBrush(hat))
            {
                g.FillPie(hatBrush,
                    headCx - headR * 1.05f, headCy - headR * 1.35f,
                    headR * 2.1f, headR * 1.5f, 180, 180);
            }
            using (var brimBrush = new SolidBrush(hatDark))
            {
                g.FillEllipse(brimBrush,
                    headCx - headR * 1.08f, headCy - headR * 0.72f,
                    headR * 2.16f, headR * 0.26f);
            }

            // Символ при ядосан – "изпуска пара"
            if (_mood == Mood.Furious || _mood == Mood.Angry)
            {
                using (var angryFont = new Font("Segoe UI", size * 0.055f, FontStyle.Bold))
                {
                    TextRenderer.DrawText(g, "💢", angryFont,
                        new Rectangle((int)(headCx + headR * 0.45f), (int)(headCy - headR * 1.25f), 40, 26),
                        Theme.RedBright,
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.NoClipping);
                }
            }

            // --- Вежди (по настроение) ---
            float browY = headCy - headR * 0.28f;
            float browLen = headR * 0.42f;
            float browTilt;
            switch (_mood)
            {
                case Mood.Happy: browTilt = -headR * 0.06f; break;   // повдигнати
                case Mood.Wary: browTilt = headR * 0.06f; break;     // леко намръщен
                case Mood.Warning: browTilt = headR * 0.12f; break;
                case Mood.Angry:
                case Mood.Furious: browTilt = headR * 0.20f; break;  // смръщени
                default: browTilt = headR * 0.03f; break;
            }

            using (var browPen = new Pen(Color.FromArgb(90, 90, 95), Math.Max(2f, size * 0.024f))
                       { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                // Вдигната лява вежда (случайна реакция на всеки 10–15 сек)
                float browRaise = now < _browRaiseUntil ? headR * 0.14f : 0f;

                g.DrawLine(browPen,
                    headCx - headR * 0.55f, browY - browTilt * 0.2f - browRaise,
                    headCx - headR * 0.55f + browLen, browY + browTilt - browRaise * 0.6f);
                g.DrawLine(browPen,
                    headCx + headR * 0.55f, browY - browTilt * 0.2f,
                    headCx + headR * 0.55f - browLen, browY + browTilt);
            }

            // --- Очи: движещи се зеници + мигане/намигане ---
            float eyeY = headCy - headR * 0.08f;
            float eyeDx = headR * 0.38f;
            float eyeR = headR * 0.10f;
            float pupilShift = _pupilX * eyeR * 0.6f;

            DrawEye(g, headCx - eyeDx, eyeY, eyeR, pupilShift, skinDark,
                closed: blinking && !_winkOnly);
            DrawEye(g, headCx + eyeDx, eyeY, eyeR, pupilShift, skinDark,
                closed: blinking); // при намигане се затваря само дясното око

            // Нос
            using (var noseBrush = new SolidBrush(skinDark))
            {
                float noseR = headR * 0.16f;
                g.FillEllipse(noseBrush, headCx - noseR, headCy + headR * 0.05f, noseR * 2, noseR * 1.6f);
            }

            // --- Големият мустак (леко потрепва) ---
            float mWiggle = (float)Math.Sin(_mustachePhase) * headR * 0.03f;
            using (var mustacheBrush = new SolidBrush(mustache))
            {
                float mw = headR * 0.72f;
                float mh = headR * 0.34f;
                float my = headCy + headR * 0.30f;
                g.FillEllipse(mustacheBrush, headCx - mw, my - mWiggle, mw, mh);
                g.FillEllipse(mustacheBrush, headCx, my + mWiggle, mw, mh);
            }

            // --- Уста (под мустака, вижда се по края) ---
            float mouthY = headCy + headR * 0.68f;
            using (var mouthPen = new Pen(Color.FromArgb(140, 70, 60), Math.Max(2f, size * 0.02f))
                       { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                if (_mood == Mood.Happy)
                {
                    g.DrawArc(mouthPen,
                        headCx - headR * 0.30f, mouthY - headR * 0.18f,
                        headR * 0.6f, headR * 0.3f, 20, 140);       // широка усмивка
                }
                else if (_mood == Mood.Angry || _mood == Mood.Furious)
                {
                    g.DrawArc(mouthPen,
                        headCx - headR * 0.30f, mouthY,
                        headR * 0.6f, headR * 0.3f, 200, 140);      // намръщена
                }
                else if (_mood == Mood.Wary || _mood == Mood.Warning)
                {
                    g.DrawLine(mouthPen,
                        headCx - headR * 0.22f, mouthY + headR * 0.04f,
                        headCx + headR * 0.22f, mouthY - headR * 0.04f); // накривена
                }
                else
                {
                    g.DrawLine(mouthPen,
                        headCx - headR * 0.22f, mouthY,
                        headCx + headR * 0.22f, mouthY);
                }
            }

            // Зачервени бузи при гняв
            if (_mood == Mood.Angry || _mood == Mood.Furious)
            {
                using (var blush = new SolidBrush(Theme.WithAlpha(Theme.RedBright, 60)))
                {
                    float bl = headR * 0.24f;
                    g.FillEllipse(blush, headCx - headR * 0.78f, headCy + headR * 0.12f, bl, bl * 0.7f);
                    g.FillEllipse(blush, headCx + headR * 0.54f, headCy + headR * 0.12f, bl, bl * 0.7f);
                }
            }
        }

        /// <summary>
        /// Малка бирена халба долу вляво на бара, с лека „пара“/аромат,
        /// който се вие нагоре и избледнява. Чисто декоративна.
        /// </summary>
        private void DrawMugWithSteam(Graphics g, Rectangle area)
        {
            if (area.Width < 120 || area.Height < 120) return;

            float mugW = 26, mugH = 32;
            float mx = area.X + 14;
            float my = area.Bottom - mugH - 6;

            // Тяло на халбата (кехлибарена бира)
            using (var beer = new SolidBrush(Color.FromArgb(210, 235, 160, 40)))
            {
                g.FillRectangle(beer, mx, my + 6, mugW, mugH - 6);
            }
            // Пяна отгоре
            using (var foam = new SolidBrush(Color.FromArgb(235, 248, 244, 230)))
            {
                g.FillEllipse(foam, mx - 2, my, mugW + 4, 12);
            }
            // Дръжка
            using (var handlePen = new Pen(Color.FromArgb(200, 220, 220, 228), 3f))
            {
                g.DrawArc(handlePen, mx + mugW - 3, my + 10, 12, 16, 300, 120);
            }
            // Стъклен ръб
            using (var glassPen = new Pen(Color.FromArgb(120, 255, 255, 255), 1.4f))
            {
                g.DrawRectangle(glassPen, mx, my + 6, mugW, mugH - 6);
            }

            // Пара: две виещи се нишки, които се издигат и избледняват
            for (int wisp = 0; wisp < 2; wisp++)
            {
                float baseX = mx + 7 + wisp * 12;
                double phase = _steamPhase + wisp * 1.4;

                using (var path = new GraphicsPath())
                {
                    var pts = new PointF[5];
                    for (int i = 0; i < pts.Length; i++)
                    {
                        float rise = i * 9f;
                        pts[i] = new PointF(
                            baseX + (float)Math.Sin(phase + i * 0.9) * 4f,
                            my - 4 - rise);
                    }
                    path.AddCurve(pts);

                    // Колкото по-нагоре, толкова по-бледа е нишката
                    int alpha = 55 + (int)(20 * Math.Sin(phase * 0.8));
                    using (var steamPen = new Pen(Color.FromArgb(
                               Math.Max(20, alpha), 235, 235, 240), 2.2f)
                               { StartCap = LineCap.Round, EndCap = LineCap.Round })
                    {
                        g.DrawPath(steamPen, path);
                    }
                }
            }
        }

        /// <summary>Едно око: отворено (със зеница, гледаща настрани) или затворено.</summary>
        private static void DrawEye(Graphics g, float cx, float cy, float r,
            float pupilShift, Color lidColor, bool closed)
        {
            if (closed)
            {
                using (var lidPen = new Pen(lidColor, Math.Max(2f, r * 0.4f))
                           { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    g.DrawLine(lidPen, cx - r, cy, cx + r, cy);
                }
                return;
            }

            // Бяло на окото + движеща се зеница
            using (var white = new SolidBrush(Color.FromArgb(245, 245, 248)))
            {
                g.FillEllipse(white, cx - r * 1.15f, cy - r * 1.05f, r * 2.3f, r * 2.1f);
            }
            using (var pupil = new SolidBrush(Color.FromArgb(35, 38, 44)))
            {
                g.FillEllipse(pupil, cx - r * 0.55f + pupilShift, cy - r * 0.55f, r * 1.1f, r * 1.1f);
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

        // =====================================================================
        //  Незадължителни PNG спрайтове (кеширани, безопасни при липса)
        // =====================================================================

        private static class PeshoSprites
        {
            private static readonly System.Collections.Generic.Dictionary<string, Image> Cache =
                new System.Collections.Generic.Dictionary<string, Image>();

            public static Image Get(string moodName)
            {
                string key = moodName.ToLowerInvariant();

                if (Cache.TryGetValue(key, out Image cached))
                {
                    return cached; // може да е и null – значи вече знаем, че липсва
                }

                Image image = null;
                try
                {
                    string path = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Assets", "Pesho", "pesho_" + key + ".png");

                    if (File.Exists(path))
                    {
                        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            image = Image.FromStream(stream);
                        }
                    }
                }
                catch
                {
                    image = null; // повредена/липсваща картинка -> вграденото рисуване
                }

                Cache[key] = image;
                return image;
            }
        }
    }
}
