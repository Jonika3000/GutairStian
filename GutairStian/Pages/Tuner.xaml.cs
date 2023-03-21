using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GutairStian.Pages
{
    /// <summary>
    /// Логика взаимодействия для Tuner.xaml
    /// </summary>
    public partial class Tuner : Page
    {
        WaveIn waveIn = new WaveIn();
        public Tuner()
        {
            InitializeComponent(); 
            waveIn.DeviceNumber = ((MainWindow)Application.Current.MainWindow).DeviceNumber;
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(44100, 16, 1);
            waveIn.RecordingStopped += WaveIn_RecordingStopped;
            waveIn.StartRecording();
        }
        private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            waveIn.Dispose();
            waveIn = null;
        }
        void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        { 
            byte[] buffer = e.Buffer;  
            float frequency = CalculateFrequency(buffer);
            TuneGuitar(frequency); 

        } 
        void TuneGuitar(float frequency)
        {
            const float E6 = 82.41f;
            const float A5 = 110.00f;
            const float D4 = 146.83f;
            const float G3 = 196.00f;
            const float B2 = 246.94f;
            const float E1 = 329.63f;

            const float F = 349f;
            const float Fsharp = 370f;

            float minFreq = 85;  
                LabelNumberF.Content= (Math.Abs(frequency )).ToString();
                if (Math.Abs(frequency - E6) < 5f)//not work
                {
                    LabelNameString.Content = "E6";
                    ProgressBarTune.Value = 100; 
                } 
                else if (Math.Abs(frequency - A5) < 5f)
                {
                    LabelNameString.Content = ("A5");
                    ProgressBarTune.Value = 100;
                }
                else if (Math.Abs(frequency - D4) < 5f)
                {
                    LabelNameString.Content = ("D4");
                    ProgressBarTune.Value = 100;
                }
                else if (Math.Abs(frequency - G3) < 5f)
                {
                    LabelNameString.Content = ("G3");
                    ProgressBarTune.Value = 100;
                } 
                else if (Math.Abs(frequency - B2) < 5f)
                {
                    LabelNameString.Content = ("B2");
                    ProgressBarTune.Value = 100;
                } 
                else if (Math.Abs(frequency - E1) < 5f)
                {
                    LabelNameString.Content = ("E1");
                    ProgressBarTune.Value = 100; 
                } 
        }
        float CalculateFrequency(byte[] buffer)
        { 
            var signal = new Complex32[buffer.Length / 2];
            for (int i = 0; i < buffer.Length; i += 2)
            {
                float sample = BitConverter.ToInt16(buffer, i) / 32768f;
                signal[i / 2] = new Complex32(sample, 0);
            } 
            Fourier.Forward(signal); 
            float maxAmplitude = 0;
            float maxFrequency = 0;
            for (int i = 0; i < signal.Length / 2; i++)
            {
                float amplitude = signal[i].Magnitude;
                if (amplitude > maxAmplitude)
                {
                    maxAmplitude = amplitude;
                    maxFrequency = i * 44100f / signal.Length;
                }
            }

            return maxFrequency;
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Container.Navigate(new Record());
        }
    }
}
