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
    /// Interaction logic for AddSongToPlaylist.xaml
    /// </summary>
    public partial class AddSongToPlaylist : Window
    {
        public Account user;
        public Song selectedSong;
        private readonly PlaylistService playlistService = new();
        public AddSongToPlaylist()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string name = selectedSong.Title;
            string author = selectedSong.ArtistName;
            string category = selectedSong.Category.CategoryName;
            SongNameTextBox.Text = name;
            AuthorTextBox.Text = author;
            CategoryTextBox.Text = category;
            PlaylistComboBoxLoad();
        }

        private void PlaylistComboBoxLoad()
        {
            PlaylistIdComboBox.ItemsSource = playlistService.GetAllPlaylists();
            PlaylistIdComboBox.DisplayMemberPath = "PlaylistName";
            PlaylistIdComboBox.SelectedValuePath = "PlaylistId";

            PlaylistIdComboBox.SelectedValue = 1;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Playlist playlist = playlistService.GetSelectedPlaylist(int.Parse(PlaylistIdComboBox.SelectedValue.ToString()));

            int position = playlistService.GetAllPlaylistSongs(playlist.PlaylistId).Capacity + 1;

            List<PlaylistSong> existedSong = playlistService.GetAllPlaylistSongs(playlist.PlaylistId).Where(x => x.SongId == selectedSong.SongId).ToList();
            if(existedSong.Count > 0 )
            {
                MessageBox.Show("This song is already in this playlist", "Existed", MessageBoxButton.OK, MessageBoxImage.Information);
            } 
            else 
            {
                playlistService.AddSongToPlaylist(playlist.PlaylistId, selectedSong.SongId, position);
                this.Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
