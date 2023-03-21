using NAudio.Gui;
using NAudio.Wave;
using NAudio.WaveFormRenderer;
using System;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GutairStian.Pages
{
    /// <summary>
    /// Логика взаимодействия для Record.xaml
    /// </summary>
    public partial class Record : Page
    {
        WaveIn waveIn;
        bool IsDragging;
        public DispatcherTimer _timer = null;
        bool nowRecord;
        double totalSeconds;
        WaveFileWriter writer;
        WaveOutEvent player;
        bool pause;
        static string workingDirectory = Environment.CurrentDirectory;
        static string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        string outputFileName;
        string outputImageName;
        public Record()
        {
            InitializeComponent();
            SetNewRndNameFiles();
            StartRecord();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += _timer_tick;
            pause = false;
        }
        private void StartRecord()
        {
            waveIn = new WaveIn();
            nowRecord = true;
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(44100, 1);
            writer = new WaveFileWriter(outputFileName, waveIn.WaveFormat);
            waveIn.DeviceNumber = ((MainWindow)System.Windows.Application.Current.MainWindow).DeviceNumber;
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.RecordingStopped += WaveIn_RecordingStopped;
            waveIn.StartRecording();
        }
        private void GetWaveImage()
        {
            var rendererSettings = new StandardWaveFormRendererSettings
            {
                Width = 800,
                TopHeight = 150,
                BottomHeight = 150,
                TopPeakPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(150, 196, 183)),
                BottomPeakPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(150, 196, 183)),
                BackgroundColor = System.Drawing.Color.FromArgb(25, 25, 25)
            };
            WaveStream stream = new WaveFileReader(outputFileName);
            var renderer = new WaveFormRenderer();
            var image = renderer.Render(stream, rendererSettings);
            image.Save(outputImageName, ImageFormat.Png);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.UriSource = new Uri(outputImageName);
            bitmap.EndInit();
            ImageWave.Source = bitmap;
            ImageWave.Visibility = Visibility.Visible;
            waveformPath.Visibility = Visibility.Hidden;
        }
        private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            waveIn.Dispose();
            waveIn = null;
            writer.Close();
            writer = null;
            GetWaveImage();
        }
        private void ListenFile()
        {
            myMediaElement.Source = new Uri(outputFileName,UriKind.RelativeOrAbsolute);
            myMediaElement.Play();
            _timer.Start();
        } 

        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            writer.WriteData(e.Buffer, 0, e.BytesRecorded);
            writer.Flush();

            int samplesRecorded = e.BytesRecorded / 2;
            short[] samples = new short[samplesRecorded];
            Buffer.BlockCopy(e.Buffer, 0, samples, 0, e.BytesRecorded);
            PathGeometry waveformGeometry = new PathGeometry();
            PathFigure waveformFigure = new PathFigure();
            waveformFigure.StartPoint = new Point(0, ActualHeight / 2);
            for (int i = 0; i < samplesRecorded; i++)
            {
                double x = i * ActualWidth / samplesRecorded;
                double y = ActualHeight / 2 - samples[i] * ActualHeight / (2 * short.MaxValue);
                waveformFigure.Segments.Add(new LineSegment(new Point(x, y), true));
            }
            waveformGeometry.Figures.Add(waveformFigure);
            waveformPath.Data = waveformGeometry;
        }
        private void SetNewRndNameFiles()
        {
            outputFileName = projectDirectory + "/Temp/" + new Random().Next(0, 20000000).ToString() + ".wav";
            outputImageName = projectDirectory + "/Temp/" + new Random().Next(0, 20000000).ToString() + ".png";
        }
        private void DeleteOldFiles()
        {
            if (player != null)
                if (player.PlaybackState == PlaybackState.Playing)
                {
                    ButtonListenImage.Source = new BitmapImage(new Uri("/Images/start.png", UriKind.RelativeOrAbsolute));
                    player.Stop();
                    player.Dispose();
                    player = null;
                }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            ButtonImageStop.Source = null;
            File.Delete(outputImageName);
            waveformPath.Data = null;
            File.Delete(outputFileName);
        }
        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            if (nowRecord)
            {
                waveIn.StopRecording();
                ButtonImageStop.Source = new BitmapImage(new Uri("/Images/icons8_record_500px.png", UriKind.RelativeOrAbsolute));
                nowRecord = false;
            }
            else
            {
                ImageWave.Visibility = Visibility.Hidden;
                waveformPath.Visibility = Visibility.Visible;
                DeleteOldFiles();
                nowRecord = true;
                ButtonImageStop.Source = new BitmapImage(new Uri("/Images/stop.png", UriKind.RelativeOrAbsolute));
                SetNewRndNameFiles();
                StartRecord();
            }
        }

        private void ButtonListen_Click(object sender, RoutedEventArgs e)
        {
            if (!nowRecord)
            {
                var  tmp = myMediaElement.Position.TotalSeconds;
               if(totalSeconds == myMediaElement.Position.TotalSeconds)
                {
                    ButtonListenImage.Source = new BitmapImage(new Uri("/Images/pause.png", UriKind.RelativeOrAbsolute));
                    ListenFile();
                }
                else if (myMediaElement.NaturalDuration.HasTimeSpan && pause== false)
                {
                    pause = true;
                    ButtonListenImage.Source = new BitmapImage(new Uri("/Images/start.png", UriKind.RelativeOrAbsolute));
                    myMediaElement.Pause();
                }
                else if(pause==true)
                {
                    ButtonListenImage.Source = new BitmapImage(new Uri("/Images/pause.png", UriKind.RelativeOrAbsolute));
                    pause =false;
                    myMediaElement.Play();
                }
                else
                {
                    ButtonListenImage.Source = new BitmapImage(new Uri("/Images/pause.png", UriKind.RelativeOrAbsolute));
                    ListenFile();
                } 
            }
        }

        private void ButtonReady_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(outputFileName) && !nowRecord)
            {
                string path;
                var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                if (dialog.ShowDialog().GetValueOrDefault())
                {
                    path = dialog.SelectedPath;
                    var filecopy = path + "\\" + Path.GetFileName(outputFileName);
                    File.Copy(outputFileName, filecopy);
                }
            }
            else
                MessageBox.Show("das");
        }

        private void myMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            ButtonListenImage.Source = new BitmapImage(new Uri("/Images/start.png", UriKind.RelativeOrAbsolute));
        }

        private void myMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            totalSeconds = myMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            if (myMediaElement.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = myMediaElement.NaturalDuration.TimeSpan;
                PlayProgress.Maximum = ts.TotalSeconds;
                PlayProgress.SmallChange = 1;
                PlayProgress.LargeChange = Math.Min(10, ts.Seconds / 10);
                //myMediaElement.Volume = (double)VolumeSlider.Value; 
                EndTime.Text = string.Format("{0:m\\:ss}", TimeSpan.FromSeconds(totalSeconds).Duration());
            }
            _timer.Start();
        } 
        private void ButtonTuner_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Container.Navigate(new Tuner());
        }
        private void PlayProgress_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            IsDragging = false;
            myMediaElement.Position = TimeSpan.FromSeconds(PlayProgress.Value);
        }
        void _timer_tick(object sender, EventArgs e)
        {
            StartTime.Text = string.Format("{0:m\\:ss}", TimeSpan.FromSeconds(myMediaElement.Position.TotalSeconds).Duration());
            if (!IsDragging && !pause)
            {
                PlayProgress.Value = myMediaElement.Position.TotalSeconds;
            }
            if (myMediaElement.NaturalDuration.HasTimeSpan)
            {
                if (myMediaElement.Position == myMediaElement.NaturalDuration.TimeSpan)
                {
                    ButtonListenImage.Source = new BitmapImage(new Uri("/Images/start.png", UriKind.RelativeOrAbsolute));
                }
            }

        }
        private void PlayProgress_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            IsDragging = true;
        }
    }
}
