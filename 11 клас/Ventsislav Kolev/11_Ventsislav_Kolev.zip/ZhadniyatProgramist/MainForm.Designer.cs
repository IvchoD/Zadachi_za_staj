using System.Drawing;
using System.Windows.Forms;
using ZhadniyatProgramist.Controls;

namespace ZhadniyatProgramist
{
    partial class MainForm
    {
        /// <summary>Контейнер за компоненти (таймерите се добавят тук и се освобождават автоматично).</summary>
        private System.ComponentModel.IContainer components = null;

        #region Полета с контроли

        // Коренна решетка = анимиран фон (жив градиент + бирени мехурчета)
        private AnimatedBackgroundPanel tableRoot;

        // --- Статистически карти (горе вляво) ---
        private TableLayoutPanel tableStats;
        private ModernPanel cardStatProgrammers;
        private Label lblStatProgrammersTitle;
        private AnimatedNumberLabel lblStatProgrammersValue;
        private ModernPanel cardStatTabs;
        private Label lblStatTabsTitle;
        private AnimatedNumberLabel lblStatTabsValue;
        private ModernPanel cardStatDebt;
        private Label lblStatDebtTitle;
        private AnimatedNumberLabel lblStatDebtValue;
        private ModernPanel cardStatTop;
        private Label lblStatTopTitle;
        private Label lblStatTopName;
        private AnimatedNumberLabel lblStatTopSum;

        // --- Таблица с всички поръчки (JOIN) ---
        private ModernPanel panelGrid;
        private TableLayoutPanel tableGridLayout;
        private Label lblGridTitle;
        private NeonSignControl neonTitle;
        private TextBox txtSearch;
        private DataGridView gridOrders;

        // --- Дясна колона: избор на програмист + дълг + плащане ---
        private ModernPanel panelRight;
        private TableLayoutPanel tableRight;
        private TableLayoutPanel tableRightHeader;
        private Label lblChooseTitle;
        private NeonSignControl neonSign;
        private Label lblDayNight;
        private Label lblChooseHint;
        private AnimatedComboBox comboProgrammers;
        private ModernPanel panelDebt;
        private Label lblDebtCaption;
        private Label lblDebtAmount;
        private Label lblDebtStatus;
        private ModernPanel panelPesho;
        private PeshoCharacterControl peshoCharacter;
        private ModernButton btnPay;

        // --- Долна лента: формуляри за добавяне ---
        private TableLayoutPanel tableBottom;
        private ModernPanel panelAddOrder;
        private TableLayoutPanel tableAddOrder;
        private Label lblAddOrderTitle;
        private Label lblOrderProgrammer;
        private AnimatedComboBox comboOrderProgrammer;
        private Label lblOrderName;
        private TextBox txtOrderName;
        private Label lblOrderPrice;
        private TextBox txtOrderPrice;
        private ModernButton btnAddOrder;

        private ModernPanel panelAddProgrammer;
        private TableLayoutPanel tableAddProgrammer;
        private Label lblAddProgTitle;
        private Label lblProgName;
        private TextBox txtProgName;
        private Label lblProgLang;
        private TextBox txtProgLanguage;
        private ModernButton btnAddProgrammer;

        // --- Долен статус бар ---
        private TableLayoutPanel tableFooter;
        private Label lblConnection;
        private Label lblFooterCenter;
        private CheckBox chkSound;
        private Label lblClock;

        // --- Анимация с монети при плащане (наслагва се върху панела с дълга) ---
        private CoinBurstControl coinBurst;

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Изгражда целия интерфейс. Всичко е върху TableLayoutPanel-и
        /// с процентни колони и щедри отстояния (мин. 16 px между панелите),
        /// затова при resize нищо не се застъпва и не се реже.
        /// При прозорец, по-малък от вътрешния минимум, се появяват
        /// скролери, вместо да се крият текстове.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            SuspendLayout();

            // =================================================================
            //  Коренна решетка: 2 колони (основна / дясна), 4 реда.
            //  Самата решетка е и „живият фон“ на приложението.
            // =================================================================
            tableRoot = new AnimatedBackgroundPanel();
            tableRoot.Dock = DockStyle.Fill;
            tableRoot.BackColor = Theme.Background;
            tableRoot.Padding = new Padding(18, 16, 18, 8);
            tableRoot.MinimumSize = new Size(1460, 980); // под този размер -> скрол, не рязане
            tableRoot.ColumnCount = 2;
            tableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67f));
            tableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33f));
            tableRoot.RowCount = 4;
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 156f)); // статистика (по-високи карти)
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // таблица
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 300f)); // формуляри (по-високи)
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 56f));  // статус бар

            // =================================================================
            //  Статистически карти – еднаква височина, 16 px "въздух"
            // =================================================================
            tableStats = new TableLayoutPanel();
            tableStats.Dock = DockStyle.Fill;
            tableStats.BackColor = Color.Transparent;
            tableStats.Margin = new Padding(0);
            tableStats.ColumnCount = 4;
            tableStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tableStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tableStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tableStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tableStats.RowCount = 1;
            tableStats.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            // --- Карта: Общо програмисти ---
            cardStatProgrammers = new ModernPanel();
            cardStatProgrammers.Dock = DockStyle.Fill;
            cardStatProgrammers.Margin = new Padding(0, 0, 16, 16);
            cardStatProgrammers.MinimumSize = new Size(0, 90);
            cardStatProgrammers.Padding = new Padding(18, 18, 18, 14);
            cardStatProgrammers.BackColor = Theme.Surface;
            cardStatProgrammers.BorderColor = Color.FromArgb(0, 110, 66);
            cardStatProgrammers.Breathe = true; // картата леко „диша“

            lblStatProgrammersValue = new AnimatedNumberLabel();
            lblStatProgrammersValue.Dock = DockStyle.Fill;
            lblStatProgrammersValue.Font = new Font("Segoe UI Semibold", 20f);
            lblStatProgrammersValue.ForeColor = Theme.NeonGreen;
            lblStatProgrammersValue.TextAlign = ContentAlignment.MiddleLeft;
            lblStatProgrammersValue.Decimals = 0;
            lblStatProgrammersValue.Suffix = "";
            lblStatProgrammersValue.Text = "0";

            lblStatProgrammersTitle = new Label();
            lblStatProgrammersTitle.Dock = DockStyle.Top;
            lblStatProgrammersTitle.Height = 36;
            lblStatProgrammersTitle.Font = new Font("Segoe UI", 10.5f);
            lblStatProgrammersTitle.ForeColor = Theme.TextMuted;
            lblStatProgrammersTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblStatProgrammersTitle.Text = "👥  Общо програмисти";

            cardStatProgrammers.Controls.Add(lblStatProgrammersValue);
            cardStatProgrammers.Controls.Add(lblStatProgrammersTitle);

            // --- Карта: Активни сметки ---
            cardStatTabs = new ModernPanel();
            cardStatTabs.Dock = DockStyle.Fill;
            cardStatTabs.Margin = new Padding(0, 0, 16, 16);
            cardStatTabs.MinimumSize = new Size(0, 90);
            cardStatTabs.Padding = new Padding(18, 18, 18, 14);
            cardStatTabs.BackColor = Theme.Surface;
            cardStatTabs.BorderColor = Color.FromArgb(146, 104, 0);
            cardStatTabs.Breathe = true; // картата леко „диша“

            lblStatTabsValue = new AnimatedNumberLabel();
            lblStatTabsValue.Dock = DockStyle.Fill;
            lblStatTabsValue.Font = new Font("Segoe UI Semibold", 20f);
            lblStatTabsValue.ForeColor = Theme.Amber;
            lblStatTabsValue.TextAlign = ContentAlignment.MiddleLeft;
            lblStatTabsValue.Decimals = 0;
            lblStatTabsValue.Suffix = "";
            lblStatTabsValue.Text = "0";

            lblStatTabsTitle = new Label();
            lblStatTabsTitle.Dock = DockStyle.Top;
            lblStatTabsTitle.Height = 36;
            lblStatTabsTitle.Font = new Font("Segoe UI", 10.5f);
            lblStatTabsTitle.ForeColor = Theme.TextMuted;
            lblStatTabsTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblStatTabsTitle.Text = "📒  Активни сметки";

            cardStatTabs.Controls.Add(lblStatTabsValue);
            cardStatTabs.Controls.Add(lblStatTabsTitle);

            // --- Карта: Обща сума дългове ---
            cardStatDebt = new ModernPanel();
            cardStatDebt.Dock = DockStyle.Fill;
            cardStatDebt.Margin = new Padding(0, 0, 16, 16);
            cardStatDebt.MinimumSize = new Size(0, 90);
            cardStatDebt.Padding = new Padding(18, 18, 18, 14);
            cardStatDebt.BackColor = Theme.Surface;
            cardStatDebt.BorderColor = Color.FromArgb(140, 46, 46);
            cardStatDebt.Breathe = true; // картата леко „диша“

            lblStatDebtValue = new AnimatedNumberLabel();
            lblStatDebtValue.Dock = DockStyle.Fill;
            lblStatDebtValue.Font = new Font("Segoe UI Semibold", 20f);
            lblStatDebtValue.ForeColor = Theme.Red;
            lblStatDebtValue.TextAlign = ContentAlignment.MiddleLeft;
            lblStatDebtValue.Decimals = 2;
            lblStatDebtValue.Suffix = " €";
            lblStatDebtValue.Text = "0.00 €";

            lblStatDebtTitle = new Label();
            lblStatDebtTitle.Dock = DockStyle.Top;
            lblStatDebtTitle.Height = 44;
            lblStatDebtTitle.Font = new Font("Segoe UI", 10.5f);
            lblStatDebtTitle.ForeColor = Theme.TextMuted;
            lblStatDebtTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblStatDebtTitle.Text = "💰  Обща сума\r\nдългове";

            cardStatDebt.Controls.Add(lblStatDebtValue);
            cardStatDebt.Controls.Add(lblStatDebtTitle);

            // --- Карта: Най-затънал програмист (пулсира леко – важна е) ---
            cardStatTop = new ModernPanel();
            cardStatTop.Dock = DockStyle.Fill;
            cardStatTop.Margin = new Padding(0, 0, 0, 16);
            cardStatTop.MinimumSize = new Size(0, 90);
            cardStatTop.Padding = new Padding(18, 18, 18, 14);
            cardStatTop.BackColor = Theme.Surface;
            cardStatTop.BorderColor = Color.FromArgb(140, 46, 46);
            cardStatTop.EnablePulse = true;

            lblStatTopSum = new AnimatedNumberLabel();
            lblStatTopSum.AutoSize = false;
            lblStatTopSum.Dock = DockStyle.Fill;
            lblStatTopSum.Margin = new Padding(0);
            lblStatTopSum.Font = new Font("Segoe UI Semibold", 14f);
            lblStatTopSum.ForeColor = Theme.Red;
            lblStatTopSum.TextAlign = ContentAlignment.MiddleLeft;
            lblStatTopSum.Decimals = 2;
            lblStatTopSum.Suffix = " €";
            lblStatTopSum.Text = "0.00 €";

            lblStatTopName = new Label();
            lblStatTopName.Dock = DockStyle.Top;
            lblStatTopName.AutoSize = false;
            lblStatTopName.Height = 34;
            lblStatTopName.MaximumSize = new Size(0, 34);
            lblStatTopName.Margin = new Padding(0);
            lblStatTopName.Font = new Font("Segoe UI Semibold", 10.25f);
            lblStatTopName.ForeColor = Theme.TextPrimary;
            lblStatTopName.TextAlign = ContentAlignment.MiddleLeft;
            lblStatTopName.AutoEllipsis = true;
            lblStatTopName.Text = "—";

            lblStatTopTitle = new Label();
            lblStatTopTitle.Dock = DockStyle.Top;
            lblStatTopTitle.AutoSize = false;
            lblStatTopTitle.Height = 36;
            lblStatTopTitle.Margin = new Padding(0);
            lblStatTopTitle.Font = new Font("Segoe UI", 10.5f);
            lblStatTopTitle.ForeColor = Theme.TextMuted;
            lblStatTopTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblStatTopTitle.Text = "💀  Най-затънал";

            cardStatTop.Controls.Add(lblStatTopSum);
            cardStatTop.Controls.Add(lblStatTopName);
            cardStatTop.Controls.Add(lblStatTopTitle);

            tableStats.Controls.Add(cardStatProgrammers, 0, 0);
            tableStats.Controls.Add(cardStatTabs, 1, 0);
            tableStats.Controls.Add(cardStatDebt, 2, 0);
            tableStats.Controls.Add(cardStatTop, 3, 0);

            // =================================================================
            //  Панел с таблицата на поръчките
            // =================================================================
            panelGrid = new ModernPanel();
            panelGrid.Dock = DockStyle.Fill;
            panelGrid.Margin = new Padding(0, 0, 16, 16);
            panelGrid.Padding = new Padding(18);
            panelGrid.BackColor = Theme.Surface;
            panelGrid.BorderColor = Theme.Border;
            panelGrid.BorderRadius = 16;

            tableGridLayout = new TableLayoutPanel();
            tableGridLayout.Dock = DockStyle.Fill;
            tableGridLayout.BackColor = Theme.Surface;
            tableGridLayout.ColumnCount = 3;
            tableGridLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            tableGridLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 228f));
            tableGridLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 290f));
            tableGridLayout.RowCount = 2;
            tableGridLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));
            tableGridLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            lblGridTitle = new Label();
            lblGridTitle.Dock = DockStyle.Fill;
            lblGridTitle.Font = new Font("Segoe UI Semibold", 13.5f);
            lblGridTitle.ForeColor = Theme.NeonGreen;
            lblGridTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblGridTitle.Margin = new Padding(0);
            lblGridTitle.Text = "📒  Всички поръчки в тефтера";

            txtSearch = new TextBox();
            txtSearch.Dock = DockStyle.Fill;
            txtSearch.Margin = new Padding(0, 9, 0, 9);
            txtSearch.BackColor = Theme.SurfaceLight;
            txtSearch.ForeColor = Theme.TextPrimary;
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Font = new Font("Segoe UI", 11f);
            txtSearch.PlaceholderText = "🔍  Търси по име, поръчка или език…";

            // Голямата неонова табела на кръчмата – леко премигва,
            // от време на време една буква "изгасва" като истински неон
            neonTitle = new NeonSignControl();
            neonTitle.Dock = DockStyle.Fill;
            neonTitle.Margin = new Padding(0, 8, 12, 8);
            neonTitle.Text = "ЖАДНИЯТ ПРОГРАМИСТ";
            neonTitle.SignColor = Theme.NeonGreen;
            neonTitle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            gridOrders = new DataGridView();
            gridOrders.Dock = DockStyle.Fill;
            gridOrders.Margin = new Padding(0, 10, 0, 0);

            tableGridLayout.Controls.Add(lblGridTitle, 0, 0);
            tableGridLayout.Controls.Add(neonTitle, 1, 0);
            tableGridLayout.Controls.Add(txtSearch, 2, 0);
            tableGridLayout.Controls.Add(gridOrders, 0, 1);
            tableGridLayout.SetColumnSpan(gridOrders, 3);

            panelGrid.Controls.Add(tableGridLayout);

            // =================================================================
            //  Дясна колона: избор на програмист, дълг, Пешо, плащане
            // =================================================================
            panelRight = new ModernPanel();
            panelRight.Dock = DockStyle.Fill;
            panelRight.Margin = new Padding(0, 0, 0, 16);
            panelRight.Padding = new Padding(18, 20, 18, 18);
            panelRight.BackColor = Theme.Surface;
            panelRight.BorderColor = Theme.Border;
            panelRight.BorderRadius = 16;

            tableRight = new TableLayoutPanel();
            tableRight.Dock = DockStyle.Fill;
            tableRight.BackColor = Theme.Surface;
            tableRight.ColumnCount = 1;
            tableRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            tableRight.RowCount = 6;
            tableRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 56f));  // заглавие + табела + ден/нощ
            tableRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f));  // описание
            tableRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 64f));  // combo (по-висок)
            tableRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 256f)); // панел с дълга – фиксиран,
                                                                             // за да не се реже сумата при малък прозорец
            tableRight.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // Бай Пешо взима остатъка
            tableRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 70f));  // бутон

            // --- Заглавна лента: заглавие + „ОТВОРЕНО“ + индикатор ден/нощ ---
            tableRightHeader = new TableLayoutPanel();
            tableRightHeader.Dock = DockStyle.Fill;
            tableRightHeader.BackColor = Theme.Surface;
            tableRightHeader.Margin = new Padding(0);
            tableRightHeader.ColumnCount = 3;
            tableRightHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            tableRightHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126f));
            tableRightHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 74f));
            tableRightHeader.RowCount = 1;
            tableRightHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            lblChooseTitle = new Label();
            lblChooseTitle.Dock = DockStyle.Fill;
            lblChooseTitle.Font = new Font("Segoe UI Semibold", 13.5f);
            lblChooseTitle.ForeColor = Theme.TextPrimary;
            lblChooseTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblChooseTitle.Margin = new Padding(0);
            lblChooseTitle.Text = "Избери програмист";

            neonSign = new NeonSignControl();
            neonSign.Dock = DockStyle.Fill;
            neonSign.Margin = new Padding(0, 4, 8, 4);

            lblDayNight = new Label();
            lblDayNight.Dock = DockStyle.Fill;
            lblDayNight.Font = new Font("Segoe UI Semibold", 10f);
            lblDayNight.ForeColor = Theme.Amber;
            lblDayNight.TextAlign = ContentAlignment.MiddleRight;
            lblDayNight.Margin = new Padding(0);
            lblDayNight.Text = "☀ Ден";

            tableRightHeader.Controls.Add(lblChooseTitle, 0, 0);
            tableRightHeader.Controls.Add(neonSign, 1, 0);
            tableRightHeader.Controls.Add(lblDayNight, 2, 0);

            lblChooseHint = new Label();
            lblChooseHint.Dock = DockStyle.Fill;
            lblChooseHint.Font = new Font("Segoe UI", 10f);
            lblChooseHint.ForeColor = Theme.TextMuted;
            lblChooseHint.TextAlign = ContentAlignment.MiddleLeft;
            lblChooseHint.Margin = new Padding(0);
            lblChooseHint.Text = "Избери програмист, за да видиш колко е затънал.";

            comboProgrammers = new AnimatedComboBox();
            comboProgrammers.Dock = DockStyle.Fill;
            comboProgrammers.Margin = new Padding(0, 10, 0, 6);
            comboProgrammers.MinimumSize = new Size(0, 40);

            // --- Панел с общия дълг (най-важният панел – по-голям и пулсиращ) ---
            panelDebt = new ModernPanel();
            panelDebt.Dock = DockStyle.Fill;
            panelDebt.Margin = new Padding(0, 14, 0, 10);
            panelDebt.MinimumSize = new Size(0, 200);
            panelDebt.Padding = new Padding(18, 12, 18, 12);
            panelDebt.BackColor = Theme.Background;
            panelDebt.BorderColor = Theme.Border;
            panelDebt.BorderRadius = 16;
            panelDebt.EnablePulse = true;

            TableLayoutPanel tableDebtLayout = new TableLayoutPanel();
            tableDebtLayout.Dock = DockStyle.Fill;
            tableDebtLayout.BackColor = Theme.Background;
            tableDebtLayout.Margin = new Padding(0);
            tableDebtLayout.ColumnCount = 1;
            tableDebtLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            tableDebtLayout.RowCount = 3;
            tableDebtLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));
            tableDebtLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            tableDebtLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30f));

            lblDebtStatus = new Label();
            lblDebtStatus.Dock = DockStyle.Fill;
            lblDebtStatus.AutoSize = false;
            lblDebtStatus.Margin = new Padding(0);
            lblDebtStatus.Font = new Font("Segoe UI Semibold", 11f);
            lblDebtStatus.ForeColor = Theme.TextMuted;
            lblDebtStatus.TextAlign = ContentAlignment.MiddleCenter;
            lblDebtStatus.Text = "Избери програмист от менюто горе.";

            lblDebtAmount = new Label();
            lblDebtAmount.Dock = DockStyle.Fill;
            lblDebtAmount.AutoSize = false;
            lblDebtAmount.Margin = new Padding(0, 2, 0, 2);
            lblDebtAmount.Font = new Font("Segoe UI", 27f, FontStyle.Bold);
            lblDebtAmount.ForeColor = Theme.TextMuted;
            lblDebtAmount.TextAlign = ContentAlignment.MiddleCenter;
            lblDebtAmount.Text = "—";

            lblDebtCaption = new Label();
            lblDebtCaption.Dock = DockStyle.Fill;
            lblDebtCaption.AutoSize = false;
            lblDebtCaption.Height = 30;
            lblDebtCaption.Margin = new Padding(0);
            lblDebtCaption.Padding = new Padding(0);
            lblDebtCaption.Font = new Font("Segoe UI Semibold", 12.5f);
            lblDebtCaption.ForeColor = Theme.TextPrimary;
            lblDebtCaption.TextAlign = ContentAlignment.MiddleCenter;
            lblDebtCaption.Text = "Общ дълг:";

            tableDebtLayout.Controls.Add(lblDebtCaption, 0, 0);
            tableDebtLayout.Controls.Add(lblDebtAmount, 0, 1);
            tableDebtLayout.Controls.Add(lblDebtStatus, 0, 2);
            panelDebt.Controls.Add(tableDebtLayout);

            // --- Бай Пешо – в собствен панел с меко неоново пулсиране ---
            panelPesho = new ModernPanel();
            panelPesho.Dock = DockStyle.Fill;
            panelPesho.Margin = new Padding(0, 8, 0, 10);
            panelPesho.Padding = new Padding(10, 8, 10, 6);
            panelPesho.BackColor = Theme.Surface;
            panelPesho.BorderColor = Theme.Border;
            panelPesho.BorderRadius = 16;
            panelPesho.EnablePulse = true;

            peshoCharacter = new PeshoCharacterControl();
            peshoCharacter.Dock = DockStyle.Fill;
            peshoCharacter.Margin = new Padding(0);
            peshoCharacter.BackColor = Theme.Surface;

            panelPesho.Controls.Add(peshoCharacter);

            // --- Бутон „Плати сметката“ (пулсира – най-важното действие) ---
            btnPay = new ModernButton();
            btnPay.Dock = DockStyle.Fill;
            btnPay.Margin = new Padding(0, 4, 0, 4);
            btnPay.MinimumSize = new Size(0, 48);
            btnPay.BackColor = Color.FromArgb(183, 28, 28);
            btnPay.HoverColor = Color.FromArgb(229, 57, 53);
            btnPay.ForeColor = Color.White;
            btnPay.BorderRadius = 14;
            btnPay.Font = new Font("Segoe UI", 12.5f, FontStyle.Bold);
            btnPay.Pulse = true;
            btnPay.Text = "🍻  ПЛАТИ СМЕТКАТА";

            tableRight.Controls.Add(tableRightHeader, 0, 0);
            tableRight.Controls.Add(lblChooseHint, 0, 1);
            tableRight.Controls.Add(comboProgrammers, 0, 2);
            tableRight.Controls.Add(panelDebt, 0, 3);
            tableRight.Controls.Add(panelPesho, 0, 4);
            tableRight.Controls.Add(btnPay, 0, 5);

            panelRight.Controls.Add(tableRight);

            // =================================================================
            //  Долна лента: два формуляра (по-високи, с повече разстояния)
            // =================================================================
            tableBottom = new TableLayoutPanel();
            tableBottom.Dock = DockStyle.Fill;
            tableBottom.BackColor = Color.Transparent;
            tableBottom.Margin = new Padding(0);
            tableBottom.ColumnCount = 2;
            tableBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tableBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tableBottom.RowCount = 1;
            tableBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            // --- Формуляр: Добави нова поръчка ---
            panelAddOrder = new ModernPanel();
            panelAddOrder.Dock = DockStyle.Fill;
            panelAddOrder.Margin = new Padding(0, 0, 16, 0);
            panelAddOrder.MinimumSize = new Size(0, 210);
            panelAddOrder.Padding = new Padding(20, 18, 20, 18);
            panelAddOrder.BackColor = Theme.Surface;
            panelAddOrder.BorderColor = Color.FromArgb(0, 110, 66);
            panelAddOrder.BorderRadius = 16;

            tableAddOrder = new TableLayoutPanel();
            tableAddOrder.Dock = DockStyle.Fill;
            tableAddOrder.BackColor = Theme.Surface;
            tableAddOrder.ColumnCount = 2;
            tableAddOrder.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 172f));
            tableAddOrder.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            tableAddOrder.RowCount = 5;
            tableAddOrder.RowStyles.Add(new RowStyle(SizeType.Absolute, 42f)); // заглавие
            tableAddOrder.RowStyles.Add(new RowStyle(SizeType.Absolute, 54f)); // програмист
            tableAddOrder.RowStyles.Add(new RowStyle(SizeType.Absolute, 54f)); // поръчка
            tableAddOrder.RowStyles.Add(new RowStyle(SizeType.Absolute, 54f)); // цена
            tableAddOrder.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // бутон (остава ~60 px)

            lblAddOrderTitle = new Label();
            lblAddOrderTitle.Dock = DockStyle.Fill;
            lblAddOrderTitle.Font = new Font("Segoe UI Semibold", 12.5f);
            lblAddOrderTitle.ForeColor = Theme.NeonGreen;
            lblAddOrderTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblAddOrderTitle.Margin = new Padding(0);
            lblAddOrderTitle.Text = "🍺  Добави нова поръчка";

            lblOrderProgrammer = new Label();
            lblOrderProgrammer.Dock = DockStyle.Fill;
            lblOrderProgrammer.Font = new Font("Segoe UI", 10.5f);
            lblOrderProgrammer.ForeColor = Theme.TextMuted;
            lblOrderProgrammer.TextAlign = ContentAlignment.MiddleLeft;
            lblOrderProgrammer.Margin = new Padding(0, 0, 8, 0);
            lblOrderProgrammer.Text = "Програмист:";

            comboOrderProgrammer = new AnimatedComboBox();
            comboOrderProgrammer.Dock = DockStyle.Fill;
            comboOrderProgrammer.Margin = new Padding(0, 5, 0, 5);
            comboOrderProgrammer.MinimumSize = new Size(0, 40);

            lblOrderName = new Label();
            lblOrderName.Dock = DockStyle.Fill;
            lblOrderName.Font = new Font("Segoe UI", 10.5f);
            lblOrderName.ForeColor = Theme.TextMuted;
            lblOrderName.TextAlign = ContentAlignment.MiddleLeft;
            lblOrderName.Margin = new Padding(0, 0, 8, 0);
            lblOrderName.Text = "Поръчка:";

            txtOrderName = new TextBox();
            txtOrderName.Dock = DockStyle.Fill;
            txtOrderName.Margin = new Padding(0, 6, 0, 6);
            txtOrderName.AutoSize = false;
            txtOrderName.MinimumSize = new Size(0, 38);
            txtOrderName.BackColor = Theme.SurfaceLight;
            txtOrderName.ForeColor = Theme.TextPrimary;
            txtOrderName.BorderStyle = BorderStyle.FixedSingle;
            txtOrderName.Font = new Font("Segoe UI", 11.25f);
            txtOrderName.PlaceholderText = "Какво поръча?";

            lblOrderPrice = new Label();
            lblOrderPrice.Dock = DockStyle.Fill;
            lblOrderPrice.Font = new Font("Segoe UI", 10.5f);
            lblOrderPrice.ForeColor = Theme.TextMuted;
            lblOrderPrice.TextAlign = ContentAlignment.MiddleLeft;
            lblOrderPrice.Margin = new Padding(0, 0, 8, 0);
            lblOrderPrice.Text = "Цена (€):";

            txtOrderPrice = new TextBox();
            txtOrderPrice.Dock = DockStyle.Fill;
            txtOrderPrice.Margin = new Padding(0, 6, 0, 6);
            txtOrderPrice.AutoSize = false;
            txtOrderPrice.MinimumSize = new Size(0, 38);
            txtOrderPrice.BackColor = Theme.SurfaceLight;
            txtOrderPrice.ForeColor = Theme.TextPrimary;
            txtOrderPrice.BorderStyle = BorderStyle.FixedSingle;
            txtOrderPrice.Font = new Font("Segoe UI", 11.25f);
            txtOrderPrice.PlaceholderText = "0.00";

            btnAddOrder = new ModernButton();
            btnAddOrder.Dock = DockStyle.Fill;
            btnAddOrder.Margin = new Padding(0, 8, 0, 2);
            btnAddOrder.MinimumSize = new Size(0, 48);
            btnAddOrder.BackColor = Color.FromArgb(0, 135, 80);
            btnAddOrder.HoverColor = Color.FromArgb(0, 175, 102);
            btnAddOrder.ForeColor = Color.White;
            btnAddOrder.BorderRadius = 12;
            btnAddOrder.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            btnAddOrder.Text = "➕  Добави към тефтера";

            tableAddOrder.Controls.Add(lblAddOrderTitle, 0, 0);
            tableAddOrder.SetColumnSpan(lblAddOrderTitle, 2);
            tableAddOrder.Controls.Add(lblOrderProgrammer, 0, 1);
            tableAddOrder.Controls.Add(comboOrderProgrammer, 1, 1);
            tableAddOrder.Controls.Add(lblOrderName, 0, 2);
            tableAddOrder.Controls.Add(txtOrderName, 1, 2);
            tableAddOrder.Controls.Add(lblOrderPrice, 0, 3);
            tableAddOrder.Controls.Add(txtOrderPrice, 1, 3);
            tableAddOrder.Controls.Add(btnAddOrder, 0, 4);
            tableAddOrder.SetColumnSpan(btnAddOrder, 2);

            panelAddOrder.Controls.Add(tableAddOrder);

            // --- Формуляр: Добави нов програмист ---
            panelAddProgrammer = new ModernPanel();
            panelAddProgrammer.Dock = DockStyle.Fill;
            panelAddProgrammer.Margin = new Padding(0);
            panelAddProgrammer.MinimumSize = new Size(0, 210);
            panelAddProgrammer.Padding = new Padding(20, 18, 20, 18);
            panelAddProgrammer.BackColor = Theme.Surface;
            panelAddProgrammer.BorderColor = Color.FromArgb(146, 104, 0);
            panelAddProgrammer.BorderRadius = 16;

            tableAddProgrammer = new TableLayoutPanel();
            tableAddProgrammer.Dock = DockStyle.Fill;
            tableAddProgrammer.BackColor = Theme.Surface;
            tableAddProgrammer.ColumnCount = 2;
            tableAddProgrammer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 172f));
            tableAddProgrammer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            tableAddProgrammer.RowCount = 5;
            tableAddProgrammer.RowStyles.Add(new RowStyle(SizeType.Absolute, 42f)); // заглавие
            tableAddProgrammer.RowStyles.Add(new RowStyle(SizeType.Absolute, 54f)); // име
            tableAddProgrammer.RowStyles.Add(new RowStyle(SizeType.Absolute, 54f)); // език
            tableAddProgrammer.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // разстояние
            tableAddProgrammer.RowStyles.Add(new RowStyle(SizeType.Absolute, 64f)); // бутон

            lblAddProgTitle = new Label();
            lblAddProgTitle.Dock = DockStyle.Fill;
            lblAddProgTitle.Font = new Font("Segoe UI Semibold", 12.5f);
            lblAddProgTitle.ForeColor = Theme.Amber;
            lblAddProgTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblAddProgTitle.Margin = new Padding(0);
            lblAddProgTitle.Text = "👨‍💻  Добави нов програмист";

            lblProgName = new Label();
            lblProgName.Dock = DockStyle.Fill;
            lblProgName.Font = new Font("Segoe UI", 10.5f);
            lblProgName.ForeColor = Theme.TextMuted;
            lblProgName.TextAlign = ContentAlignment.MiddleLeft;
            lblProgName.Margin = new Padding(0, 0, 8, 0);
            lblProgName.Text = "Име:";

            txtProgName = new TextBox();
            txtProgName.Dock = DockStyle.Fill;
            txtProgName.Margin = new Padding(0, 6, 0, 6);
            txtProgName.AutoSize = false;
            txtProgName.MinimumSize = new Size(0, 38);
            txtProgName.BackColor = Theme.SurfaceLight;
            txtProgName.ForeColor = Theme.TextPrimary;
            txtProgName.BorderStyle = BorderStyle.FixedSingle;
            txtProgName.Font = new Font("Segoe UI", 11.25f);
            txtProgName.PlaceholderText = "Име на програмиста";

            lblProgLang = new Label();
            lblProgLang.Dock = DockStyle.Fill;
            lblProgLang.Font = new Font("Segoe UI", 10.5f);
            lblProgLang.ForeColor = Theme.TextMuted;
            lblProgLang.TextAlign = ContentAlignment.MiddleLeft;
            lblProgLang.Margin = new Padding(0, 0, 8, 0);
            lblProgLang.Text = "Любим език:";

            txtProgLanguage = new TextBox();
            txtProgLanguage.Dock = DockStyle.Fill;
            txtProgLanguage.Margin = new Padding(0, 6, 0, 6);
            txtProgLanguage.AutoSize = false;
            txtProgLanguage.MinimumSize = new Size(0, 38);
            txtProgLanguage.BackColor = Theme.SurfaceLight;
            txtProgLanguage.ForeColor = Theme.TextPrimary;
            txtProgLanguage.BorderStyle = BorderStyle.FixedSingle;
            txtProgLanguage.Font = new Font("Segoe UI", 11.25f);
            txtProgLanguage.PlaceholderText = "Любим език";

            btnAddProgrammer = new ModernButton();
            btnAddProgrammer.Dock = DockStyle.Fill;
            btnAddProgrammer.Margin = new Padding(0, 4, 0, 2);
            btnAddProgrammer.MinimumSize = new Size(0, 48);
            btnAddProgrammer.BackColor = Color.FromArgb(255, 171, 0);
            btnAddProgrammer.HoverColor = Color.FromArgb(255, 196, 60);
            btnAddProgrammer.ForeColor = Color.FromArgb(30, 22, 6);
            btnAddProgrammer.BorderRadius = 12;
            btnAddProgrammer.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
            btnAddProgrammer.Text = "👨‍💻  Добави програмист";

            tableAddProgrammer.Controls.Add(lblAddProgTitle, 0, 0);
            tableAddProgrammer.SetColumnSpan(lblAddProgTitle, 2);
            tableAddProgrammer.Controls.Add(lblProgName, 0, 1);
            tableAddProgrammer.Controls.Add(txtProgName, 1, 1);
            tableAddProgrammer.Controls.Add(lblProgLang, 0, 2);
            tableAddProgrammer.Controls.Add(txtProgLanguage, 1, 2);
            tableAddProgrammer.Controls.Add(btnAddProgrammer, 0, 4);
            tableAddProgrammer.SetColumnSpan(btnAddProgrammer, 2);

            panelAddProgrammer.Controls.Add(tableAddProgrammer);

            tableBottom.Controls.Add(panelAddOrder, 0, 0);
            tableBottom.Controls.Add(panelAddProgrammer, 1, 0);

            // =================================================================
            //  Долен статус бар (връзка / смяна / звук / часовник)
            // =================================================================
            tableFooter = new TableLayoutPanel();
            tableFooter.Dock = DockStyle.Fill;
            tableFooter.BackColor = Color.Transparent;
            tableFooter.Margin = new Padding(0, 8, 0, 0);
            tableFooter.ColumnCount = 4;
            tableFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28f));
            tableFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44f));
            tableFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170f));
            tableFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28f));
            tableFooter.RowCount = 1;
            tableFooter.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            lblConnection = new Label();
            lblConnection.Dock = DockStyle.Fill;
            lblConnection.Font = new Font("Segoe UI", 10f);
            lblConnection.ForeColor = Theme.NeonGreen;
            lblConnection.TextAlign = ContentAlignment.MiddleLeft;
            lblConnection.Margin = new Padding(0);
            lblConnection.Text = "●  Свързан с базата данни";

            lblFooterCenter = new Label();
            lblFooterCenter.Dock = DockStyle.Fill;
            lblFooterCenter.Font = new Font("Segoe UI", 10f);
            lblFooterCenter.ForeColor = Theme.TextMuted;
            lblFooterCenter.TextAlign = ContentAlignment.MiddleCenter;
            lblFooterCenter.Margin = new Padding(0);
            lblFooterCenter.Text = "Кръчма „Жадният Програмист“ © 2026";

            chkSound = new CheckBox();
            chkSound.Dock = DockStyle.Fill;
            chkSound.Font = new Font("Segoe UI", 10f);
            chkSound.ForeColor = Theme.TextMuted;
            chkSound.BackColor = Color.Transparent;
            chkSound.Margin = new Padding(0);
            chkSound.Checked = true; // звукът е включен по подразбиране
            chkSound.Text = "🔊 Звук: Вкл";
            chkSound.TextAlign = ContentAlignment.MiddleLeft;

            lblClock = new Label();
            lblClock.Dock = DockStyle.Fill;
            lblClock.Font = new Font("Segoe UI Semibold", 10f);
            lblClock.ForeColor = Theme.TextMuted;
            lblClock.TextAlign = ContentAlignment.MiddleRight;
            lblClock.Margin = new Padding(0);
            lblClock.Text = "00:00:00";

            tableFooter.Controls.Add(lblConnection, 0, 0);
            tableFooter.Controls.Add(lblFooterCenter, 1, 0);
            tableFooter.Controls.Add(chkSound, 2, 0);
            tableFooter.Controls.Add(lblClock, 3, 0);

            // =================================================================
            //  Сглобяване на коренната решетка
            // =================================================================
            tableRoot.Controls.Add(tableStats, 0, 0);
            tableRoot.Controls.Add(panelRight, 1, 0);
            tableRoot.SetRowSpan(panelRight, 2);
            tableRoot.Controls.Add(panelGrid, 0, 1);
            tableRoot.Controls.Add(tableBottom, 0, 2);
            tableRoot.SetColumnSpan(tableBottom, 2);
            tableRoot.Controls.Add(tableFooter, 0, 3);
            tableRoot.SetColumnSpan(tableFooter, 2);

            // --- Наслагваща се анимация с монети (скрита, показва се при плащане) ---
            coinBurst = new CoinBurstControl();
            coinBurst.Visible = false;

            // =================================================================
            //  Самата форма
            // =================================================================
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1500, 1000);
            MinimumSize = new Size(1500, 950);   // разумен минимум на прозореца
            AutoScroll = true;                    // под вътрешния минимум -> скрол
            BackColor = Theme.Background;
            ForeColor = Theme.TextPrimary;
            Font = new Font("Segoe UI", 10f);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "🍺 Кръчма „Жадният Програмист“ — тефтерите на бай Пешо";

            Controls.Add(coinBurst);
            Controls.Add(tableRoot);

            ResumeLayout(false);
        }
    }
}
