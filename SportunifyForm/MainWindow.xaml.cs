using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using NAudio.Wave;
using Repositories.Models;
using Services.Services;

namespace SportunifyForm
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Account _account;
        private readonly SongService _songService = new();
        public readonly QueueService _queueService = new();
        private readonly PlaylistService _playlistService = new();
        private IWavePlayer _wavePlayer;
        private AudioFileReader _audioFileReader;
        private string _currentTempFilePath;
        private bool _isPlaying;
        private TimeSpan _totalDuration;
        private DispatcherTimer _timer;
        private readonly object playbackLock = new object();
        private bool _isManualSkip;


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
            _wavePlayer.PlaybackStopped += OnPlaybackStopped;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Set initial volume
            SetVolume(VolumeSlider.Value);
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolume(e.NewValue);
        }

        private void SetVolume(double volume)
        {
            if (_wavePlayer != null && _audioFileReader != null)
            {
                _audioFileReader.Volume = (float)volume;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_audioFileReader != null)
            {
                _totalDuration = _audioFileReader.TotalTime;
                SongProgressBar.Maximum = _totalDuration.TotalSeconds;
                SongProgressBar.Value = _audioFileReader.CurrentTime.TotalSeconds;

                CurrentTimeTextBlock.Text = _audioFileReader.CurrentTime.ToString(@"mm\:ss");

                TimeSpan timeRemaining = _totalDuration - _audioFileReader.CurrentTime;
                TotalTimeTextBlock.Text = timeRemaining.ToString(@"mm\:ss");
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
            lock (playbackLock)
            {
                _isManualSkip = true; 
                StopCurrentSong();
                PlayNextSongInQueue();
            }
        }


        private void PlayNextSongInQueue()
        {
            lock (playbackLock)
            {
                var nextSong = _queueService.SkipSongInQueue();
                if (nextSong != null)
                {
                    NowPlayingTextBox.Text = "Now Playing: " + nextSong.Title;

                    Task.Delay(1000).Wait();

                    PlaySongFromBytes(nextSong.SongMedia);
                }
                else
                {
                    NowPlayingTextBox.Text = "Now Playing: ";
                    StopPlayback();
                }
                UpdateQueueDataGrid();
            }
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            lock (playbackLock)
            {
                _queueService.ShuffleQueue();
                UpdateQueueDataGrid();
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            AddSongDetail detail = new(_account);
            detail.OnSongDetailClosed += ReloadSongList;
            detail.ShowDialog();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Song selected = SongListDataGrid.SelectedItem as Song;

            if (selected == null)
            {
                MessageBox.Show("Please select a Song before updating!", "Select One", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            AddSongDetail detail = new(_account);
            detail.SelectedSong = selected;
            detail.ShowDialog();
            ReloadSongList();
        }

        private async void SongMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HelloNameLabel.Content = $"Hello, {_account.Name}!";

            try
            {
                var songs = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));
                SongListDataGrid.ItemsSource = songs;
                var playlists = await Task.Run(() => _playlistService.GetPlayListByAccountId(_account.AccountId));
                PlaylistDataGrid.ItemsSource = playlists;
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
            PlayNextSongInQueue();
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            _totalDuration = _audioFileReader.TotalTime;
            TotalTimeTextBlock.Text = _totalDuration.ToString(@"mm\:ss");
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
                var playlists = await Task.Run(() => _playlistService.GetAllPlaylists());
                SongListDataGrid.ItemsSource = songs;
                PlaylistDataGrid.ItemsSource = playlists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user's songs: {ex.Message}");

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
                Console.WriteLine($"Error fetching all songs: {ex.Message}");

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

        private void PlaySongFromBytes(byte[] songBytes)
        {
            lock (playbackLock)
            {
                try
                {
                    if (!string.IsNullOrEmpty(_currentTempFilePath) && File.Exists(_currentTempFilePath))
                    {
                        File.Delete(_currentTempFilePath);
                    }

                    _currentTempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".mp3");
                    File.WriteAllBytes(_currentTempFilePath, songBytes);

                    // Giải phóng tài nguyên trước khi khởi tạo lại
                    DisposeWavePlayer();

                    _audioFileReader = new AudioFileReader(_currentTempFilePath);
                    _wavePlayer = new WaveOutEvent();
                    _wavePlayer.Init(_audioFileReader);

                    _wavePlayer.PlaybackStopped -= OnPlaybackStopped;
                    _wavePlayer.PlaybackStopped += OnPlaybackStopped;

                    _wavePlayer.Play();
                    IsPlaying = true;

                    var currentSong = _queueService.GetCurrentSong();
                    if (currentSong != null)
                    {
                        SongInfoTextBlock.Text = $"{currentSong.Title} - {currentSong.ArtistName}";
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("Lỗi truy cập bị từ chối: " + ex.Message, "Lỗi");
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Show("Không tìm thấy tệp: " + ex.Message, "Lỗi");
                }
                catch (COMException ex)
                {
                    MessageBox.Show("Lỗi COM: " + ex.Message, "Lỗi");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi trong PlaySongFromBytes: " + ex.Message, "Lỗi");
                }
            }
        }

        private void DisposeWavePlayer()
        {
            if (_wavePlayer != null)
            {
                _wavePlayer.Stop();
                _wavePlayer.Dispose();
                _wavePlayer = null;
            }

            if (_audioFileReader != null)
            {
                _audioFileReader.Dispose();
                _audioFileReader = null;
            }
        }

        private void StopCurrentSong()
        {
            lock (playbackLock)
            {
                try
                {
                    DisposeWavePlayer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi trong StopCurrentSong: " + ex.Message, "Lỗi");
                }
            }
        }

        private void StopPlayback()
        {
            lock (playbackLock)
            {
                try
                {
                    DisposeWavePlayer();

                    _wavePlayer = new WaveOutEvent();
                    _wavePlayer.PlaybackStopped += OnPlaybackStopped;

                    IsPlaying = false;
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("Lỗi truy cập bị từ chối: " + ex.Message, "Lỗi");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi trong StopPlayback: " + ex.Message, "Lỗi");
                }
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

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            lock (playbackLock)
            {
                if (e.Exception != null)
                {
                    MessageBox.Show("Phát lại dừng do lỗi: " + e.Exception.Message, "Lỗi");
                }
                else if (!_isManualSkip) 
                {
                    PlayNextSongInQueue();
                }

                _isManualSkip = false; 
            }
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ReloadSongList()
        {
            SongListDataGrid.ItemsSource = await Task.Run(() => _songService.GetSongsFromAccount(_account.AccountId));
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string songName = SongNameTextBox.Text;
            string artistName = ArtistTextBox.Text;

            var searchResults = await Task.Run(() => _songService.SearchSongByNameOrArtist(songName, artistName));

            SongListDataGrid.ItemsSource = searchResults;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Song selected = SongListDataGrid.SelectedItem as Song;

            if (selected == null)
            {
                MessageBox.Show("Please select a Song before deleting!", "Select One", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            AddSongDetail detail = new(_account);
            detail.SelectedSong = selected;

            MessageBoxResult result = MessageBox.Show($"Do you really want to delete {selected.Title}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _songService.DeleteSong(selected);
                ReloadSongList();
                MessageBox.Show($"Deleted {selected.Title}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Cancelled!", "Cancel", MessageBoxButton.OK, MessageBoxImage.Question);
            }
        }

        public void ViewPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Playlist selectedPlaylist)
            {
                PlaylistDetailForm form = new();
                form.user = _account;
                form.playlist = selectedPlaylist;
                form.mainWindow = this;
                this.Visibility = System.Windows.Visibility.Hidden;
                form.ShowDialog();
                this.Visibility = System.Windows.Visibility.Visible;
                this.UpdateQueueDataGrid();
                if (!_queueService.IsPlaying)
                {
                    if(_queueService.GetCurrentQueue().Count > 0)
                        PlayNextSongInQueue();
                }
            }
                
        }

        public void UpdatePlaylistDataGrid()
        {
            PlaylistDataGrid.ItemsSource = null;
            PlaylistDataGrid.ItemsSource = _playlistService.GetAllPlaylists();
        }

        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            CreatePlaylistForm form = new();
            form.user = _account;
            form.ShowDialog();
            PlaylistDataGrid.ItemsSource = null;
            PlaylistDataGrid.ItemsSource = _playlistService.GetAllPlaylists();
        }

        private void AddToPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button && button.Tag is Song selectedSong)
            {
                AddSongToPlaylist form = new();
                form.user = _account;
                form.selectedSong = selectedSong;
                form.ShowDialog();
                UpdatePlaylistDataGrid();
            }
        }

        private async void YourPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var playlists = await Task.Run(() => _playlistService.GetPlayListByAccountId(_account.AccountId));
                PlaylistDataGrid.ItemsSource = playlists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all songs: {ex.Message}");

                MessageBox.Show("Unable to retrieve all songs. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AllPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var playlists = await Task.Run(() => _playlistService.GetAllPlaylists());
                PlaylistDataGrid.ItemsSource = playlists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all playlists: {ex.Message}");

                MessageBox.Show("Unable to retrieve all playlists. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
