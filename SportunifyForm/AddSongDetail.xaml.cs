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
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace SportunifyForm
{
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

        private static IAmazonS3 s3Client;
        private static AwsSettings awsSettings;

        public AddSongDetail(Account account)
        {
            InitializeComponent();
            InitializeTimer();
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            _account = account;

            LoadAwsSettings();
            InitializeS3Client();
        }

        private void LoadAwsSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            awsSettings = new AwsSettings();
            configuration.GetSection("AWS").Bind(awsSettings);
        }

        private void InitializeS3Client()
        {
            s3Client = new AmazonS3Client(
                awsSettings.AccessKeyId,
                awsSettings.SecretAccessKey,
                Amazon.RegionEndpoint.GetBySystemName(awsSettings.Region)
            );
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
            mediaPlayer.Stop();                           // 
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
            int categoryId = (int)SongCategoryIdComboBox.SelectedValue;

            byte[] songMedia = FileToByteArray(fileName);

            // Upload song to S3 and get the key name
            string s3Key = await UploadFileToS3Async(songMedia, Path.GetFileName(fileName));

            Song newSong = new Song
            {
                Title = songName,
                ArtistName = author,
                SongMedia = s3Key, // Store the S3 key name
                AccountId = _account.AccountId,
                CategoryId = categoryId
            };

            // Create an instance of the service and add the song
            SongService songService = new SongService();
            songService.AddSongs(newSong);

            MessageBox.Show("Song saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            ClearForm();
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

        private async Task<string> UploadFileToS3Async(byte[] fileBytes, string fileName)
        {
            string keyName = Guid.NewGuid() + "_" + fileName;
            using (var stream = new MemoryStream(fileBytes))
            {
                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.UploadAsync(stream, awsSettings.BucketName, keyName);
            }
            return keyName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SongCategoryIdComboBox.ItemsSource = categoryService.GetAllCategories();
            SongCategoryIdComboBox.DisplayMemberPath = "CategoryName";
            SongCategoryIdComboBox.SelectedValuePath = "CategoryId";
        }
    }
}
