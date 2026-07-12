namespace _10_raya_kirilova_
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            lbl_title = new Label();
            lbl_herbs = new Label();
            lbl_име = new Label();
            lbl_оплакване = new Label();
            lbl_profil = new Label();
            lbl_избрани = new Label();
            lbl_информация = new Label();
            chb_лайка = new CheckBox();
            chb_Мащерка = new CheckBox();
            chb_Змийско = new CheckBox();
            chb_Магарешки = new CheckBox();
            txt_Име = new TextBox();
            cmb_оплаквания = new ComboBox();
            lbl_лайка = new Label();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            pictureBox1 = new PictureBox();
            btn_отвара = new Button();
            lbl_билка = new Label();
            lbl_Семейство = new Label();
            lbl_Действие = new Label();
            lbl_Част = new Label();
            lbl_Препоръка = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // lbl_title
            // 
            lbl_title.AutoSize = true;
            lbl_title.BackColor = Color.Transparent;
            lbl_title.Font = new Font("Georgia", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_title.ForeColor = Color.Linen;
            lbl_title.Location = new Point(511, 44);
            lbl_title.Name = "lbl_title";
            lbl_title.Size = new Size(325, 54);
            lbl_title.TabIndex = 0;
            lbl_title.Text = "Дигиталният Хербариий\r\n            на Знахаря";
            lbl_title.Click += label1_Click;
            // 
            // lbl_herbs
            // 
            lbl_herbs.AutoSize = true;
            lbl_herbs.BackColor = Color.Transparent;
            lbl_herbs.Font = new Font("Georgia", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            lbl_herbs.ForeColor = Color.Black;
            lbl_herbs.Location = new Point(102, 148);
            lbl_herbs.Name = "lbl_herbs";
            lbl_herbs.Size = new Size(183, 24);
            lbl_herbs.TabIndex = 2;
            lbl_herbs.Text = "Налични Билки";
            // 
            // lbl_име
            // 
            lbl_име.AutoSize = true;
            lbl_име.BackColor = Color.Transparent;
            lbl_име.Font = new Font("Georgia", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_име.ForeColor = Color.FromArgb(64, 64, 0);
            lbl_име.Location = new Point(918, 207);
            lbl_име.Name = "lbl_име";
            lbl_име.Size = new Size(150, 18);
            lbl_име.TabIndex = 7;
            lbl_име.Text = "Име на пациента:";
            // 
            // lbl_оплакване
            // 
            lbl_оплакване.AutoSize = true;
            lbl_оплакване.BackColor = Color.Transparent;
            lbl_оплакване.Font = new Font("Georgia", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lbl_оплакване.ForeColor = Color.FromArgb(64, 64, 0);
            lbl_оплакване.Location = new Point(918, 279);
            lbl_оплакване.Name = "lbl_оплакване";
            lbl_оплакване.Size = new Size(100, 18);
            lbl_оплакване.TabIndex = 8;
            lbl_оплакване.Text = "Оплакване:";
            // 
            // lbl_profil
            // 
            lbl_profil.AutoSize = true;
            lbl_profil.BackColor = Color.Transparent;
            lbl_profil.Font = new Font("Georgia", 10.2F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            lbl_profil.ForeColor = SystemColors.MenuBar;
            lbl_profil.Location = new Point(948, 148);
            lbl_profil.Name = "lbl_profil";
            lbl_profil.Size = new Size(206, 20);
            lbl_profil.TabIndex = 9;
            lbl_profil.Text = "Пациентски профил";
            // 
            // lbl_избрани
            // 
            lbl_избрани.AutoSize = true;
            lbl_избрани.BackColor = Color.Transparent;
            lbl_избрани.Font = new Font("Georgia", 10.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            lbl_избрани.ForeColor = SystemColors.MenuBar;
            lbl_избрани.Location = new Point(997, 359);
            lbl_избрани.Name = "lbl_избрани";
            lbl_избрани.Size = new Size(119, 21);
            lbl_избрани.TabIndex = 10;
            lbl_избрани.Text = "Препоръка";
            // 
            // lbl_информация
            // 
            lbl_информация.AutoSize = true;
            lbl_информация.BackColor = Color.Transparent;
            lbl_информация.Font = new Font("Georgia", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            lbl_информация.ForeColor = SystemColors.MenuBar;
            lbl_информация.Location = new Point(491, 150);
            lbl_информация.Name = "lbl_информация";
            lbl_информация.Size = new Size(219, 18);
            lbl_информация.TabIndex = 11;
            lbl_информация.Text = "Информация за билката";
            // 
            // chb_лайка
            // 
            chb_лайка.AutoSize = true;
            chb_лайка.BackColor = Color.Transparent;
            chb_лайка.FlatStyle = FlatStyle.Flat;
            chb_лайка.Font = new Font("Georgia", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            chb_лайка.ForeColor = Color.DarkSeaGreen;
            chb_лайка.Location = new Point(53, 209);
            chb_лайка.Name = "chb_лайка";
            chb_лайка.Size = new Size(87, 24);
            chb_лайка.TabIndex = 12;
            chb_лайка.Text = "Лайка";
            chb_лайка.UseVisualStyleBackColor = false;
            chb_лайка.CheckedChanged += chb_лайка_CheckedChanged;
            // 
            // chb_Мащерка
            // 
            chb_Мащерка.AutoSize = true;
            chb_Мащерка.BackColor = Color.Transparent;
            chb_Мащерка.FlatStyle = FlatStyle.Flat;
            chb_Мащерка.Font = new Font("Georgia", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            chb_Мащерка.ForeColor = Color.DarkSeaGreen;
            chb_Мащерка.Location = new Point(53, 322);
            chb_Мащерка.Name = "chb_Мащерка";
            chb_Мащерка.Size = new Size(116, 24);
            chb_Мащерка.TabIndex = 13;
            chb_Мащерка.Text = "Мащерка";
            chb_Мащерка.UseVisualStyleBackColor = false;
            chb_Мащерка.CheckedChanged += chb_Мащерка_CheckedChanged;
            // 
            // chb_Змийско
            // 
            chb_Змийско.AutoSize = true;
            chb_Змийско.BackColor = Color.Transparent;
            chb_Змийско.FlatStyle = FlatStyle.Flat;
            chb_Змийско.Font = new Font("Georgia", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            chb_Змийско.ForeColor = Color.DarkSeaGreen;
            chb_Змийско.Location = new Point(53, 446);
            chb_Змийско.Name = "chb_Змийско";
            chb_Змийско.Size = new Size(175, 24);
            chb_Змийско.TabIndex = 14;
            chb_Змийско.Text = "Змийско Мляко";
            chb_Змийско.UseVisualStyleBackColor = false;
            chb_Змийско.CheckedChanged += chb_Змийско_CheckedChanged;
            // 
            // chb_Магарешки
            // 
            chb_Магарешки.AutoSize = true;
            chb_Магарешки.BackColor = Color.Transparent;
            chb_Магарешки.FlatStyle = FlatStyle.Flat;
            chb_Магарешки.Font = new Font("Georgia", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            chb_Магарешки.ForeColor = Color.DarkSeaGreen;
            chb_Магарешки.Location = new Point(53, 568);
            chb_Магарешки.Name = "chb_Магарешки";
            chb_Магарешки.Size = new Size(198, 24);
            chb_Магарешки.TabIndex = 15;
            chb_Магарешки.Text = "Магарешки бодил";
            chb_Магарешки.UseVisualStyleBackColor = false;
            chb_Магарешки.CheckedChanged += chb_Магарешки_CheckedChanged;
            // 
            // txt_Име
            // 
            txt_Име.BackColor = SystemColors.MenuBar;
            txt_Име.BorderStyle = BorderStyle.FixedSingle;
            txt_Име.Font = new Font("Georgia", 9F, FontStyle.Italic, GraphicsUnit.Point);
            txt_Име.ForeColor = Color.Black;
            txt_Име.Location = new Point(1072, 204);
            txt_Име.Name = "txt_Име";
            txt_Име.Size = new Size(151, 25);
            txt_Име.TabIndex = 16;
            // 
            // cmb_оплаквания
            // 
            cmb_оплаквания.BackColor = SystemColors.MenuBar;
            cmb_оплаквания.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb_оплаквания.FlatStyle = FlatStyle.Flat;
            cmb_оплаквания.Font = new Font("Georgia", 9F, FontStyle.Italic, GraphicsUnit.Point);
            cmb_оплаквания.FormattingEnabled = true;
            cmb_оплаквания.Items.AddRange(new object[] { "Стомашни проблеми", "Кашлица", "Кожни проблеми", "Пречистване на организма" });
            cmb_оплаквания.Location = new Point(1072, 276);
            cmb_оплаквания.Name = "cmb_оплаквания";
            cmb_оплаквания.Size = new Size(151, 26);
            cmb_оплаквания.TabIndex = 17;
            cmb_оплаквания.SelectedIndexChanged += cmb_оплаквания_SelectedIndexChanged;
            // 
            // lbl_лайка
            // 
            lbl_лайка.AutoSize = true;
            lbl_лайка.BackColor = Color.Transparent;
            lbl_лайка.Font = new Font("Georgia", 9F, FontStyle.Italic, GraphicsUnit.Point);
            lbl_лайка.ForeColor = Color.Linen;
            lbl_лайка.Location = new Point(53, 246);
            lbl_лайка.Name = "lbl_лайка";
            lbl_лайка.Size = new Size(191, 36);
            lbl_лайка.TabIndex = 18;
            lbl_лайка.Text = "Успокоява и помага\r\nпри стомашни проблеми";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Georgia", 9F, FontStyle.Italic, GraphicsUnit.Point);
            label1.ForeColor = Color.Linen;
            label1.Location = new Point(53, 370);
            label1.Name = "label1";
            label1.Size = new Size(187, 36);
            label1.TabIndex = 19;
            label1.Text = "Действа антисептично\r\nи помага при кашлица";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Georgia", 9F, FontStyle.Italic, GraphicsUnit.Point);
            label2.ForeColor = Color.Linen;
            label2.Location = new Point(53, 491);
            label2.Name = "label2";
            label2.Size = new Size(169, 36);
            label2.TabIndex = 20;
            label2.Text = "Използва се за кожни \r\nпроблеми и брадавици";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Georgia", 9F, FontStyle.Italic, GraphicsUnit.Point);
            label3.ForeColor = Color.Linen;
            label3.Location = new Point(53, 613);
            label3.Name = "label3";
            label3.Size = new Size(224, 36);
            label3.TabIndex = 21;
            label3.Text = "Помага на черния дроб и \r\nпречистването на организма";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Location = new Point(435, 204);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(325, 224);
            pictureBox1.TabIndex = 22;
            pictureBox1.TabStop = false;
            // 
            // btn_отвара
            // 
            btn_отвара.BackColor = Color.Transparent;
            btn_отвара.BackgroundImage = (Image)resources.GetObject("btn_отвара.BackgroundImage");
            btn_отвара.BackgroundImageLayout = ImageLayout.Stretch;
            btn_отвара.FlatAppearance.BorderSize = 0;
            btn_отвара.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn_отвара.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn_отвара.FlatStyle = FlatStyle.Flat;
            btn_отвара.Font = new Font("Georgia", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            btn_отвара.ForeColor = Color.Black;
            btn_отвара.Location = new Point(826, 613);
            btn_отвара.Name = "btn_отвара";
            btn_отвара.Size = new Size(466, 142);
            btn_отвара.TabIndex = 23;
            btn_отвара.UseVisualStyleBackColor = false;
            btn_отвара.Click += btn_отвара_Click;
            // 
            // lbl_билка
            // 
            lbl_билка.AutoSize = true;
            lbl_билка.BackColor = Color.Transparent;
            lbl_билка.Font = new Font("Georgia", 18F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            lbl_билка.Location = new Point(435, 435);
            lbl_билка.Name = "lbl_билка";
            lbl_билка.Size = new Size(0, 35);
            lbl_билка.TabIndex = 24;
            // 
            // lbl_Семейство
            // 
            lbl_Семейство.AutoSize = true;
            lbl_Семейство.BackColor = Color.Transparent;
            lbl_Семейство.Font = new Font("Georgia", 10.2F, FontStyle.Italic, GraphicsUnit.Point);
            lbl_Семейство.Location = new Point(435, 490);
            lbl_Семейство.Name = "lbl_Семейство";
            lbl_Семейство.Size = new Size(0, 20);
            lbl_Семейство.TabIndex = 25;
            // 
            // lbl_Действие
            // 
            lbl_Действие.AutoSize = true;
            lbl_Действие.BackColor = Color.Transparent;
            lbl_Действие.Font = new Font("Georgia", 10.2F, FontStyle.Italic, GraphicsUnit.Point);
            lbl_Действие.Location = new Point(435, 529);
            lbl_Действие.Name = "lbl_Действие";
            lbl_Действие.Size = new Size(0, 20);
            lbl_Действие.TabIndex = 26;
            // 
            // lbl_Част
            // 
            lbl_Част.AutoSize = true;
            lbl_Част.BackColor = Color.Transparent;
            lbl_Част.Font = new Font("Georgia", 10.2F, FontStyle.Italic, GraphicsUnit.Point);
            lbl_Част.Location = new Point(435, 593);
            lbl_Част.Name = "lbl_Част";
            lbl_Част.Size = new Size(0, 20);
            lbl_Част.TabIndex = 27;
            // 
            // lbl_Препоръка
            // 
            lbl_Препоръка.AutoSize = true;
            lbl_Препоръка.BackColor = Color.Transparent;
            lbl_Препоръка.Font = new Font("Georgia", 13.8F, FontStyle.Italic, GraphicsUnit.Point);
            lbl_Препоръка.Location = new Point(861, 413);
            lbl_Препоръка.Name = "lbl_Препоръка";
            lbl_Препоръка.Size = new Size(0, 27);
            lbl_Препоръка.TabIndex = 28;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1304, 843);
            Controls.Add(lbl_Препоръка);
            Controls.Add(lbl_Част);
            Controls.Add(lbl_Действие);
            Controls.Add(lbl_Семейство);
            Controls.Add(lbl_билка);
            Controls.Add(btn_отвара);
            Controls.Add(pictureBox1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lbl_лайка);
            Controls.Add(cmb_оплаквания);
            Controls.Add(txt_Име);
            Controls.Add(chb_Магарешки);
            Controls.Add(chb_Змийско);
            Controls.Add(chb_Мащерка);
            Controls.Add(chb_лайка);
            Controls.Add(lbl_информация);
            Controls.Add(lbl_избрани);
            Controls.Add(lbl_profil);
            Controls.Add(lbl_оплакване);
            Controls.Add(lbl_име);
            Controls.Add(lbl_herbs);
            Controls.Add(lbl_title);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Form1";
            Text = "Form1";
            Load += cmb_оплаквания_SelectedIndexChanged;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbl_title;
        private Label lbl_herbs;
        private Label lbl_име;
        private Label lbl_оплакване;
        private Label lbl_profil;
        private Label lbl_избрани;
        private Label lbl_информация;
        private CheckBox chb_лайка;
        private CheckBox chb_Мащерка;
        private CheckBox chb_Змийско;
        private CheckBox chb_Магарешки;
        private TextBox txt_Име;
        private ComboBox cmb_оплаквания;
        private Label lbl_лайка;
        private Label label1;
        private Label label2;
        private Label label3;
        private PictureBox pictureBox1;
        private CheckedListBox checkedListBox1;
        private Button btn_отвара;
        private Label lbl_билка;
        private Label lbl_Семейство;
        private Label lbl_Действие;
        private Label lbl_Част;
        private Label lbl_Препоръка;
    }
}