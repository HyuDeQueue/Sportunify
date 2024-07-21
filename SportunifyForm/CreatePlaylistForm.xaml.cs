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
    /// Interaction logic for CreatePlaylistForm.xaml
    /// </summary>
    public partial class CreatePlaylistForm : Window
    {
        public Account user;
        private PlaylistService playlistService = new();

        public CreatePlaylistForm()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to create this playlist?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                string playlistName = PlaylistNameTextBox.Text;
                string playlistDescription = PlaylistDescriptionTextBox.Text;
                Playlist newPlaylist = new Playlist() { PlaylistName = playlistName, Description = playlistDescription, AccountId = user.AccountId };
                playlistService.AddPlaylist(newPlaylist);
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
