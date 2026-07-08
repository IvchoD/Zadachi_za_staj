namespace zadacha
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "Дигиталният хербарий на бай Михал";
        }

        private void btnBrew_Click(object sender, EventArgs e)
        {
            // "Тези 3 проверки за глупакоустойчивост ги измисли някой по - умен от мене!" - A wise man
            if (herbs.CheckedItems.Count == 0)
            {
                MessageBox.Show("Моля, изберете поне една билка!", "Липсват билки", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }
            if (Symptoms.SelectedIndex == -1)
            {
                MessageBox.Show("Моля, изберете симптом от списъка!", "Липсва симптом", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(txtPatientName.Text))
            {
                MessageBox.Show("Моля, въведете име на пациента!", "Липсва име", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            Random random = new Random();
            int num = random.Next(10, 41);

            string patientName = txtPatientName.Text;
            string finalMessage = $"Рецептата за {patientName} е готова: ";

            if (herbs.GetItemCheckState(0) == CheckState.Checked)
            {
                finalMessage += $"{num}гр лайка, ";
                num = random.Next(10, 41);
            }
            if (herbs.GetItemCheckState(1) == CheckState.Checked)
            {
                finalMessage += $"{num}гр мащерка, ";
                num = random.Next(10, 41);
            }
            if (herbs.GetItemCheckState(2) == CheckState.Checked)
            {
                finalMessage += $"{num}гр змийско мляко, ";
                num = random.Next(10, 41);
            }
            if (herbs.GetItemCheckState(3) == CheckState.Checked)
            {
                finalMessage += $"{num}гр магарешки бодил, ";
            }

            finalMessage = finalMessage.Substring(0, finalMessage.Length - 2) + ". ";
            string recommendation = "";
            if (Symptoms.SelectedIndex == 0)
            {
                recommendation = "Да се приема след третото кукуригане на петела!";
            }
            else if (Symptoms.SelectedIndex == 1)
            {
                recommendation = "Да се приема с чаша изворна вода и добро настроение!";
            }
            else if (Symptoms.SelectedIndex == 2)
            {
                recommendation = "Да се пие точно преди залез слънце, обърнат(а) на изток!";

            }
            else if (Symptoms.SelectedIndex == 3)
            {
                recommendation = "Да се приема след три дълбоки въздишки и една усмивка!";
            }
            finalMessage += recommendation;
            MessageBox.Show(finalMessage, "Вашата рецепта!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void herbs_SelectedIndexChanged(object sender, EventArgs e)
        {
            //1
            if (herbs.GetItemCheckState(0) == CheckState.Checked)
            {
                picChamomile.Visible = true;
            }
            else
            {
                picChamomile.Visible = false;
            }
            //2
            if (herbs.GetItemCheckState(1) == CheckState.Checked)
            {
                picThyme.Visible = true;
            }
            else
            {
                picThyme.Visible = false;
            }
            //3
            if (herbs.GetItemCheckState(2) == CheckState.Checked)
            {
                picSnakeMilk.Visible = true;
            }
            else
            {
                picSnakeMilk.Visible = false;
            }
            //4
            if (herbs.GetItemCheckState(3) == CheckState.Checked)
            {
                picDonkeyThorn.Visible = true;
            }
            else
            {
                picDonkeyThorn.Visible = false;
            }
        }

        private void picChamomile_Click(object sender, EventArgs e)
        {

        }

        private void picThyme_Click(object sender, EventArgs e)
        {

        }

        private void picSnakeMilk_Click(object sender, EventArgs e)
        {

        }

        private void picDonkeyThorn_Click(object sender, EventArgs e)
        {

        }

        private void Symptoms_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtPatientName_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblName_Click(object sender, EventArgs e)
        {

        }
    }
}
