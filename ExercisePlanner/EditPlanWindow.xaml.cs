using ExercisePlanner.DAL;
using System.Text.RegularExpressions;
using System.Windows;
using static ExercisePlanner.DAL.DatabaseHelper;

namespace ExercisePlanner
{
    public partial class EditPlanWindow : Window
    {
        public string PlanName => PlanNameTextBox.Text;
        public string Description => DescriptionTextBox.Text;

        public string SampleExercisesID => SampleExercisesTextBox.Text;

        public int UserId { get; private set; }
        public SamplePlan ExistingPlan { get; private set; }

        public EditPlanWindow()
        {
            InitializeComponent();
        }

        public EditPlanWindow(SamplePlan planToEdit)
        {
            InitializeComponent();
            ExistingPlan = planToEdit;

            PlanNameTextBox.Text = planToEdit.Name;
            DescriptionTextBox.Text = planToEdit.Description;
            SampleExercisesTextBox.Text = planToEdit.SampleExerciseIds;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PlanName) || string.IsNullOrWhiteSpace(Description) || string.IsNullOrWhiteSpace(SampleExercisesID))
            {
                MessageBox.Show("Please fill all required fields.");
                return;
            }

            if (!IsValidSampleExercisesID(SampleExercisesID))
            {
                MessageBox.Show("Sample Exercise IDs should be a comma-separated list of numbers with no extra commas.");
                return;
            }

            var existingIds = DatabaseHelper.GetAllExerciseIds();


            var inputIds = SampleExercisesID.Split(',').Select(id => int.Parse(id)).ToList();


            var missingIds = inputIds.Except(existingIds).ToList();
            if (missingIds.Any())
            {
                MessageBox.Show($"The following Exercise IDs do not exist: {string.Join(", ", missingIds)}");
                return;
            }

            if (ExistingPlan == null) 
            {
                try
                {
                    DatabaseHelper.AddSamplePlan(PlanName, Description, SampleExercisesID);
                    DialogResult = true; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to add plan: {ex.Message}");
                }
            }
            else 
            {
                try
                {
                    DatabaseHelper.UpdateSamplePlan(ExistingPlan.Id, PlanName, Description, SampleExercisesID);
                    DialogResult = true; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to edit plan: {ex.Message}");
                }
            }

            Close();
        }

        private bool IsValidSampleExercisesID(string input)
        {

            var regex = new Regex(@"^\d+(,\d+)*$");
            return regex.IsMatch(input);
        }
    }
}
