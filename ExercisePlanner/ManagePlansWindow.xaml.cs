using ExercisePlanner.DAL;
using System;
using System.Linq;
using System.Windows;
using static ExercisePlanner.DAL.DatabaseHelper;

namespace ExercisePlanner
{
    public partial class ManagePlansWindow : Window
    {
        public ManagePlansWindow()
        {
            InitializeComponent();
            LoadSamplePlans();
            LoadSampleExercises();
        }

        private void LoadSamplePlans()
        {
            try
            {
                var samplePlans = DatabaseHelper.GetSamplePlans();
                PlansDataGrid.ItemsSource = samplePlans;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load sample plans: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSampleExercises()
        {
            try
            {
                var sampleExercises = DatabaseHelper.GetSampleExercises();
                ExercisesDataGrid.ItemsSource = sampleExercises;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load sample exercises: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSampleExercise(object sender, RoutedEventArgs e)
        {
            var selectedExercise = ExercisesDataGrid.SelectedItem as SampleExercise;
            if (selectedExercise != null)
            {
                var plans = DatabaseHelper.GetPlansContainingExercise(selectedExercise.Id);

                if (plans.Any())
                {
                    string planNames = string.Join(", ", plans);
                    MessageBox.Show(
                        $"This exercise is part of the following plans: {planNames}. Remove it from these plans before deleting.",
                        "Cannot Delete Exercise",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return; 
                }

                DatabaseHelper.DeleteSampleExercise(selectedExercise.Id);
                LoadSampleExercises(); 
            }
        }


        private void AddSamplePlan(object sender, RoutedEventArgs e)
        {
            var addPlanWindow = new EditPlanWindow(); 
            addPlanWindow.ShowDialog(); 
            LoadSamplePlans(); 
        }

        private void EditSamplePlan(object sender, RoutedEventArgs e)
        {
            var selectedPlan = PlansDataGrid.SelectedItem as SamplePlan;
            if (selectedPlan != null)
            {
                var editPlanWindow = new EditPlanWindow(selectedPlan); 
                editPlanWindow.ShowDialog();
                LoadSamplePlans(); 
            }
        }

        private void DeleteSamplePlan(object sender, RoutedEventArgs e)
        {
            var selectedPlan = PlansDataGrid.SelectedItem as SamplePlan;
            if (selectedPlan != null)
            {
                DatabaseHelper.DeleteSamplePlan(selectedPlan.Id);
                LoadSamplePlans(); 
            }
        }

        
        private void AddSampleExercise(object sender, RoutedEventArgs e)
        {
            var addExerciseWindow = new EditExerciseWindow(); 
            addExerciseWindow.ShowDialog(); 
            LoadSampleExercises(); 
        }

        private void EditSampleExercise(object sender, RoutedEventArgs e)
        {
            var selectedExercise = ExercisesDataGrid.SelectedItem as SampleExercise;
            if (selectedExercise != null)
            {
                var editExerciseWindow = new EditExerciseWindow(selectedExercise); 
                editExerciseWindow.ShowDialog();
                LoadSampleExercises(); 
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
            Close();
        }


    }
}
