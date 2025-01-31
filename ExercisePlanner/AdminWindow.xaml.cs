using System;
using System.Windows;
using System.Windows.Controls;
using ExercisePlanner.DAL;
using static ExercisePlanner.DAL.DatabaseHelper;

namespace ExercisePlanner
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = DatabaseHelper.GetAllUsers();
            UsersDataGrid.ItemsSource = users; 
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (RoleComboBox.SelectedItem is not ComboBoxItem selectedRoleItem)
            {
                MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string role = selectedRoleItem.Content.ToString();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username and password cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string result = DatabaseHelper.AddUser(username, password, role);
            MessageBox.Show(result, "Add User", MessageBoxButton.OK, result == "User added successfully." ? MessageBoxImage.Information : MessageBoxImage.Error);

            if (result == "User added successfully.")
            {
                LoadUsers(); 
            }
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is not User selectedUser)
            {
                MessageBox.Show("Please select a user to edit.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string newUsername = UsernameTextBox.Text.Trim();
            string newPassword = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(newUsername) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Username and password cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RoleComboBox.SelectedItem is not ComboBoxItem selectedRoleItem)
            {
                MessageBox.Show("Please select a new role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string newRole = selectedRoleItem.Content.ToString();

            string result = DatabaseHelper.ModifyUser(selectedUser.Id, newUsername, newPassword, newRole);
            MessageBox.Show(result, "Edit User", MessageBoxButton.OK, result == "User modified successfully." ? MessageBoxImage.Information : MessageBoxImage.Error);

            if (result == "User modified successfully.")
            {
                LoadUsers(); 
            }
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is not User selectedUser)
            {
                MessageBox.Show("Please select a user to delete.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirmResult = MessageBox.Show($"Are you sure you want to delete user '{selectedUser.Username}'?",
                                                "Confirm Delete",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Question);

            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }

            string result = DatabaseHelper.DeleteUser(selectedUser.Id);
            MessageBox.Show(result, "Delete User", MessageBoxButton.OK, result == "User deleted successfully." ? MessageBoxImage.Information : MessageBoxImage.Error);

            if (result == "User deleted successfully.")
            {
                LoadUsers(); 
            }
        }

        private void SamplesButton_Click(object sender, RoutedEventArgs e)
        {
            ManagePlansWindow plansWindow = new ManagePlansWindow();
            plansWindow.Show();
            Close();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
