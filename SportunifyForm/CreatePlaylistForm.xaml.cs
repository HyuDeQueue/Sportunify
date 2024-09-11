using Repositories.Models;
using Services.Services;
using System.Windows;

namespace SportunifyForm
{
    /// <summary>
    /// Interaction logic for CreatePlaylistForm.xaml
    /// </summary>
    public partial class CreatePlaylistForm : Window
    {
        public Account user;
        public Playlist playlist;
        private PlaylistService playlistService = new();
        private SongService songService = new();

        public CreatePlaylistForm()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to create this playlist?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                string playlistName = PlaylistNameTextBox.Text;
                string playlistDescription = PlaylistDescriptionTextBox.Text;
                Playlist newPlaylist = new Playlist() { PlaylistName = playlistName, Description = playlistDescription, AccountId = user.AccountId };
                playlistService.AddPlaylist(newPlaylist);
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        //private async void LoadNonPlaylistSongs()
        //{
        //    List<PlaylistSong> songsInPlaylist = await Task.Run(() => playlistService.GetAllPlaylistSongs(playlist.PlaylistId));
        //    List<Song> songs = new List<Song>();
        //    songsInPlaylist.ForEach(song =>
        //    {
        //        songs.Add(song.Song);
        //    });
        //    SongListDataGrid.ItemsSource = null;
        //    SongListDataGrid.ItemsSource = songService.GetAllSongs().Except(songs).ToList();
        //}

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //private void AddToPlaylistButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button button && button.Tag is Song selectedSong)
        //    {
        //        Song song = (Song)button.Tag;
        //        playlistService.AddSongToPlaylist()
        //        Window_Loaded(sender, e);
        //    }
        //}
    }
}
