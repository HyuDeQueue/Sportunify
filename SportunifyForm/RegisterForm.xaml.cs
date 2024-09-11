using Repositories.Models;
using Services.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SportunifyForm
{
    public partial class RegisterForm : Window
    {
        private readonly AccountService _accountService = new();

        public RegisterForm()
        {
            InitializeComponent();
            InitializePlaceholders();

            NameTextBox.KeyDown += TextBox_KeyDown;
            UsernameTextBox.KeyDown += TextBox_KeyDown;
            PasswordTextBox.KeyDown += PasswordBox_KeyDown;
            ConfirmPasswordTextBox.KeyDown += PasswordBox_KeyDown;
        }

        private void InitializePlaceholders()
        {
            NameTextBox.Text = "Name";
            UsernameTextBox.Text = "Username";
            PasswordTextBox.Password = "Password";
            ConfirmPasswordTextBox.Password = "Password";

            NameTextBox.Opacity = 0.5;
            UsernameTextBox.Opacity = 0.5;
            PasswordTextBox.Opacity = 0.5;
            ConfirmPasswordTextBox.Opacity = 0.5;
        }

        private async void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            var name = NameTextBox.Text.Trim();
            var username = UsernameTextBox.Text.Trim();
            var password = PasswordTextBox.Password;
            var confirmPassword = ConfirmPasswordTextBox.Password;

            if (name == "Name") name = string.Empty;
            if (username == "Username") username = string.Empty;
            if (password == "Password") password = string.Empty;
            if (confirmPassword == "Password") confirmPassword = string.Empty;

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (password.Length < 5)
            {
                MessageBox.Show("Passwords must have at least 5 characters!");
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

            LoadingSpinner.Visibility = Visibility.Collapsed;
            Register_Button.IsEnabled = true;
            Login_Button.IsEnabled = true;
            Close_Button.IsEnabled = true;

            MessageBox.Show("Registration Success!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            Close();
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            Close();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "Name" || textBox.Text == "Username"))
            {
                textBox.Text = "";
                textBox.Opacity = 1;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "NameTextBox")
                {
                    textBox.Text = "Name";
                }
                else if (textBox.Name == "UsernameTextBox")
                {
                    textBox.Text = "Username";
                }
                textBox.Opacity = 0.5;
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null && (passwordBox.Password == "Password" || passwordBox.Password == "Confirm Password"))
            {
                passwordBox.Clear();
                passwordBox.Opacity = 1;
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            if (passwordBox != null && string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                if (passwordBox.Name == "PasswordTextBox")
                {
                    passwordBox.Password = "Password";
                }
                else if (passwordBox.Name == "ConfirmPasswordTextBox")
                {
                    passwordBox.Password = "Confirm Password";
                }
                passwordBox.Opacity = 0.5;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Register_Button_Click(sender, new RoutedEventArgs());
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Register_Button_Click(sender, new RoutedEventArgs());
            }
        }
    }
}
