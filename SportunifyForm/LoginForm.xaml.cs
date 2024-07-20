using Repositories.Models;
using Services.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SportunifyForm
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private async void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill out all form fields.");
                return;
            }

            Account account = new Account
            {
                Username = username,
                Password = password
            };

            var accountService = new AccountService(); // Assuming you have this service

            // Show the loading spinner and disable buttons
            LoadingSpinner.Visibility = Visibility.Visible;
            Login_Button.IsEnabled = false;
            Register_Button.IsEnabled = false;
            Close_Button.IsEnabled = false;

            Account loggedAccount = await Task.Run(() => accountService.Login(account));

            if (loggedAccount != null)
            {
                MainWindow mainWindow = new MainWindow(loggedAccount); // Pass the account data to MainWindow
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Username or password is not correct.");
            }

            // Hide the loading spinner and enable buttons
            LoadingSpinner.Visibility = Visibility.Collapsed;
            Login_Button.IsEnabled = true;
            Register_Button.IsEnabled = true;
            Close_Button.IsEnabled = true;
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            this.Close();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
