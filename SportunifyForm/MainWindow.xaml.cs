using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Repositories.Models;
using Services.Services;

namespace SportunifyForm
{
    public partial class MainWindow : Window
    {
        private readonly Account _account;
        private readonly SongService _songService = new();
        private readonly QueueService _queueService = new();
        private string _currentTempFilePath;
        private bool _isPlaying = false;

        public MainWindow(Account account)
        {
            InitializeComponent();
            _account = account;
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                mediaPlayer.Pause();
                PlayPauseButton.Content = "Play";
            }
            else
            {
                mediaPlayer.Play();
                PlayPauseButton.Content = "Pause";
            }
            _isPlaying = !_isPlaying;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            OnSongFinishedPlaying();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            _isPlaying = false;
            PlayPauseButton.Content = "Play";
            NowPlayingTextBox.Text = "Now Playing: ";
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
                // Xóa tệp tạm thời trước đó nếu tồn tại
                if (!string.IsNullOrEmpty(_currentTempFilePath) && File.Exists(_currentTempFilePath))
                {
                    try
                    {
                        File.Delete(_currentTempFilePath);
                    }
                    catch
                    {
                        // Xử lý ngoại lệ nếu tệp đang được sử dụng hoặc không thể xóa
                    }
                }

                // Ghi mảng byte vào tệp tạm thời
                _currentTempFilePath = Path.GetTempFileName();
                File.WriteAllBytes(_currentTempFilePath, songBytes);

                // Đặt nguồn cho MediaElement và phát tệp
                mediaPlayer.Source = new Uri(_currentTempFilePath, UriKind.Absolute);
                mediaPlayer.Play();
                _isPlaying = true;
                PlayPauseButton.Content = "Pause";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in PlaySongFromBytes: " + ex.Message);
            }
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
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
                mediaPlayer.Stop();
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
