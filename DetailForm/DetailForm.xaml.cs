using Microsoft.Win32;
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
using System.Windows.Threading;

namespace DetailForm
{
    /// <summary>
    /// Interaction logic for DetalliForm.xaml
    /// </summary>
    public partial class DetalliForm : Window
    {
        public DetalliForm()
        {
            InitializeComponent();
        }

        MediaPlayer mediaPlayer = new MediaPlayer();
        string fileName;

        private void BT_Click_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = ".mp3"
            };
            bool? diaLogOK = fileDialog.ShowDialog();
            if (diaLogOK == true)
            {
                fileName = fileDialog.FileName;
                FileName.Text = fileDialog.SafeFileName;
                mediaPlayer.Open(new Uri(fileName));
            }
        }

        private void BT_Click_Play(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();

            // Create a DispatcherTimer
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) // Set timer interval to 5 seconds
            };

            // Define the event handler to stop playback when the timer ticks
            timer.Tick += (s, args) =>
            {
                mediaPlayer.Stop();
                timer.Stop(); // Stop the timer after it ticks
            };

            // Start the timer
            timer.Start();
        }
    }
}
