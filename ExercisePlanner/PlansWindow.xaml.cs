using ExercisePlanner.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static ExercisePlanner.DAL.DatabaseHelper;
using System.Windows.Controls;

namespace ExercisePlanner
{
    public partial class PlansWindow : Window
    {
        private readonly User _currentUser;
        private readonly MainAppWindow _mainAppWindow;

        public PlansWindow(User currentUser, MainAppWindow mainAppWindow)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _mainAppWindow = mainAppWindow;
            LoadSamplePlans();
        }

        private void LoadSamplePlans()
        {
            try
            {
                var samplePlans = DatabaseHelper.GetSamplePlans();

                if (samplePlans == null || !samplePlans.Any())
                {
                    MessageBox.Show("No sample plans found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }


                PlansDataGrid.ItemsSource = samplePlans;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load plans: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddToPlanButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlansDataGrid.SelectedItem == null)
            {
                MessageBox.Show("No plan selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (PlansDataGrid.SelectedItem is SamplePlan selectedPlan)
            {
                if (DayComboBox.SelectedItem is ComboBoxItem selectedDayItem)
                {
                    string selectedDay = selectedDayItem.Content.ToString();
                    DatabaseHelper.AddPlanToUserExercises(_currentUser.Id, selectedPlan.Id, selectedDay);
                    _mainAppWindow.LoadExercises();
                    MessageBox.Show($"Plan '{selectedPlan.Name}' has been added to your schedule on {selectedDay}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No day selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainAppWindow mainAppWindow = new MainAppWindow(_currentUser);
            mainAppWindow.Show();
            Close();
        }

    }
}
