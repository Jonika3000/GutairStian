using NAudio.Wave;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GutairStian.Pages
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
            GetAllDevices();
            ComboBoxDevices.SelectedIndex = ((MainWindow)Application.Current.MainWindow).DeviceNumber;
        }
        private void GetAllDevices()
        {
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                ComboBoxDevices.Items.Add(deviceInfo.ProductName);
                //Console.WriteLine("Device {0}: {1}, {2} channels", waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }
        }

        private void ComboBoxDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).DeviceNumber = ComboBoxDevices.SelectedIndex;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow).Container.Navigate();
        }
    }
}
