using Microsoft.Win32;
using NAudio.Wave;
using Repositories.Models;
using Repositories.Repositories;
using Services.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
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
        private Song SelectedSong { get; set; } = null;

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
            // Example save functionality
            LoadingSpinner.Visibility = Visibility.Visible;
            await Task.Run(() =>
            {
                // Simulate a long-running save operation
                Task.Delay(2000).Wait();
            });
            LoadingSpinner.Visibility = Visibility.Collapsed;

            MessageBox.Show("Song details saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

            base.OnClosed(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Example initialization, e.g., loading categories
            // SongCategoryIdComboBox.ItemsSource = categoryService.GetCategories();
        }
    }
}
