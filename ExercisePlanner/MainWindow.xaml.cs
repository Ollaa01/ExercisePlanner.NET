using ExercisePlanner.DAL;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static ExercisePlanner.DAL.DatabaseHelper;

namespace ExercisePlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            User loggedInUser = DatabaseHelper.LoginUser(username, password);

            if (loggedInUser != null)
            {
                if (loggedInUser.Role == "Admin")
                {
                    AdminWindow adminWindow = new AdminWindow(); // Tworzymy obiekt okna administratora
                    adminWindow.Show(); // Pokazujemy okno administratora
                }
                else
                {
                    MainAppWindow mainAppWindow = new MainAppWindow(loggedInUser); // Przekazujemy zalogowanego użytkownika
                    mainAppWindow.Show(); // Pokazujemy główne okno
                }

                // Zamknij okno logowania (MainWindow)
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid credentials.");
            }
        }



        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Otwórz okno rejestracji
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();

            // Zamknij okno logowania (MainWindow)
            this.Close();
        }
    }
}