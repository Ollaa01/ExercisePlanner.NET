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

            DataContext = this; // Binding dla WelcomeMessage
            LoadExercises();
        }

        private void LoadExercises()
        {
            try
            {
                int userId = CurrentUser.Id;
                var exercises = DatabaseHelper.GetExercisesByUser(userId);

                // Wypełnianie zakładek danymi
                PopulateTabControl(exercises);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load exercises: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateTabControl(IEnumerable<Exercise> exercises)
        {
            foreach (TabItem tab in DaysTabControl.Items)
            {
                string day = tab.Header.ToString();
                var exercisesForDay = exercises
                    .Where(e => e.Day.Equals(day, StringComparison.OrdinalIgnoreCase))
                    .Select(e => new
                    {
                        DisplayText = $"{e.Name} - {e.Category} - Powtórzenia: {e.Reps}",
                        OriginalExercise = e
                    });

                var listBox = new ListBox
                {
                    ItemsSource = exercisesForDay,
                    DisplayMemberPath = "DisplayText", // Wyświetlane teksty
                    Tag = day
                };

                tab.Content = listBox; // Przypisanie ListBox jako zawartości zakładki
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
            if (DaysTabControl.SelectedItem is TabItem selectedTab &&
                selectedTab.Content is ListBox listBox &&
                listBox.SelectedItem is Exercise selectedExercise)
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
            if (DaysTabControl.SelectedItem is TabItem selectedTab &&
                selectedTab.Content is ListBox listBox &&
                listBox.SelectedItem is Exercise selectedExercise)
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
    }
}
