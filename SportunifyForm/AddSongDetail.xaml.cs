using Microsoft.Win32;
using NAudio.Wave;
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
    public partial class AddSongDetail : Window
    {
        private Account _account;
        private DispatcherTimer timer;
        private TimeSpan songDuration;
        private TimeSpan currentTime;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private string fileName;
        private bool isPlaying = false;
        private CategoryService categoryService = new();
        public Song SelectedSong { get; set; } = null;
        SongService songService = new();

        public event Action OnSongDetailClosed;

        public AddSongDetail(Account account)
        {
            InitializeComponent();
            InitializeTimer();
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
            if (audioFile != null)
            {
                songDuration = audioFile.TotalTime;
                currentTime = audioFile.CurrentTime;
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

                // Dispose of existing resources if needed
                if (audioFile != null)
                {
                    audioFile.Dispose();
                }

                if (outputDevice != null)
                {
                    outputDevice.Dispose();
                }

                // Initialize new audio file and output device
                audioFile = new AudioFileReader(fileName);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
            }
        }

        private void BT_Click_PlayPause(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(fileName) && audioFile != null)
            {
                if (isPlaying)
                {
                    outputDevice.Pause();
                    isPlaying = false;
                    PlayButton.Content = "▶️"; // Change to play icon
                    timer.Stop();
                }
                else
                {
                    outputDevice.Play();
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string songName = SongNameTextBox.Text;
            string author = AuthorTextBox.Text;
            int categoryId = (int)SongCategoryIdComboBox.SelectedValue;
            
            byte[] songMedia = FileToByteArray(fileName);

            // Disable buttons and show loading animation
            SaveButton.IsEnabled = false;
            Close.IsEnabled = false;
            PlayButton.IsEnabled = false;
            LoadingSpinner.Visibility = Visibility.Visible;



            Song newSong = new Song
            {
                Title = songName,
                ArtistName = author,
                SongMedia = songMedia,
                AccountId = _account.AccountId,
                CategoryId = categoryId
            };

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
                this.Close();
            }
        }

        private void ClearForm()
        {
            SongNameTextBox.Clear();
            AuthorTextBox.Clear();
            SongCategoryIdComboBox.SelectedIndex = -1;
            FileName.Text = string.Empty;

            fileName = string.Empty;

            TimelineProgressBar.Value = 0;
            ElapsedTimeTextBlock.Text = "00:00";
            RemainingTimeTextBlock.Text = "00:00";
            outputDevice.Stop();
            isPlaying = false;
            PlayButton.Content = "▶️"; 
            timer.Stop();
        }


        public byte[] FileToByteArray(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            // Stop playback if it's currently playing
            if (isPlaying)
            {
                outputDevice.Stop();
                isPlaying = false;
                PlayButton.Content = "▶️"; // Change to play icon
                timer.Stop();
            }

            // Dispose of the audio file and output device if they exist
            audioFile?.Dispose();
            outputDevice?.Dispose();

            // Close the form
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Ensure resources are disposed of when the window is closed
            if (isPlaying)
            {
                outputDevice.Stop();
                isPlaying = false;
                PlayButton.Content = "▶️"; // Change to play icon
                timer.Stop();
            }

            // Dispose of the audio file and output device if they exist
            audioFile?.Dispose();
            outputDevice?.Dispose();
            OnSongDetailClosed?.Invoke();
            base.OnClosed(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SongLabel.Content = "Create a Song";
            SongCategoryIdComboBox.ItemsSource = categoryService.GetAllCategories();
            SongCategoryIdComboBox.DisplayMemberPath = "CategoryName";
            SongCategoryIdComboBox.SelectedValuePath = "CategoryId";

            SongCategoryIdComboBox.SelectedValue = 1;

            if (SelectedSong != null)
            {
                SongLabel.Content = "Update a Song";
                SongNameTextBox.Text = SelectedSong.Title.ToString();
                AuthorTextBox.Text = SelectedSong.ArtistName.ToString();
                SongCategoryIdComboBox.SelectedValue = SelectedSong.CategoryId.ToString();
            }
        }
    }
}
