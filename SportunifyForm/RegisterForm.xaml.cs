using Repositories.Models;
using Services.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SportunifyForm
{
    /// <summary>
    /// Interaction logic for RegisterForm.xaml
    /// </summary>
    public partial class RegisterForm : Window
    {
        private readonly AccountService _accountService = new();

        public RegisterForm()
        {
            InitializeComponent();
        }

        private async void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            var name = NameTextBox.Text.Trim();
            var username = UsernameTextBox.Text.Trim();
            var password = PasswordTextBox.Password;
            var confirmPassword = ConfirmPasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            Account account = new Account
            {
                Name = name,
                Username = username,
                Password = password
            };

            // Show the loading spinner and disable buttons
            LoadingSpinner.Visibility = Visibility.Visible;
            Register_Button.IsEnabled = false;
            Login_Button.IsEnabled = false;
            Close_Button.IsEnabled = false;

            var existingAccount = await Task.Run(() => _accountService.CheckAccountExists(account));
            if (existingAccount != null)
            {
                MessageBox.Show("Username already exists.");
                LoadingSpinner.Visibility = Visibility.Collapsed;
                Register_Button.IsEnabled = true;
                Login_Button.IsEnabled = true;
                Close_Button.IsEnabled = true;
                return;
            }

            await Task.Run(() => _accountService.Register(account));

            // Hide the spinner and re-enable buttons
            LoadingSpinner.Visibility = Visibility.Collapsed;
            Register_Button.IsEnabled = true;
            Login_Button.IsEnabled = true;
            Close_Button.IsEnabled = true;

            // Show success message
            MessageBox.Show("Registration Success!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Open the login form and close the registration form
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
