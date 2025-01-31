using ExercisePlanner.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static ExercisePlanner.DAL.DatabaseHelper;
using System.Windows.Controls;
using System.Windows.Data;

namespace ExercisePlanner
{
    public partial class MainAppWindow : Window
    {
        public User CurrentUser { get; private set; }
        public string WelcomeMessage => $"Welcome {CurrentUser.Username}! This is your weekly exercise plan:";

        public MainAppWindow(User loggedInUser)
        {
            InitializeComponent();
            CurrentUser = loggedInUser;
            DataContext = this;

            DaysTabControl.SelectedIndex = 0;
        }

        public void LoadExercises()
        {
            try
            {
                int userId = CurrentUser.Id;
                var exercises = DatabaseHelper.GetExercisesByUser(userId);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load exercises: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void AddExerciseButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditExerciseWindow(CurrentUser.Id);
            if (addWindow.ShowDialog() == true)
            {
                LoadExercises();
            }
        }

        private void EditExerciseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExercisesDataGrid.SelectedItem is Exercise selectedExercise) 
            {
                var editWindow = new AddEditExerciseWindow(CurrentUser.Id, selectedExercise);
                if (editWindow.ShowDialog() == true)
                {
                    LoadExercises(); 
                }
            }
            else
            {
                MessageBox.Show("Please select an exercise to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void DeleteExerciseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExercisesDataGrid.SelectedItem is Exercise selectedExercise) 
            {
                var result = MessageBox.Show($"Are you sure you want to delete {selectedExercise.Name}?", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseHelper.DeleteExercise(selectedExercise.Id);
                        LoadExercises(); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete exercise: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an exercise to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void CheckPlansButton_Click(object sender, RoutedEventArgs e)
        {
            PlansWindow plansWindow = new PlansWindow(CurrentUser, this);
            plansWindow.Show();
            Close();
        }
        private void DaysTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DaysTabControl.SelectedItem is TabItem selectedTab)
            {
                string selectedDay = selectedTab.Tag.ToString();
                LoadExercisesForDay(selectedDay);
            }
        }

        private void LoadExercisesForDay(string day)
        {
            try
            {
                int userId = CurrentUser.Id;
                var exercises = DatabaseHelper.GetExercisesByUser(userId)
                    .Where(e => e.Day.Equals(day, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                ExercisesDataGrid.ItemsSource = exercises; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load exercises: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
