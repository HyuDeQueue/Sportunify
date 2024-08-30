using Repositories.Models;
using Services.Services;
using System.Windows;

namespace SportunifyForm
{
    /// <summary>
    /// Interaction logic for ViewMoreDetailForm.xaml
    /// </summary>
    public partial class ViewMoreDetailForm : Window
    {

        private readonly Account _account;
        private readonly SongService _songService = new();

        public ViewMoreDetailForm(Account account)
        {
            InitializeComponent();
            _account = account;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HelloNameLabel.Content = $"View {_account.Username}'s list of songs!";
            try
            {
                var songs = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));
                ViewSongListDataGrid.ItemsSource = songs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user's songs: {ex.Message}");

                MessageBox.Show("Unable to retrieve your songs. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
