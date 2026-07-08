using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ZhadniyatProgramist.Controls;
using ZhadniyatProgramist.Models;
using ZhadniyatProgramist.Services;

namespace ZhadniyatProgramist
{
    /// <summary>
    /// Главният екран на кръчмата:
    ///  - таблица с всички поръчки (SQL JOIN, без голи ID-та);
    ///  - избор на програмист и анимирано изчисляване на дълга (SQL SUM);
    ///  - плащане на сметката (parameterized DELETE) с анимация от монети;
    ///  - добавяне на поръчки и програмисти с валидация;
    ///  - търсене, анимирана статистика, ден/нощ режим, звукови ефекти.
    /// Разположението на контролите е в MainForm.Designer.cs.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly System.Windows.Forms.Timer _fadeTimer;     // плавно появяване на прозореца
        private readonly System.Windows.Forms.Timer _debtTimer;     // анимация на сумата и цвета на дълга
        private readonly System.Windows.Forms.Timer _clockTimer;    // часовник в долната лента
        private readonly System.Windows.Forms.Timer _dayNightTimer; // ден/нощ режим (проверка на всяка минута)

        private double _displayedDebt;                 // сумата, показвана в момента
        private double _targetDebt;                    // реалната сума от базата
        private Color _debtColorCurrent;               // текущ цвят на сумата
        private Color _debtColorTarget;                // целеви цвят според статуса

        private const string AppTitle = "Тефтерът на бай Пешо";

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Opacity = 0; // прозорецът тръгва невидим и се появява плавно

            _debtColorCurrent = Theme.TextMuted;
            _debtColorTarget = Theme.TextMuted;

            _fadeTimer = new System.Windows.Forms.Timer(components) { Interval = 20 };
            _fadeTimer.Tick += FadeTimer_Tick;

            _debtTimer = new System.Windows.Forms.Timer(components) { Interval = 15 };
            _debtTimer.Tick += DebtTimer_Tick;

            _clockTimer = new System.Windows.Forms.Timer(components) { Interval = 1000 };
            _clockTimer.Tick += (s, e) => lblClock.Text = DateTime.Now.ToString("HH:mm:ss");

            // Ден/нощ режимът се обновява веднъж в минута – никакво натоварване.
            _dayNightTimer = new System.Windows.Forms.Timer(components) { Interval = 60000 };
            _dayNightTimer.Tick += (s, e) => UpdateDayNight();

            // Свързване на събитията с обработчиците
            Load += MainForm_Load;
            comboProgrammers.SelectedIndexChanged += (s, e) => UpdateDebtForSelection();
            comboProgrammers.SelectionChangeCommitted += (s, e) => SoundService.PlaySelect();
            btnPay.Click += BtnPay_Click;
            btnAddOrder.Click += BtnAddOrder_Click;
            btnAddProgrammer.Click += BtnAddProgrammer_Click;
            txtSearch.TextChanged += (s, e) => RefreshGrid();
            txtOrderPrice.KeyPress += TxtOrderPrice_KeyPress;
            chkSound.CheckedChanged += ChkSound_CheckedChanged;

            StyleGrid();
        }

        // =====================================================================
        //  Инициализация и стилове
        // =====================================================================

        private void MainForm_Load(object sender, EventArgs e)
        {
            EnableDarkTitleBar();

            LoadProgrammers();
            RefreshGrid();
            RefreshStats();
            UpdateDebtForSelection(); // показва подканата „Избери програмист…“
            UpdateDayNight();         // начално състояние на ден/нощ режима

            lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
            _clockTimer.Start();
            _dayNightTimer.Start();
            _fadeTimer.Start();

            // Лека кръчмарска атмосфера (само ако има Assets/Sounds/ambience.wav)
            SoundService.StartAmbience();
        }

        /// <summary>Тъмна заглавна лента под Windows 10/11 (чисто козметично).</summary>
        private void EnableDarkTitleBar()
        {
            try
            {
                int useDarkMode = 1;
                DwmSetWindowAttribute(Handle, 20, ref useDarkMode, sizeof(int));
            }
            catch
            {
                // По-старите версии на Windows не поддържат атрибута – игнорираме.
            }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(
            IntPtr hwnd, int attribute, ref int value, int size);

        /// <summary>Пълно стилизиране на DataGridView в тъмната тема.</summary>
        private void StyleGrid()
        {
            gridOrders.BorderStyle = BorderStyle.None;
            gridOrders.BackgroundColor = Theme.Surface;
            gridOrders.EnableHeadersVisualStyles = false;
            gridOrders.ReadOnly = true;
            gridOrders.AllowUserToAddRows = false;
            gridOrders.AllowUserToDeleteRows = false;
            gridOrders.AllowUserToResizeRows = false;
            gridOrders.RowHeadersVisible = false;
            gridOrders.MultiSelect = false;
            gridOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridOrders.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridOrders.GridColor = Theme.Border;

            // Цветен header – малко по-висок и с повече padding, за да се чете лесно
            gridOrders.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            gridOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridOrders.ColumnHeadersHeight = 46;
            gridOrders.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 66, 40);
            gridOrders.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 66, 40);
            gridOrders.ColumnHeadersDefaultCellStyle.ForeColor = Theme.TextPrimary;
            gridOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10.5f);
            gridOrders.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gridOrders.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);

            // Редове + алтернативен цвят – по-високи редове, по-щедър padding
            gridOrders.DefaultCellStyle.BackColor = Theme.Surface;
            gridOrders.DefaultCellStyle.ForeColor = Theme.TextPrimary;
            gridOrders.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 96, 57);
            gridOrders.DefaultCellStyle.SelectionForeColor = Color.White;
            gridOrders.DefaultCellStyle.Font = new Font("Segoe UI", 10.25f);
            gridOrders.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            gridOrders.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(27, 32, 39);
            gridOrders.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 96, 57);
            gridOrders.RowTemplate.Height = 38;

            // Hover ефект по редовете – редът под мишката леко светва
            gridOrders.CellMouseEnter += GridOrders_CellMouseEnter;
            gridOrders.CellMouseLeave += GridOrders_CellMouseLeave;

            // Двойно буфериране срещу трептене при скрол (вътрешно свойство)
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, gridOrders, new object[] { true });
        }

        /// <summary>Редът под мишката леко светва (модерен hover ефект).</summary>
        private void GridOrders_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= gridOrders.Rows.Count) return;
            gridOrders.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(38, 46, 56);
        }

        /// <summary>Връщаме оригиналния цвят на реда (вкл. алтернативния).</summary>
        private void GridOrders_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= gridOrders.Rows.Count) return;
            gridOrders.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Empty;
        }

        // =====================================================================
        //  Ден/нощ режим
        // =====================================================================

        /// <summary>
        /// Автоматичен визуален режим според часа:
        ///  07:00–18:00 – по-светъл фон, по-меки неонови ефекти, „Дневна смяна“;
        ///  иначе       – по-тъмен фон, неонът свети по-силно, „Нощна смяна“.
        /// </summary>
        private void UpdateDayNight()
        {
            int hour = DateTime.Now.Hour;
            bool isDay = hour >= 7 && hour < 18;

            Theme.IsNight = !isDay;      // неоновите контроли се съобразяват сами
            tableRoot.DayMode = isDay;   // фонът става по-светъл/по-тъмен

            if (isDay)
            {
                lblDayNight.Text = "☀ Ден";
                lblDayNight.ForeColor = Theme.Amber;
                lblFooterCenter.Text =
                    "Кръчма „Жадният Програмист“ © 2026 — Дневна смяна при бай Пешо";
            }
            else
            {
                lblDayNight.Text = "🌙 Нощ";
                lblDayNight.ForeColor = Color.FromArgb(150, 178, 255);
                lblFooterCenter.Text =
                    "Кръчма „Жадният Програмист“ © 2026 — Нощна смяна — тефтерите се пълнят";
            }
        }

        /// <summary>Превключвател на звука в статус бара.</summary>
        private void ChkSound_CheckedChanged(object sender, EventArgs e)
        {
            SoundService.Enabled = chkSound.Checked;
            chkSound.Text = chkSound.Checked ? "🔊 Звук: Вкл" : "🔇 Звук: Изкл";

            if (chkSound.Checked)
            {
                SoundService.StartAmbience();
            }
            else
            {
                SoundService.StopAmbience();
            }
        }

        // =====================================================================
        //  Зареждане на данни
        // =====================================================================

        /// <summary>Зарежда програмистите в двете падащи менюта, като пази избора.</summary>
        private void LoadProgrammers()
        {
            Safe(() =>
            {
                List<Programmer> programmers = DatabaseHelper.GetProgrammers();
                FillCombo(comboProgrammers, programmers);
                FillCombo(comboOrderProgrammer, programmers);
            });
        }

        private static void FillCombo(ComboBox combo, List<Programmer> programmers)
        {
            int selectedId = (combo.SelectedItem as Programmer)?.Id ?? -1;

            combo.BeginUpdate();
            combo.Items.Clear();
            foreach (Programmer programmer in programmers)
            {
                combo.Items.Add(programmer);
            }
            combo.EndUpdate();

            // Възстановяваме предишния избор по Id (ако още съществува)
            if (selectedId != -1)
            {
                for (int i = 0; i < combo.Items.Count; i++)
                {
                    if (combo.Items[i] is Programmer p && p.Id == selectedId)
                    {
                        combo.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        /// <summary>Презарежда таблицата (с текущия текст за търсене).</summary>
        private void RefreshGrid()
        {
            Safe(() =>
            {
                gridOrders.DataSource = DatabaseHelper.GetOrders(txtSearch.Text.Trim());
                ConfigureGridColumns();
            });
        }

        /// <summary>Формат и ширини на колоните след всяко зареждане.</summary>
        private void ConfigureGridColumns()
        {
            if (gridOrders.Columns.Count == 0) return;

            if (gridOrders.Columns.Contains("Програмист"))
                gridOrders.Columns["Програмист"].FillWeight = 100;

            if (gridOrders.Columns.Contains("Любим език"))
                gridOrders.Columns["Любим език"].FillWeight = 65;

            if (gridOrders.Columns.Contains("Поръчка"))
                gridOrders.Columns["Поръчка"].FillWeight = 110;

            if (gridOrders.Columns.Contains("Цена"))
            {
                DataGridViewColumn priceColumn = gridOrders.Columns["Цена"];
                priceColumn.FillWeight = 55;
                priceColumn.DefaultCellStyle.Format = "0.00 '€'"; // 12.50 €
                priceColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                priceColumn.DefaultCellStyle.Padding = new Padding(0, 0, 14, 0);
            }

            if (gridOrders.Columns.Contains("Дата"))
            {
                DataGridViewColumn dateColumn = gridOrders.Columns["Дата"];
                dateColumn.FillWeight = 80;
                dateColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        /// <summary>
        /// Обновява информационните карти. Числата НЕ се сменят рязко –
        /// AnimatedNumberLabel ги брои плавно до новата стойност.
        /// </summary>
        private void RefreshStats()
        {
            Safe(() =>
            {
                var stats = DatabaseHelper.GetStatistics();

                lblStatProgrammersValue.SetValue(stats.ProgrammersCount);
                lblStatTabsValue.SetValue(stats.ActiveTabsCount);
                lblStatDebtValue.SetValue(stats.TotalDebt);

                if (stats.TopDebtorName == null)
                {
                    lblStatTopName.Text = "Няма длъжници";
                    lblStatTopSum.SetValue(0);
                }
                else
                {
                    lblStatTopName.Text = stats.TopDebtorName;
                    lblStatTopSum.SetValue(stats.TopDebtorSum);
                }

                lblConnection.Text = "●  Свързан с базата данни";
                lblConnection.ForeColor = Theme.NeonGreen;
            });
        }

        // =====================================================================
        //  Дълг – изчисляване и анимации
        // =====================================================================

        /// <summary>
        /// Извиква се при смяна на избрания програмист.
        /// Взима дълга от базата (SUM) и стартира анимацията на панела.
        /// </summary>
        private void UpdateDebtForSelection()
        {
            if (comboProgrammers.SelectedItem is not Programmer programmer)
            {
                // Начално състояние – нищо не е избрано
                lblDebtAmount.Text = "—";
                lblDebtAmount.ForeColor = Theme.TextMuted;
                lblDebtStatus.Text = "Избери програмист от менюто горе.";
                lblDebtStatus.ForeColor = Theme.TextMuted;
                panelDebt.BorderColor = Theme.Border;
                peshoCharacter.ShowIdle(); // Пешо чака спокойно
                return;
            }

            Safe(() =>
            {
                _targetDebt = DatabaseHelper.GetTotalDebt(programmer.Id);

                var (statusColor, statusText) = GetDebtStatus(_targetDebt);
                _debtColorTarget = statusColor;
                lblDebtStatus.Text = statusText;
                lblDebtStatus.ForeColor = statusColor;
                panelDebt.BorderColor = statusColor; // рамката на панела също сменя цвета си

                _debtTimer.Start(); // сумата „тича“ плавно към новата стойност

                // 1) сумата е обновена, 2) цветът е обновен, 3) ред е на Пешо:
                peshoCharacter.UpdateMood((decimal)_targetDebt);

                // Дълг над 100 €: панелът се разклаща, рамката пулсира
                // в червено, Пешо трепери по-силно (в UpdateMood).
                if (_targetDebt > 100)
                {
                    panelDebt.Shake();
                    panelDebt.FlashAlert();
                    SoundService.PlayBigDebt();
                }
            });
        }

        /// <summary>Цвят и съобщение според размера на дълга (по изискванията на бай Пешо).</summary>
        private static (Color Color, string Status) GetDebtStatus(double debt)
        {
            if (debt <= 0)
                return (Theme.NeonGreen, "Чист е! Засега…");

            if (debt < 20)
                return (Theme.Amber, "Дългът е още в приличните граници.");

            if (debt <= 50)
                return (Theme.Red, "Внимание! Дългът расте застрашително!");

            return (Theme.RedBright, "Бай Пешо вече гледа лошо!");
        }

        /// <summary>Анимация: сумата и цветът плавно приближават целевите стойности.</summary>
        private void DebtTimer_Tick(object sender, EventArgs e)
        {
            _displayedDebt += (_targetDebt - _displayedDebt) * 0.18;
            _debtColorCurrent = Theme.Lerp(_debtColorCurrent, _debtColorTarget, 0.18f);

            bool amountReady = Math.Abs(_targetDebt - _displayedDebt) < 0.01;
            bool colorReady = ColorsAreClose(_debtColorCurrent, _debtColorTarget);

            if (amountReady && colorReady)
            {
                _displayedDebt = _targetDebt;
                _debtColorCurrent = _debtColorTarget;
                _debtTimer.Stop();
            }

            lblDebtAmount.Text = string.Format("{0:0.00} €", _displayedDebt);
            lblDebtAmount.ForeColor = _debtColorCurrent;
        }

        private static bool ColorsAreClose(Color a, Color b)
        {
            return Math.Abs(a.R - b.R) < 4 &&
                   Math.Abs(a.G - b.G) < 4 &&
                   Math.Abs(a.B - b.B) < 4;
        }

        /// <summary>Плавно появяване на прозореца при стартиране.</summary>
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            Opacity += 0.06;
            if (Opacity >= 1)
            {
                Opacity = 1;
                _fadeTimer.Stop();
            }
        }

        // =====================================================================
        //  Действия на потребителя
        // =====================================================================

        /// <summary>Бутон „Плати сметката“ – изтрива поръчките на избрания програмист.</summary>
        private void BtnPay_Click(object sender, EventArgs e)
        {
            if (comboProgrammers.SelectedItem is not Programmer programmer)
            {
                SoundService.PlayError();
                MessageBox.Show("Първо избери програмист.", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Safe(() =>
            {
                double debt = DatabaseHelper.GetTotalDebt(programmer.Id);

                if (debt <= 0)
                {
                    MessageBox.Show("Този човек вече е чист. Не го тормози.", AppTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogResult confirmation = MessageBox.Show(
                    string.Format("{0} дължи {1:0.00} €\n\nПлаща ли цялата сметка?",
                        programmer.Name, debt),
                    "Потвърждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmation != DialogResult.Yes) return;

                DatabaseHelper.PayTab(programmer.Id); // parameterized DELETE

                RefreshGrid();
                RefreshStats();
                UpdateDebtForSelection(); // дългът се анимира надолу към 0

                // Бай Пешо става щастлив: „Ей така те искам!“ + подскачане
                peshoCharacter.Celebrate();

                // Монети + „ПЛАТЕНО“ върху панела с дълга + звук
                coinBurst.Play(RectangleToClient(panelDebt.RectangleToScreen(panelDebt.ClientRectangle)));
                SoundService.PlayPayment();

                MessageBox.Show("Сметката е платена. Бай Пешо временно е спокоен.", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        /// <summary>Бутон „Добави към тефтера“ – нова поръчка с пълна валидация.</summary>
        private void BtnAddOrder_Click(object sender, EventArgs e)
        {
            // 1. Трябва да има избран програмист
            if (comboOrderProgrammer.SelectedItem is not Programmer programmer)
            {
                SoundService.PlayError();
                MessageBox.Show("Първо избери програмист.", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Името на поръчката не може да е празно
            string itemName = txtOrderName.Text.Trim();
            if (itemName.Length == 0)
            {
                SoundService.PlayError();
                MessageBox.Show("Въведи какво е поръчал човекът.", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOrderName.Focus();
                return;
            }

            // 3. Цената трябва да е валидно положително число (приема и запетая, и точка)
            string priceText = txtOrderPrice.Text.Trim().Replace(',', '.');
            if (!double.TryParse(priceText, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out double price) || price <= 0)
            {
                SoundService.PlayError();
                MessageBox.Show("Цената трябва да е положително число.", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOrderPrice.Focus();
                return;
            }

            Safe(() =>
            {
                DatabaseHelper.AddTab(new TabItem
                {
                    ProgrammerId = programmer.Id,
                    ItemName = itemName,
                    Price = Math.Round(price, 2),
                    Date = DateTime.Now
                });

                txtOrderName.Clear();
                txtOrderPrice.Clear();

                RefreshGrid();
                RefreshStats();
                UpdateDebtForSelection(); // ако е избран същият човек, дългът скача нагоре

                SoundService.PlayOrderAdded();

                // Малка анимация: халба „пада“ в тефтера (върху центъра на таблицата)
                Rectangle gridRect = RectangleToClient(panelGrid.RectangleToScreen(panelGrid.ClientRectangle));
                var dropRect = new Rectangle(
                    gridRect.X + gridRect.Width / 2 - 130,
                    gridRect.Y + Math.Max(8, gridRect.Height / 2 - 130),
                    260, 240);
                coinBurst.PlayOrderDrop(dropRect);

                // Бай Пешо реагира на новата сума на човека, който поръча;
                // над 50 € – „почукване по бара“ и „Първо плащаш, после поръчваш!“
                decimal newDebt = (decimal)DatabaseHelper.GetTotalDebt(programmer.Id);
                peshoCharacter.ReactToNewOrder(newDebt);

                MessageBox.Show("Поръчката е записана в тефтера! 🍺", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        /// <summary>Бутон „Добави програмист“ – нов клиент на кръчмата.</summary>
        private void BtnAddProgrammer_Click(object sender, EventArgs e)
        {
            string name = txtProgName.Text.Trim();
            if (name.Length == 0)
            {
                SoundService.PlayError();
                MessageBox.Show("Името не може да е празно.", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProgName.Focus();
                return;
            }

            string language = txtProgLanguage.Text.Trim();
            if (language.Length == 0)
            {
                SoundService.PlayError();
                MessageBox.Show("Любимият език не може да е празен.", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProgLanguage.Focus();
                return;
            }

            Safe(() =>
            {
                DatabaseHelper.AddProgrammer(new Programmer
                {
                    Name = name,
                    FavoriteLanguage = language
                });

                txtProgName.Clear();
                txtProgLanguage.Clear();

                LoadProgrammers();
                RefreshStats();

                SoundService.PlayOrderAdded();

                MessageBox.Show("Програмистът е добавен. Да му е сладка бирата!", AppTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        /// <summary>Позволява само цифри и десетичен разделител в полето за цена.</summary>
        private void TxtOrderPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool isDigit = char.IsDigit(e.KeyChar);
            bool isSeparator = e.KeyChar == '.' || e.KeyChar == ',';
            bool isControlKey = char.IsControl(e.KeyChar); // Backspace и др.

            if (!isDigit && !isSeparator && !isControlKey)
            {
                e.Handled = true;
            }
        }

        // =====================================================================
        //  Помощни методи
        // =====================================================================

        /// <summary>
        /// Изпълнява операция с базата, като хваща всички грешки
        /// и ги показва на потребителя, вместо приложението да крашва.
        /// </summary>
        private void Safe(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                lblConnection.Text = "●  Проблем с базата данни";
                lblConnection.ForeColor = Theme.Red;

                SoundService.PlayError();
                MessageBox.Show("Възникна грешка:\n" + ex.Message, "Грешка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
