namespace zadacha
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
            herbs = new CheckedListBox();
            Symptoms = new ComboBox();
            txtPatientName = new TextBox();
            btnBrew = new Button();
            picChamomile = new PictureBox();
            picThyme = new PictureBox();
            picSnakeMilk = new PictureBox();
            picDonkeyThorn = new PictureBox();
            lblName = new Label();
            lblSymptom = new Label();
            lblHerbs = new Label();
            lblTitle = new Label();
            picSituation = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picChamomile).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picThyme).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSnakeMilk).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picDonkeyThorn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSituation).BeginInit();
            SuspendLayout();
            // 
            // herbs
            // 
            herbs.BackColor = Color.FromArgb(229, 230, 229);
            herbs.CheckOnClick = true;
            herbs.Font = new Font("Arial Narrow", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            herbs.FormattingEnabled = true;
            herbs.Items.AddRange(new object[] { "Лайка", "Мащерка", "Змийско мляко", "Магарешки бодил" });
            herbs.Location = new Point(33, 313);
            herbs.Name = "herbs";
            herbs.Size = new Size(456, 236);
            herbs.TabIndex = 0;
            herbs.SelectedIndexChanged += herbs_SelectedIndexChanged;
            // 
            // Symptoms
            // 
            Symptoms.DropDownStyle = ComboBoxStyle.DropDownList;
            Symptoms.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Symptoms.FormattingEnabled = true;
            Symptoms.Items.AddRange(new object[] { "Болки в кръста", "Уроки от съседката", "Запушен нос", "Сухо гърло" });
            Symptoms.Location = new Point(652, 228);
            Symptoms.Name = "Symptoms";
            Symptoms.Size = new Size(303, 45);
            Symptoms.TabIndex = 1;
            Symptoms.SelectedIndexChanged += Symptoms_SelectedIndexChanged;
            // 
            // txtPatientName
            // 
            txtPatientName.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPatientName.Location = new Point(652, 386);
            txtPatientName.Name = "txtPatientName";
            txtPatientName.Size = new Size(269, 43);
            txtPatientName.TabIndex = 2;
            txtPatientName.TextChanged += txtPatientName_TextChanged;
            // 
            // btnBrew
            // 
            btnBrew.BackColor = Color.FromArgb(205, 185, 139);
            btnBrew.Font = new Font("Arial Narrow", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBrew.Location = new Point(652, 457);
            btnBrew.Name = "btnBrew";
            btnBrew.Size = new Size(287, 73);
            btnBrew.TabIndex = 3;
            btnBrew.Text = "Забъркай отварата";
            btnBrew.UseVisualStyleBackColor = false;
            btnBrew.Click += btnBrew_Click;
            // 
            // picChamomile
            // 
            picChamomile.Image = (Image)resources.GetObject("picChamomile.Image");
            picChamomile.Location = new Point(417, 318);
            picChamomile.Name = "picChamomile";
            picChamomile.Size = new Size(52, 50);
            picChamomile.SizeMode = PictureBoxSizeMode.StretchImage;
            picChamomile.TabIndex = 4;
            picChamomile.TabStop = false;
            picChamomile.Visible = false;
            picChamomile.Click += picChamomile_Click;
            // 
            // picThyme
            // 
            picThyme.Image = (Image)resources.GetObject("picThyme.Image");
            picThyme.Location = new Point(417, 377);
            picThyme.Name = "picThyme";
            picThyme.Size = new Size(52, 51);
            picThyme.SizeMode = PictureBoxSizeMode.StretchImage;
            picThyme.TabIndex = 5;
            picThyme.TabStop = false;
            picThyme.Visible = false;
            picThyme.Click += picThyme_Click;
            // 
            // picSnakeMilk
            // 
            picSnakeMilk.Image = (Image)resources.GetObject("picSnakeMilk.Image");
            picSnakeMilk.Location = new Point(417, 439);
            picSnakeMilk.Name = "picSnakeMilk";
            picSnakeMilk.Size = new Size(52, 46);
            picSnakeMilk.SizeMode = PictureBoxSizeMode.StretchImage;
            picSnakeMilk.TabIndex = 6;
            picSnakeMilk.TabStop = false;
            picSnakeMilk.Visible = false;
            picSnakeMilk.Click += picSnakeMilk_Click;
            // 
            // picDonkeyThorn
            // 
            picDonkeyThorn.Image = (Image)resources.GetObject("picDonkeyThorn.Image");
            picDonkeyThorn.Location = new Point(417, 501);
            picDonkeyThorn.Name = "picDonkeyThorn";
            picDonkeyThorn.Size = new Size(63, 41);
            picDonkeyThorn.SizeMode = PictureBoxSizeMode.StretchImage;
            picDonkeyThorn.TabIndex = 7;
            picDonkeyThorn.TabStop = false;
            picDonkeyThorn.Visible = false;
            picDonkeyThorn.Click += picDonkeyThorn_Click;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.BackColor = Color.Transparent;
            lblName.Font = new Font("Arial Narrow", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblName.Location = new Point(642, 301);
            lblName.Name = "lblName";
            lblName.Size = new Size(117, 57);
            lblName.TabIndex = 8;
            lblName.Text = "Име:";
            lblName.Click += lblName_Click;
            // 
            // lblSymptom
            // 
            lblSymptom.AutoSize = true;
            lblSymptom.BackColor = Color.Transparent;
            lblSymptom.Font = new Font("Arial Narrow", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSymptom.Location = new Point(641, 143);
            lblSymptom.Name = "lblSymptom";
            lblSymptom.Size = new Size(239, 57);
            lblSymptom.TabIndex = 9;
            lblSymptom.Text = "Симптоми:";
            // 
            // lblHerbs
            // 
            lblHerbs.AutoSize = true;
            lblHerbs.BackColor = Color.Transparent;
            lblHerbs.Font = new Font("Arial Narrow", 48F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblHerbs.Location = new Point(21, 224);
            lblHerbs.Name = "lblHerbs";
            lblHerbs.Size = new Size(210, 75);
            lblHerbs.TabIndex = 10;
            lblHerbs.Text = "Билки:";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Arial Narrow", 48F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(43, 22);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(938, 75);
            lblTitle.TabIndex = 11;
            lblTitle.Text = "Дигиталният Хербарий на Знахаря";
            // 
            // picSituation
            // 
            picSituation.Image = (Image)resources.GetObject("picSituation.Image");
            picSituation.Location = new Point(285, 120);
            picSituation.Name = "picSituation";
            picSituation.Size = new Size(313, 168);
            picSituation.SizeMode = PictureBoxSizeMode.StretchImage;
            picSituation.TabIndex = 12;
            picSituation.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(229, 220, 209);
            ClientSize = new Size(1030, 577);
            Controls.Add(picSituation);
            Controls.Add(lblTitle);
            Controls.Add(lblHerbs);
            Controls.Add(lblSymptom);
            Controls.Add(lblName);
            Controls.Add(picDonkeyThorn);
            Controls.Add(picSnakeMilk);
            Controls.Add(picThyme);
            Controls.Add(picChamomile);
            Controls.Add(btnBrew);
            Controls.Add(txtPatientName);
            Controls.Add(Symptoms);
            Controls.Add(herbs);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)picChamomile).EndInit();
            ((System.ComponentModel.ISupportInitialize)picThyme).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSnakeMilk).EndInit();
            ((System.ComponentModel.ISupportInitialize)picDonkeyThorn).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSituation).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckedListBox herbs;
        private ComboBox Symptoms;
        private TextBox txtPatientName;
        private Button btnBrew;
        private PictureBox picChamomile;
        private PictureBox picThyme;
        private PictureBox picSnakeMilk;
        private PictureBox picDonkeyThorn;
        private Label lblName;
        private Label lblSymptom;
        private Label lblHerbs;
        private Label lblTitle;
        private PictureBox picSituation;
    }
}
