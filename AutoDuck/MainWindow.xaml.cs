using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;


namespace AutoDuckProgram
{
    public partial class MainWindow : Window
    {
        List<string> captureDeviceIDs = new List<string>();
        List<string> playbackDeviceIDs = new List<string>();

        public AutoDuck autoDuck = new AutoDuck();

        public MainWindow()
        {
            InitializeComponent();
            UpdateDevicesLists();
        }

        void UpdateDevicesLists()
        {
            List<AudioDevice> captureDevices = AudioDevicesManager.GetCaptureDevices();
            List<AudioDevice> playbackDevices = AudioDevicesManager.GetPlaybackDevices();
            if (IncludePlaybackDevices.IsChecked ?? false)
            {
                captureDevices.AddRange(playbackDevices);
            }

            CaptureDevicesList.Items.Clear();
            captureDeviceIDs.Clear();

            PlaybackDevicesList.Items.Clear();
            playbackDeviceIDs.Clear();

            foreach (AudioDevice device in captureDevices)
            {
                Grid grid = new Grid();
                CheckBox checkBox = new CheckBox();
                ProgressBar progressBar = new ProgressBar();

                checkBox.Content = device.Name;
                checkBox.Tag = device.ID;
                checkBox.Click += CaptureDeviceChanged;
                checkBox.Height = 16;


                grid.Children.Add(checkBox);
                CaptureDevicesList.Items.Add(grid);
            }
            foreach (AudioDevice device in playbackDevices)
            {
                Grid grid = new Grid();
                CheckBox checkBox = new CheckBox();
                ProgressBar progressBar = new ProgressBar();

                checkBox.Content = device.Name;
                checkBox.Tag = device.ID;
                checkBox.Click += PlaybackDeviceChanged;
                checkBox.Height = 16;

                grid.Children.Add(checkBox);
                PlaybackDevicesList.Items.Add(grid);
            }
            StartStopButton.IsEnabled = false;
        }

        private void UpdateDevices(object sender, RoutedEventArgs e)
        {
            UpdateDevicesLists();
        }

        private void StartStopAutoDuck_Click(object sender, RoutedEventArgs e)
        {
            if (autoDuck.isRunning)
            {
                autoDuck.Stop();
                UpdateDevicesLists();
                StartStopButton.Content = "Start";
                CaptureDevicesList.IsEnabled = true;
                PlaybackDevicesList.IsEnabled = true;
            }
            else
            {
                autoDuck.Parameters = new AutoDuckParametes(captureDeviceIDs, playbackDeviceIDs);
                autoDuck.Start();
                StartStopButton.Content = "Stop";
                CaptureDevicesList.IsEnabled = false;
                PlaybackDevicesList.IsEnabled = false;
            }
        }

        private void DelayChanged(object sender, RoutedEventArgs e)
        {
            TextBox inputBox = (TextBox)sender;
            inputBox.Text = VerifyStringIsInFormatOfFloat(inputBox.Text);

            if (inputBox.Text == string.Empty)
            {
                inputBox.Text = AutoDuck.delay.ToString();
            }
            else { 
                AutoDuck.delay = float.Parse(inputBox.Text, CultureInfo.InvariantCulture);
            }
        }

        private void FadeOutTimeChanged(object sender, RoutedEventArgs e)
        {
            TextBox inputBox = (TextBox)sender;
            inputBox.Text = VerifyStringIsInFormatOfFloat(inputBox.Text);

            if (inputBox.Text == string.Empty)
            {
                inputBox.Text = AutoDuck.fadeOutTime.ToString();
            }
            else { 
                AutoDuck.fadeOutTime = float.Parse(inputBox.Text, CultureInfo.InvariantCulture);
            }
        }

        private void CaptureDeviceChanged(object sender, RoutedEventArgs e)
        {
            string id = (string)((CheckBox)sender).Tag;
            if (captureDeviceIDs.Contains(id))
            {
                captureDeviceIDs.Remove(id);
            }
            else
            {
                captureDeviceIDs.Add(id);
            }
            if (playbackDeviceIDs.Count > 0 && captureDeviceIDs.Count > 0)
            {
                StartStopButton.IsEnabled = true;
            }
            else
            {
                StartStopButton.IsEnabled = false;
            }
        }

        private void PlaybackDeviceChanged(object sender, RoutedEventArgs e)
        {
            string id = (string)((CheckBox)sender).Tag;
            if (playbackDeviceIDs.Contains(id))
            {
                playbackDeviceIDs.Remove(id);
            }
            else
            {
                playbackDeviceIDs.Add(id);
            }

            if (playbackDeviceIDs.Count > 0 && captureDeviceIDs.Count > 0)
            {
                StartStopButton.IsEnabled = true;
            }
            else
            {
                StartStopButton.IsEnabled = false;
            }
        }

        private void MaxVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AutoDuck.maxVolume = (float)((Slider)sender).Value;
        }

        private void MinVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AutoDuck.minVolume = (float)((Slider)sender).Value;
        }

        private void TresholdVolumeChanged(object sender, RoutedEventArgs e)
        {
            AutoDuck.volumeTresshold = (float)TresholdSlider.Value;
        }
       
        private void WindowClose(object sender, CancelEventArgs e)
        {
            autoDuck.Stop();
            AudioDevicesManager.StopRecording();
        }

        private string VerifyStringIsInFormatOfFloat(string oldString)
        {
            Regex regex = new Regex(@"(\d+\.?\d*)");

            if (regex.Matches(oldString).Count==0) {
                return "";
            }else {
                return regex.Matches(oldString)[0].Value;
            }
        }
    }
}
