﻿using ExercisePlanner.DAL;
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

    public partial class AddEditExerciseWindow : Window
    {
        public string ExerciseName => ExerciseNameTextBox.Text;
        public string Category => CategoryTextBox.Text;
        public int Reps => int.TryParse(RepsTextBox.Text, out int r) ? r : 0;

        public int Sets => int.TryParse(SetsTextBox.Text, out int s) ? s : 0;
        public string Day => (DayComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

        public int UserId { get; private set; }
        public Exercise ExistingExercise { get; private set; }


        public AddEditExerciseWindow(int userId)
        {
            InitializeComponent();
            UserId = userId;
        }

        public AddEditExerciseWindow(int userId, Exercise exerciseToEdit)
        {
            InitializeComponent();
            UserId = userId;
            ExistingExercise = exerciseToEdit;

            ExerciseNameTextBox.Text = exerciseToEdit.Name;
            CategoryTextBox.Text = exerciseToEdit.Category;
            RepsTextBox.Text = exerciseToEdit.Reps.ToString();
            SetsTextBox.Text = exerciseToEdit.Sets.ToString();
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

            if (ExistingExercise == null) 
            {
                try
                {
                    DatabaseHelper.AddExercise(UserId, ExerciseName, Category, Reps, Sets, Day);
                    DialogResult = true; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to add exercise: {ex.Message}");
                }
            }
            else 
            {
                try
                {
                    DatabaseHelper.EditExercise(ExistingExercise.Id, ExerciseName, Category, Reps, Sets, Day);
                    DialogResult = true; 
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
