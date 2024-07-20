using Microsoft.Win32;
using Repositories.Models;
using Repositories.Repositories;
using Services.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SportunifyForm
{
    /// <summary>
    /// Interaction logic for AddSongDetail.xaml
    /// </summary>
    public partial class AddSongDetail : Window
    {
        private Account _account;
        private DispatcherTimer timer;
        private TimeSpan songDuration;
        private TimeSpan currentTime;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private string fileName;
        private bool isPlaying = false;
        private CategoryService categoryService = new();
        private Song SelectedSong { get; set; } = null;

        public AddSongDetail(Account account)
        {
            InitializeComponent();
            InitializeTimer();
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            _account = account;
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                songDuration = mediaPlayer.NaturalDuration.TimeSpan;
                currentTime = mediaPlayer.Position;
                if (currentTime < songDuration)
                {
                    TimelineProgressBar.Maximum = songDuration.TotalSeconds;
                    TimelineProgressBar.Value = currentTime.TotalSeconds;
                    ElapsedTimeTextBlock.Text = currentTime.ToString(@"mm\:ss");
                    RemainingTimeTextBlock.Text = (songDuration - currentTime).ToString(@"mm\:ss");
                }
                else
                {
                    timer.Stop();
                    isPlaying = false;
                    PlayButton.Content = "▶️"; // Change back to play icon
                }
            }
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            mediaPlayer.Position = TimeSpan.Zero; // Reset position to start
            timer.Stop();
            isPlaying = false;
            PlayButton.Content = "▶️"; // Change back to play icon
            mediaPlayer.Stop(); // 
        }

        private void BT_Click_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = ".mp3",
                Filter = "Audio Files (*.mp3;*.wav)|*.mp3;*.wav"
            };
            bool? dialogOK = fileDialog.ShowDialog();
            if (dialogOK == true)
            {
                fileName = fileDialog.FileName;
                FileName.Text = fileDialog.SafeFileName;
                mediaPlayer.Open(new Uri(fileName));
            }
        }

        private void BT_Click_PlayPause(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (isPlaying)
                {
                    mediaPlayer.Pause();
                    isPlaying = false;
                    PlayButton.Content = "▶️"; // Change to play icon
                    timer.Stop();
                }
                else
                {
                    mediaPlayer.Play();
                    isPlaying = true;
                    PlayButton.Content = "⏸"; // Change to pause icon
                    timer.Start();
                }
            }
            else
            {
                MessageBox.Show("Please open a song file first.", "No File Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            this.Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string songName = SongNameTextBox.Text;
            string author = AuthorTextBox.Text;
            int? categoryId = SongCategoryIdComboBox.SelectedValue as int?;
            string fileName = FileName.Text;

            // Validate form fields
            if (string.IsNullOrEmpty(songName))
            {
                MessageBox.Show("Please enter a song name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(author))
            {
                MessageBox.Show("Please enter the author's name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!categoryId.HasValue)
            {
                MessageBox.Show("Please select a song category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Please open a song file.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            byte[] songMedia = FileToByteArray(fileName);

            Song newSong = new Song
            {
                Title = songName,
                ArtistName = author,
                SongMedia = songMedia,
                AccountId = _account.AccountId, // Assuming a static AccountId for now
                CategoryId = categoryId.Value
            };

            // Disable buttons and show loading animation
            SaveButton.IsEnabled = false;
            Close.IsEnabled = false;
            PlayButton.IsEnabled = false;
            LoadingSpinner.Visibility = Visibility.Visible;

            try
            {
                SongService songService = new SongService();
                await Task.Run(() => songService.AddSongs(newSong));

                MessageBox.Show("Song saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save song: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Re-enable buttons and hide loading animation
                SaveButton.IsEnabled = true;
                Close.IsEnabled = true;
                PlayButton.IsEnabled = true;
                LoadingSpinner.Visibility = Visibility.Collapsed;
            }
        }


        private void ClearForm()
        {
            SongNameTextBox.Clear();
            AuthorTextBox.Clear();
            SongCategoryIdComboBox.SelectedIndex = -1;
            FileName.Text = string.Empty;

            mediaPlayer.Stop();
            mediaPlayer.Close();
            fileName = string.Empty;

            TimelineProgressBar.Value = 0;
            ElapsedTimeTextBlock.Text = "00:00";
            RemainingTimeTextBlock.Text = "00:00";
            PlayButton.Content = "▶️";
            isPlaying = false;
            timer.Stop();
        }

        public byte[] FileToByteArray(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SongCategoryIdComboBox.ItemsSource = categoryService.GetAllCategories();
            SongCategoryIdComboBox.DisplayMemberPath = "CategoryName";
            SongCategoryIdComboBox.SelectedValuePath = "CategoryId";

            SongCategoryIdComboBox.SelectedValue = "1";

            SongLabel.Content = "Create a Song";

            if (SelectedSong != null)
            {
                SongLabel.Content = "Update a Song";
                SongNameTextBox.Text = SelectedSong.Title;
                AuthorTextBox.Text = SelectedSong.ArtistName;
                SongCategoryIdComboBox.SelectedValue = SelectedSong.CategoryId;
                FileName.Text = SelectedSong.SongMedia.ToString();
             }
        }
    }
}

