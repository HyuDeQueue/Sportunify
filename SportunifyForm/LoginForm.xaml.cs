using Repositories.Models;
using Services.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SportunifyForm
{
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
            UsernameTextBox.Text = "Username";
            PasswordTextBox.Password = "Password";
            UsernameTextBox.Opacity = 0.5;
            PasswordTextBox.Opacity = 0.5;

            UsernameTextBox.KeyDown += TextBox_KeyDown;
            PasswordTextBox.KeyDown += TextBox_KeyDown;
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

            if (username == "Username")
            {
                username = string.Empty;
            }
            if (password == "Password")
            {
                password = string.Empty;
            }

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

        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "Username")
            {
                UsernameTextBox.Text = string.Empty;
                UsernameTextBox.Opacity = 1;
            }
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                UsernameTextBox.Text = "Username";
                UsernameTextBox.Opacity = 0.5;
            }
        }

        private void PasswordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Password == "Password")
            {
                PasswordTextBox.Clear();
                PasswordTextBox.Opacity = 1;
            }
        }

        private void PasswordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {
                PasswordTextBox.Password = "Password";
                PasswordTextBox.Opacity = 0.5;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_Button_Click(sender, new RoutedEventArgs());
            }
        }
    }
}
