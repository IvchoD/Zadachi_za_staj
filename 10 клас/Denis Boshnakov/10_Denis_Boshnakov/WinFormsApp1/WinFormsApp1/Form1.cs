using System.Xml.Linq;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void checkedListBoxHerbs_SelectedIndexChanged(object sender, EventArgs e)
        {
            string choice = checkedListBoxHerbs.SelectedItem.ToString();

            if (choice == "Лайка")
            {
                pictureBox1.BackgroundImage = Properties.Resources.Chamomile;
            }
            else if (choice == "Мащерка")
            {
                pictureBox1.BackgroundImage = Properties.Resources.Thyme;
            }
            else if (choice == "Змийско мляко")
            {
                pictureBox1.BackgroundImage = Properties.Resources.GreaterCelandine;
            }
            else if (choice == "Магарешки бодил")
            {
                pictureBox1.BackgroundImage = Properties.Resources.MilkThistle;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextBoxPatient.PlaceholderText = "Име на пациента...";
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string PatientName = TextBoxPatient.Text;

            string symptom = Symptoms.SelectedItem.ToString();

            string choice = checkedListBoxHerbs.CheckedItems.ToString();

            string herbs = "";

            
            foreach (object item in checkedListBoxHerbs.CheckedItems)
            {
                string herbName = item.ToString();
                string grams = "20гр. ";

                switch (herbName)
                {
                    case "Лайка":
                        grams = "20гр. ";
                        break;
                    case "Мащерка":
                        grams = "15гр. ";
                        break;
                    case "Змийско мляко":
                        grams = "10гр. ";
                        break;
                    case "Магарешки бодил":
                        grams = "25гр. ";
                        break;
                }
                    herbs = herbs + grams + herbName.ToString() + " и ";
            }

            if (herbs.EndsWith(" и "))
            {
                herbs = herbs.Substring(0, herbs.Length - 3) + ". ";
            }
            

            string recipe = "Рецептата за " + PatientName + " е готова: " +
                        herbs + " За оплакване: " + symptom + ". " +
                        "Да се пие на гладно по пълнолуние!";

            MessageBox.Show(recipe, "recipe");
        }

        private void TextBoxPatient_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
