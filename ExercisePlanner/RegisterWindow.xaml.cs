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

namespace ExercisePlanner
{

    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            string result = DatabaseHelper.RegisterUser(username, password);


            if (result == "Registration successful.")
            {
                MessageBox.Show(result, "Succesfull. Please now log into account.");
                MainWindow mainWindow = new MainWindow(); 
                mainWindow.Show(); 

            }
            
            else
            {
                MessageBox.Show(result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }

    }
}
