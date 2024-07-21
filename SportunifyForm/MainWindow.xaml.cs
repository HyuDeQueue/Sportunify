using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Repositories.Models;
using Services.Services;
using System.ComponentModel;
using System.Windows.Data;
using System.Threading.Tasks;

namespace SportunifyForm
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Account _account;
        private readonly SongService _songService = new();
        private readonly QueueService _queueService = new();
        private string _currentTempFilePath;
        private bool _isPlaying;

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged(nameof(IsPlaying));
                    UpdatePlayPauseButtonContent();
                }
            }
        }

        public MainWindow(Account account)
        {
            InitializeComponent();
            DataContext = this;
            _account = account;
            mediaPlayer.LoadedBehavior = MediaState.Manual; // Ensure manual control of the media element
            mediaPlayer.UnloadedBehavior = MediaState.Manual;
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsPlaying)
            {
                // Pause the media
                mediaPlayer.Pause();
            }
            else
            {
                // Play the media
                mediaPlayer.Play();
            }

            // Toggle the IsPlaying property
            IsPlaying = !IsPlaying;
        }

        private void UpdatePlayPauseButtonContent()
        {
            var playPauseTextBlock = (TextBlock)PlayPauseButton.Template.FindName("PlayPauseTextBlock", PlayPauseButton);
            if (playPauseTextBlock != null)
            {
                playPauseTextBlock.Text = IsPlaying ? "⏸" : "▶️";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            OnSongFinishedPlaying();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            IsPlaying = false;
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            _queueService.ShuffleQueue();
            UpdateQueueDataGrid();
            PlayNextSongInQueue();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            AddSongDetail detail = new(_account);
            detail.OnSongDetailClosed += ReloadSongList;
            detail.ShowDialog();
        }

        private async void SongMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HelloNameLabel.Content = $"Hello, {_account.Name}!";
            SongListDataGrid.ItemsSource = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ViewAllUser_Click(object sender, RoutedEventArgs e)
        {
            GetAllUserForm getAllUserForm = new();
            getAllUserForm.ShowDialog();
        }

        private async void YourSongsButton_Click(object sender, RoutedEventArgs e)
        {
            SongListDataGrid.ItemsSource = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));
        }

        private async void AllSongsButton_Click(object sender, RoutedEventArgs e)
        {
            SongListDataGrid.ItemsSource = await Task.Run(() => _songService.GetAllSongs());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginForm loginForm = new();
            loginForm.Show();
            Close();
        }

        private void AddToQueueButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Song selectedSong)
            {
                _queueService.AddSongToQueue(selectedSong);
                UpdateQueueDataGrid();
                MessageBox.Show("Song added to queue: " + selectedSong.Title);
                if (!_queueService.IsPlaying)
                {
                    PlayNextSongInQueue();
                }
            }
        }

        private void UpdateQueueDataGrid()
        {
            QueueDataGrid.ItemsSource = null;
            QueueDataGrid.ItemsSource = _queueService.GetCurrentQueue();
        }

        private void PlayNextSongInQueue()
        {
            if (!_queueService.IsPlaying && _queueService.GetCurrentQueue().Any())
            {
                _queueService.PlayQueue();
                var currentSong = _queueService.GetCurrentSong();
                if (currentSong != null)
                {
                    NowPlayingTextBox.Text = "Now Playing: " + currentSong.Title;
                    PlaySongFromBytes(currentSong.SongMedia);
                }
                UpdateQueueDataGrid();
            }
        }

        private void PlaySongFromBytes(byte[] songBytes)
        {
            try
            {
                // Delete previous temp file if exists
                if (!string.IsNullOrEmpty(_currentTempFilePath) && File.Exists(_currentTempFilePath))
                {
                    try
                    {
                        File.Delete(_currentTempFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to delete temp file: " + ex.Message);
                    }
                }

                // Write byte array to temp file
                _currentTempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");
                File.WriteAllBytes(_currentTempFilePath, songBytes);

                // Check if temp file exists and can be read
                if (!File.Exists(_currentTempFilePath))
                {
                    MessageBox.Show("Temp file does not exist after creation.");
                    return;
                }

                // Set source for MediaElement and play file
                mediaPlayer.Source = new Uri(_currentTempFilePath, UriKind.Absolute);
                mediaPlayer.Volume = 0.5; // Ensure volume is not muted
                mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
                mediaPlayer.MediaFailed += MediaPlayer_MediaFailed; // Handle MediaFailed event
                mediaPlayer.Play();
                IsPlaying = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in PlaySongFromBytes: " + ex.Message);
            }
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            OnSongFinishedPlaying();
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("MediaFailed event triggered: " + e.ErrorException.Message);
        }

        private void OnSongFinishedPlaying()
        {
            _queueService.SkipSongInQueue();
            if (_queueService.GetCurrentSong() != null)
            {
                NowPlayingTextBox.Text = "Now Playing: " + _queueService.GetCurrentSong().Title;
                PlaySongFromBytes(_queueService.GetCurrentSong().SongMedia);
            }
            else
            {
                NowPlayingTextBox.Text = "Now Playing: ";
                mediaPlayer.Stop();
                IsPlaying = false;
            }
            UpdateQueueDataGrid();
            if (_queueService.GetCurrentQueue().Any())
            {
                PlayNextSongInQueue();
            }
        }

        private async void ReloadSongList()
        {
            SongListDataGrid.ItemsSource = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));
        }
    }
}
