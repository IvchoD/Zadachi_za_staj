namespace _10_raya_kirilova_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cmb_оплаквания_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_оплаквания.SelectedIndex == 0)
            {
                lbl_Препоръка.Text = "Комбинацията от:\n повече Лайка и Мащерка \n успокоява стомаха и\n намаляват възпалението \n (лайката е основната билка)";
            }
            else if (cmb_оплаквания.SelectedIndex == 1)
            {
                lbl_Препоръка.Text = "Комбинацията от:\n повече Мащерка и Лайка \n облекчава кашлицата и \n успокояват гърлото\n (мащерката е основната билка)";
            }

            else if (cmb_оплаквания.SelectedIndex == 2)
            {
                lbl_Препоръка.Text = "Комбинацията от:\n Змийско мляко и Лайка \n помагат при кожни проблеми \n (змийското мляко е\n основната билка)";
            }

            else if (cmb_оплаквания.SelectedIndex == 3)
            {
                lbl_Препоръка.Text = "Комбинацията от:\n  Магарешки бодил и Змийско мляко \n подпомагат черния дроб \n (магарешкият бодил е\n основната билка)";
            }


        }

        private void chb_лайка_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_лайка.Checked)
            {
                pictureBox1.Image = Image.FromFile(@"C:\Users\KRASI HP ED\Desktop\10_raya_kirilova_\images\Лайка.png");
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                lbl_билка.Text = "Лайка";
                lbl_Действие.Text = "Действие: Противовъзпалително,\n успокояващо, спазмолитично";
                lbl_Семейство.Text = " Семейство: Сложноцветни";
                lbl_Част.Text = "Част, която се използва: Цветовете";
            }
        }

        private void chb_Мащерка_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_Мащерка.Checked)
            {
                pictureBox1.Image = Image.FromFile(@"C:\Users\KRASI HP ED\Desktop\10_raya_kirilova_\images\Мащерка.png");
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                lbl_билка.Text = "Мащерка";
                lbl_Действие.Text = "Действие: Отхрачващо, антисептично,\n противокашлично, противомикробно";
                lbl_Семейство.Text = " Семейство: Устоцветни";
                lbl_Част.Text = "Част, която се използва: Стръковете";

            }
        }

        private void chb_Змийско_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_Змийско.Checked)
            {
                pictureBox1.Image = Image.FromFile(@"C:\Users\KRASI HP ED\Desktop\10_raya_kirilova_\images\Змийско_Мляко.png");
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                lbl_билка.Text = "Змийско мляко";
                lbl_Действие.Text = "Действие: Спазмолитично, жлъчегонно, \n млечният сок се използва при брадавици";
                lbl_Семейство.Text = " Семейство: Макови";
                lbl_Част.Text = "Част, която се използва: \n Надземната част и млечният сок";

            }
        }

        private void chb_Магарешки_CheckedChanged(object sender, EventArgs e)
        {
            if (chb_Магарешки.Checked)
            {
                pictureBox1.Image = Image.FromFile(@"C:\Users\KRASI HP ED\Desktop\10_raya_kirilova_\images\Магарешки_бодил.png");
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                lbl_билка.Text = "Магарешки бодил";
                lbl_Действие.Text = "Действие: Защитава и подпомага \n възстановяването на черния дроб";
                lbl_Семейство.Text = " Семейство: Сложноцветни";
                lbl_Част.Text = "Част, която се използва: Семената";
            }
        }

        private void btn_отвара_Click(object sender, EventArgs e)
        {
            string recepta = "";
            if (cmb_оплаквания.SelectedIndex == 0)
            {
                if (chb_лайка.Checked)
                    recepta += "25 гр. Лайка\n";

                if (chb_Мащерка.Checked)
                    recepta += "20 гр. Мащерка\n";

                if (chb_Змийско.Checked)
                    recepta += "10 гр. Змийско мляко\n";

                if (chb_Магарешки.Checked)
                    recepta += "10 гр. Магарешки бодил\n";
            }

            else if (cmb_оплаквания.SelectedIndex == 1)
            {
                if (chb_лайка.Checked)
                    recepta += "20 гр. Лайка\n";

                if (chb_Мащерка.Checked)
                    recepta += "25 гр. Мащерка\n";

                if (chb_Змийско.Checked)
                    recepta += "10 гр. Змийско мляко\n";

                if (chb_Магарешки.Checked)
                    recepta += "10 гр. Магарешки бодил\n";
            }

            else if (cmb_оплаквания.SelectedIndex == 2)
            {
                if (chb_лайка.Checked)
                    recepta += "15 гр. Лайка\n";

                if (chb_Мащерка.Checked)
                    recepta += "10 гр. Мащерка\n";

                if (chb_Змийско.Checked)
                    recepta += "25 гр. Змийско мляко\n";

                if (chb_Магарешки.Checked)
                    recepta += "5 гр. Магарешки бодил\n";
            }

            else if (cmb_оплаквания.SelectedIndex == 3)
            {
                if (chb_лайка.Checked)
                    recepta += "10 гр. Лайка\n";

                if (chb_Мащерка.Checked)
                    recepta += "10 гр. Мащерка\n";

                if (chb_Змийско.Checked)
                    recepta += "15 гр. Змийско мляко\n";

                if (chb_Магарешки.Checked)
                    recepta += "25 гр. Магарешки бодил\n";
            }

            if (recepta == "")
            {
                MessageBox.Show("Не сте избрали билки.");
                return;
            }


            MessageBox.Show(
                "Рецептата за " + txt_Име.Text + " е готова!\n\n" + "Оплакване: " + cmb_оплаквания.Text + "\n\n" + recepta + "\nДа се пие на гладно по пълнолуние!",
                "Дигиталният Хербарий на Знахаря");
        }
    }
}