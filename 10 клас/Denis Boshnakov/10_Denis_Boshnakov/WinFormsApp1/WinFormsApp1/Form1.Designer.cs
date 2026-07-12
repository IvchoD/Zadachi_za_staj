namespace WinFormsApp1
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
            pictureBox1 = new PictureBox();
            checkedListBoxHerbs = new CheckedListBox();
            Symptoms = new ComboBox();
            TextBoxPatient = new TextBox();
            button1 = new Button();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            pictureBox5 = new PictureBox();
            pictureBox6 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(236, 101);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(201, 193);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // checkedListBoxHerbs
            // 
            checkedListBoxHerbs.BackColor = Color.LavenderBlush;
            checkedListBoxHerbs.Font = new Font("Comic Sans MS", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            checkedListBoxHerbs.ForeColor = Color.PaleVioletRed;
            checkedListBoxHerbs.FormattingEnabled = true;
            checkedListBoxHerbs.Items.AddRange(new object[] { "Лайка", "Мащерка", "Змийско мляко", "Магарешки бодил" });
            checkedListBoxHerbs.Location = new Point(12, 12);
            checkedListBoxHerbs.Name = "checkedListBoxHerbs";
            checkedListBoxHerbs.Size = new Size(201, 418);
            checkedListBoxHerbs.TabIndex = 1;
            checkedListBoxHerbs.SelectedIndexChanged += checkedListBoxHerbs_SelectedIndexChanged;
            // 
            // Symptoms
            // 
            Symptoms.Font = new Font("Comic Sans MS", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Symptoms.ForeColor = Color.PaleVioletRed;
            Symptoms.FormattingEnabled = true;
            Symptoms.Items.AddRange(new object[] { "Болки в кръста", "Главоболие от раздумка", "Уроки от съседката", "Любовна мъка", "Липса на муза" });
            Symptoms.Location = new Point(21, 132);
            Symptoms.Name = "Symptoms";
            Symptoms.Size = new Size(183, 27);
            Symptoms.TabIndex = 2;
            Symptoms.Text = "Моля изберете...";
            // 
            // TextBoxPatient
            // 
            TextBoxPatient.Font = new Font("Comic Sans MS", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            TextBoxPatient.ForeColor = Color.PaleVioletRed;
            TextBoxPatient.Location = new Point(21, 170);
            TextBoxPatient.Name = "TextBoxPatient";
            TextBoxPatient.Size = new Size(183, 26);
            TextBoxPatient.TabIndex = 3;
            TextBoxPatient.TextChanged += TextBoxPatient_TextChanged;
            // 
            // button1
            // 
            button1.BackColor = Color.LavenderBlush;
            button1.BackgroundImage = (Image)resources.GetObject("button1.BackgroundImage");
            button1.BackgroundImageLayout = ImageLayout.Zoom;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Comic Sans MS", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.PaleVioletRed;
            button1.Location = new Point(21, 224);
            button1.Name = "button1";
            button1.Size = new Size(183, 51);
            button1.TabIndex = 4;
            button1.Text = "Забъркай отварата";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.AccessibleRole = AccessibleRole.None;
            pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.BackgroundImage = (Image)resources.GetObject("pictureBox2.BackgroundImage");
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.Location = new Point(525, -16);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(308, 310);
            pictureBox2.TabIndex = 5;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.BackgroundImage = (Image)resources.GetObject("pictureBox3.BackgroundImage");
            pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox3.Location = new Point(21, 291);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(183, 122);
            pictureBox3.TabIndex = 6;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            pictureBox4.BackColor = Color.Transparent;
            pictureBox4.BackgroundImage = (Image)resources.GetObject("pictureBox4.BackgroundImage");
            pictureBox4.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox4.Location = new Point(653, 325);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(167, 157);
            pictureBox4.TabIndex = 7;
            pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.BackColor = Color.Transparent;
            pictureBox5.BackgroundImage = (Image)resources.GetObject("pictureBox5.BackgroundImage");
            pictureBox5.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox5.Location = new Point(160, 193);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(266, 169);
            pictureBox5.TabIndex = 8;
            pictureBox5.TabStop = false;
            pictureBox5.Click += pictureBox5_Click;
            // 
            // pictureBox6
            // 
            pictureBox6.BackColor = Color.Transparent;
            pictureBox6.BackgroundImage = (Image)resources.GetObject("pictureBox6.BackgroundImage");
            pictureBox6.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox6.Location = new Point(366, 55);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(132, 92);
            pictureBox6.TabIndex = 10;
            pictureBox6.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LavenderBlush;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBox6);
            Controls.Add(pictureBox1);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(button1);
            Controls.Add(TextBoxPatient);
            Controls.Add(Symptoms);
            Controls.Add(checkedListBoxHerbs);
            Controls.Add(pictureBox5);
            DoubleBuffered = true;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private CheckedListBox checkedListBoxHerbs;
        private ComboBox Symptoms;
        private TextBox TextBoxPatient;
        private Button button1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private PictureBox pictureBox6;
    }
}
