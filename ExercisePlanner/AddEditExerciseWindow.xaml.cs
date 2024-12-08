using ExercisePlanner.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static ExercisePlanner.DAL.DatabaseHelper;

namespace ExercisePlanner
{
    /// <summary>
    /// Logika interakcji dla klasy AddEditExerciseWindow.xaml
    /// </summary>
    public partial class AddEditExerciseWindow : Window
    {
        public string ExerciseName => ExerciseNameTextBox.Text;
        public string Category => CategoryTextBox.Text;
        public int Reps => int.TryParse(RepsTextBox.Text, out int r) ? r : 0;
        public string Day => (DayComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

        public int UserId { get; private set; }
        public Exercise ExistingExercise { get; private set; }

        // Konstruktor dla dodawania ćwiczenia
        public AddEditExerciseWindow(int userId)
        {
            InitializeComponent();
            UserId = userId;
        }

        // Konstruktor dla edycji ćwiczenia
        public AddEditExerciseWindow(int userId, Exercise exerciseToEdit)
        {
            InitializeComponent();
            UserId = userId;
            ExistingExercise = exerciseToEdit;

            // Wypełnij pola na podstawie istniejącego ćwiczenia
            ExerciseNameTextBox.Text = exerciseToEdit.Name;
            CategoryTextBox.Text = exerciseToEdit.Category;
            RepsTextBox.Text = exerciseToEdit.Reps.ToString();
            foreach (ComboBoxItem item in DayComboBox.Items)
            {
                if (item.Content.ToString() == exerciseToEdit.Day)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ExerciseName) || string.IsNullOrWhiteSpace(Day))
            {
                MessageBox.Show("Please fill all required fields.");
                return;
            }

            if (ExistingExercise == null) // Dodawanie nowego ćwiczenia
            {
                try
                {
                    DatabaseHelper.AddExercise(UserId, ExerciseName, Category, Reps, Day);
                    DialogResult = true; // Potwierdź operację
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to add exercise: {ex.Message}");
                }
            }
            else // Edytowanie istniejącego ćwiczenia
            {
                try
                {
                    DatabaseHelper.EditExercise(ExistingExercise.Id, ExerciseName, Category, Reps, Day);
                    DialogResult = true; // Potwierdź operację
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to edit exercise: {ex.Message}");
                }
            }

            Close();
        }
    }
}
