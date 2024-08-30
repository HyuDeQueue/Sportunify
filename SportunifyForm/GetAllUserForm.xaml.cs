using Repositories.Models;
using Services.Services;
using System.Windows;

namespace SportunifyForm
{
    /// <summary>
    /// Interaction logic for GetAllUser.xaml
    /// </summary>
    public partial class GetAllUserForm : Window
    {
        private AccountService _service = new AccountService();

        public GetAllUserForm()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewAllUserList.ItemsSource = null;
            ViewAllUserList.ItemsSource = _service.GetAllAccounts();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ViewDetail_Click(object sender, RoutedEventArgs e)
        {
            Account selected = ViewAllUserList.SelectedItem as Account;
            if (selected == null)
            {
                MessageBox.Show("Please select an account before viewing their songs!", "Select One", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            ViewUserDetailForm frm = new ViewUserDetailForm(selected);
            frm.SelectedAccount = selected;
            frm.ShowDialog();
            ViewAllUserList.ItemsSource = null;
            ViewAllUserList.ItemsSource = _service.GetAllAccounts();
        }
    }
}
