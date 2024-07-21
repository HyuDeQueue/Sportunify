﻿using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NAudio.Wave;
using Repositories.Models;
using Services.Services;

namespace SportunifyForm
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Account _account;
        private readonly SongService _songService = new();
        private readonly QueueService _queueService = new();
        private IWavePlayer _wavePlayer;
        private AudioFileReader _audioFileReader;
        private string _currentTempFilePath;
        private bool _isPlaying;
        private TimeSpan _totalDuration;

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

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow(Account account)
        {
            InitializeComponent();
            DataContext = this;
            _account = account;
            _wavePlayer = new WaveOutEvent();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                _totalDuration = mediaPlayer.NaturalDuration.TimeSpan;
                SongProgressBar.Maximum = _totalDuration.TotalSeconds;
                SongProgressBar.Value = mediaPlayer.Position.TotalSeconds;

                CurrentTimeTextBlock.Text = mediaPlayer.Position.ToString(@"mm\:ss");
                TotalTimeTextBlock.Text = _totalDuration.ToString(@"mm\:ss");
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_queueService.GetCurrentQueue().Any())
            {
                MessageBox.Show("Please select a song before playing.", "No Song Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsPlaying)
            {
                _wavePlayer.Pause();
            }
            else
            {
                _wavePlayer.Play();
            }

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

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            OnSongFinishedPlaying();
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
            //SongListDataGrid.ItemsSource = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));

            try
            {
                var songs = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));
                SongListDataGrid.ItemsSource = songs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user's songs: {ex.Message}");

                MessageBox.Show("Unable to retrieve your songs. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {

        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {

        }

        private void ViewAllUser_Click(object sender, RoutedEventArgs e)
        {
            GetAllUserForm getAllUserForm = new();
            getAllUserForm.ShowDialog();
        }

        private async void YourSongsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var songs = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));
                SongListDataGrid.ItemsSource = songs;
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error fetching user's songs: {ex.Message}");

                // Notify the user
                MessageBox.Show("Unable to retrieve your songs. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AllSongsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var songs = await Task.Run(() => _songService.GetAllSongs());
                SongListDataGrid.ItemsSource = songs;
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error fetching all songs: {ex.Message}");

                // Notify the user
                MessageBox.Show("Unable to retrieve all songs. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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


        private void RemoveFromQueueButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Song selectedSong)
            {
                _queueService.RemoveSongFromQueue(selectedSong);
                UpdateQueueDataGrid();
            }
        }

        private void MoveUpQueueButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Song selectedSong)
            {
                _queueService.MoveSongUpInQueue(selectedSong);
                UpdateQueueDataGrid();
            }
        }

        private void MoveDownQueueButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Song selectedSong)
            {
                _queueService.MoveSongDownInQueue(selectedSong);
                UpdateQueueDataGrid();
            }
        }
        private void PlaySongFromBytes(byte[] songBytes)
        {
            try
            {
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

                _currentTempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp3");
                File.WriteAllBytes(_currentTempFilePath, songBytes);

                if (_audioFileReader != null)
                {
                    _audioFileReader.Dispose();
                }

                if (_wavePlayer.PlaybackState == PlaybackState.Playing)
                {
                    _wavePlayer.Stop();
                }

                _audioFileReader = new AudioFileReader(_currentTempFilePath);
                _wavePlayer.Init(_audioFileReader);
                _wavePlayer.PlaybackStopped += OnPlaybackStopped;
                _wavePlayer.Play();
                IsPlaying = true;

                var currentSong = _queueService.GetCurrentSong();
                if (currentSong != null)
                {
                    SongInfoTextBlock.Text = $"{currentSong.Title} - {currentSong.ArtistName}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in PlaySongFromBytes: " + ex.Message);
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            OnSongFinishedPlaying();
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
                _wavePlayer.Stop();
                IsPlaying = false;
            }
            UpdateQueueDataGrid();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ReloadSongList()
        {
            SongListDataGrid.ItemsSource = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));
        }
    }
}
