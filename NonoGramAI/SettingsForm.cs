using System;
using System.Windows.Forms;
using NonoGramAI.Properties;

namespace NonoGramAI
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            popTextBox.Text = Settings.Default.Population.ToString();
            genTextBox.Text = Settings.Default.Generations.ToString();
            algTextBox.Text = Settings.Default.Algorithm.ToString();
            crossTextBox.Text = Settings.Default.CrossoverMethod.ToString();
            mutTextBox.Text = Settings.Default.MutationMethod.ToString();
            trivialCheckBox.Checked = Settings.Default.SolveTrivial;
            scoringCheckBox.Checked = Settings.Default.SolutionScoring;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.Default.Population = int.Parse(popTextBox.Text);
                Settings.Default.Generations = int.Parse(genTextBox.Text);
                Settings.Default.Algorithm = int.Parse(algTextBox.Text);
                Settings.Default.CrossoverMethod = int.Parse(crossTextBox.Text);
                Settings.Default.MutationMethod = int.Parse(mutTextBox.Text);
                Settings.Default.SolveTrivial = trivialCheckBox.Checked;
                Settings.Default.SolutionScoring = scoringCheckBox.Checked;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Close();
            }
            Settings.Default.Save();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
