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
    /// Interaction logic for PlaylistDetailForm.xaml
    /// </summary>
    public partial class PlaylistDetailForm : Window
    {
        public Playlist playlist;
        public Account user;
        public MainWindow mainWindow;
        private readonly PlaylistService _playlistSerivce = new();
        private readonly QueueService _queueSerivce = new();
        public PlaylistDetailForm()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            CreatePlaylistForm form = new();
            form.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlaylistNameLabel.Content = playlist.PlaylistName;
            SongListDataGrid.ItemsSource = null;
            List<Song> songs = new List<Song>();
            _playlistSerivce.GetAllPlaylistSongs(playlist.PlaylistId).ForEach(song =>
            {
                songs.Add(song.Song);
            });
            SongListDataGrid.ItemsSource = songs;
        }

        private async void AddSongButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var song in (List<Song>)SongListDataGrid.ItemsSource)
            {
                await Task.Run(()=>mainWindow._queueService.AddSongToQueue(song));
            }
            this.Close();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
