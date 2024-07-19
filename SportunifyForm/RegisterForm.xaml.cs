using Repositories.Models;
using Services.Services;
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

        private void Register_Button_Click(object sender, RoutedEventArgs e)
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

            Account account = new Account();
            account.Name = name;
            account.Username = username;
            account.Password = password;

            var existingAccount = _accountService.CheckAccountExists(account);
            if (existingAccount != null)
            {
                MessageBox.Show("Username already exists.");
                return;
            }

            _accountService.Register(account);

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
