using Repositories.Models;
using Services.Services;
using System.Security.Principal;
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

namespace SportunifyForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Account _account;
        private SongService songService = new();

        public MainWindow(Account account)
        {
            InitializeComponent();
            _account = account;

        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            AddSongDetail detail = new(_account);
            detail.ShowDialog();
        }
        private void SongMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HelloNameLabel.Content = $"Hello, {_account.Name}!";
            SongListDataGrid.ItemsSource = null;
            SongListDataGrid.ItemsSource = songService.GetSongsFromAccount(_account.AccountId);
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ViewAllUser_Click(object sender, RoutedEventArgs e)
        {
            GetAllUserForm getAllUserForm = new GetAllUserForm();   
            getAllUserForm.ShowDialog();
        }
        private void YourSongsButton_Click(object sender, RoutedEventArgs e)
        {
            SongListDataGrid.ItemsSource = null;
            SongListDataGrid.ItemsSource = songService.GetSongsFromAccount(_account.AccountId);
        }

        private void AllSongsButton_Click(object sender, RoutedEventArgs e)
        {
            SongListDataGrid.ItemsSource = null;
            SongListDataGrid.ItemsSource = songService.GetAllSongs();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}