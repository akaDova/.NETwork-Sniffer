using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SnifferLTS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            comBoxDevices.ItemsSource = Sniffer.DevicesList.Keys;
            comBoxDevices.SelectedIndex = 1;
            ListViewPackets = listViewPackets;
            mainWindow = this;
            
            
        }


        public static MainWindow mainWindow;

        public static ObservableCollection<ArrivedPacket> Packets;

        public static ListView ListViewPackets;

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            Sniffer.StopCapturing();


        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (comBoxDevices.SelectedIndex == -1)
                return;         
            Sniffer.Device = (PcapDevice)Sniffer.DevicesList[(ComboBoxItem)comBoxDevices.SelectedItem];
            Sniffer.StartCapturing();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxPacket.Text = "";
            textBoxPacket1.Text = "";
            listViewPackets.Items.Clear();
        }

        private void ListViewPackets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBoxPacket.Text = "";
            textBoxPacket1.Text = "";
            ArrivedPacket packet = (ArrivedPacket)listViewPackets.SelectedItem;
            if (packet == null)
                return;

            textBoxPacket.Text = Encoding.UTF8.GetString(packet.PacketData.Bytes);
            textBoxPacket1.Text = BitConverter.ToString(packet.PacketData.Bytes);
            //dataGridPacket.ItemsSource = packet.PacketData;
            //listViewPackets.SelectedItem
        }
    }
}
