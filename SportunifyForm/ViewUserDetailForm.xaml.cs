using Repositories.Models;
using Services.Services;
using System.Windows;

namespace SportunifyForm
{
    /// <summary>
    /// Interaction logic for ViewUserDetailForm.xaml
    /// </summary>
    public partial class ViewUserDetailForm : Window
    {

        private readonly Account _account;
        private readonly SongService _songService = new();
        private readonly PlaylistService _playlistService = new();
        public Account SelectedAccount { get; set; } = null;

        public ViewUserDetailForm(Account account)
        {
            InitializeComponent();
            _account = account;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedAccount != null)
            {
                HelloNameLabel.Content = $"View {SelectedAccount.Username}'s list of songs!";
                try
                {
                    var songs = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));
                    var playlist = await Task.Run(() => _playlistService.GetPlayListByAccountId(_account.AccountId));
                    ViewSongListDataGrid.ItemsSource = songs;
                    ViewPlaylistDetail.ItemsSource = playlist;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching user's songs: {ex.Message}");

                    MessageBox.Show("Unable to retrieve your songs. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    Console.WriteLine($"Error fetching user's playlist: {ex.Message}");

                    MessageBox.Show("Unable to retrieve your playlist. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void ViewPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            ViewMoreDetailForm viewMoreDetailForm = new ViewMoreDetailForm(_account);
            viewMoreDetailForm.Show();
        }

    }
}
