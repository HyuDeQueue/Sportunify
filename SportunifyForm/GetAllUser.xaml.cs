using Repositories.Repositories;
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
    /// Interaction logic for GetAllUser.xaml
    /// </summary>
    public partial class GetAllUser : Window
    {
        private AccountService _service = new AccountService();

        public GetAllUser()
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
    }
}
