using Repositories.Models;
using Services.Services;
using System.Windows.Controls;
using System.Windows;

namespace SportunifyForm
{
    public partial class MainWindow : Window
    {
        private readonly Account _account;
        private SongService songService = new();
        private QueueService queueService = new();

        public MainWindow(Account account)
        {
            InitializeComponent();
            _account = account;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            AddSongDetail detail = new(_account);
            detail.ShowDialog();
        }

        private void SongMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HelloNameLabel.Content = $"Hello, {_account.Name}!";
            SongListDataGrid.ItemsSource = null;
            SongListDataGrid.ItemsSource = songService.GetSongsFromAccount(_account.AccountId);
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ViewAllUser_Click(object sender, RoutedEventArgs e)
        {
            GetAllUserForm getAllUserForm = new GetAllUserForm();
            getAllUserForm.ShowDialog();
        }

        private void YourSongsButton_Click(object sender, RoutedEventArgs e)
        {
            SongListDataGrid.ItemsSource = null;
            SongListDataGrid.ItemsSource = songService.GetSongsFromAccount(_account.AccountId);
        }

        private void AllSongsButton_Click(object sender, RoutedEventArgs e)
        {
            SongListDataGrid.ItemsSource = null;
            SongListDataGrid.ItemsSource = songService.GetAllSongs();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void AddToQueueButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Song selectedSong = button.Tag as Song;
            queueService.AddSongToQueue(selectedSong);
            UpdateQueueDataGrid();
            PlayNextSongInQueue();
        }

        private void UpdateQueueDataGrid()
        {
            QueueDataGrid.ItemsSource = null;
            QueueDataGrid.ItemsSource = queueService.GetCurrentQueue();
        }

        private void PlayNextSongInQueue()
        {
            if (!queueService.IsPlaying && queueService.GetCurrentQueue().Any())
            {
                queueService.PlayQueue();
                NowPlayingTextBox.Text = "Now Playing: " + queueService.GetCurrentSong().Title;
                UpdateQueueDataGrid();
            }
        }

        private void OnSongFinishedPlaying()
        {
            queueService.SkipSongInQueue();
            if (queueService.GetCurrentSong() != null)
            {
                NowPlayingTextBox.Text = "Now Playing: " + queueService.GetCurrentSong().Title;
            }
            else
            {
                NowPlayingTextBox.Text = "Now Playing: ";
            }
            UpdateQueueDataGrid();
            if (queueService.GetCurrentQueue().Any())
            {
                PlayNextSongInQueue();
            }
        }
    }
}
