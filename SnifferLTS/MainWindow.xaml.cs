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
            Packets = new ObservableCollection<ArrivedPacket>();
            Sniffer.Device = (PcapDevice)Sniffer.DevicesList[(ComboBoxItem)comBoxDevices.SelectedItem];
            Sniffer.StartCapturing();
        }

        internal static void OnPacketArrivalDevice(object sender, CaptureEventArgs e)
        {
            Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            //ArrayList list = (ArrayList)MainWindow.ListViewPackets.ItemsSource;
            //MainWindow.ListViewPackets.ItemsSource
            var list = MainWindow.Packets;
            while (packet != null)
            {
                Type t = packet.GetType();
                if (t == typeof(UdpPacket))
                {
                    UdpPacket p = (UdpPacket)packet;
                    list.Add(new ArrivedPacket("IPv6", e.Packet.Timeval.ToString(), p.DestinationPort.ToString(), p.SourcePort.ToString()));
                }
                else if (t == typeof(TcpPacket))
                {
                    TcpPacket p = (TcpPacket)packet;
                    list.Add(new ArrivedPacket("TCP", e.Packet.Timeval.ToString(), p.DestinationPort.ToString(), p.SourcePort.ToString()));
                }
                else if (t == typeof(ARPPacket))
                {
                    ARPPacket p = (ARPPacket)packet;
                    list.Add(new ArrivedPacket("ARP", e.Packet.Timeval.ToString(), p.SenderProtocolAddress.ToString(), p.TargetProtocolAddress.ToString()));
                }


                else if (t == typeof(IPv4Packet))
                {
                    IPv4Packet p = (IPv4Packet)packet;
                    list.Add(new ArrivedPacket("IPv4", e.Packet.Timeval.ToString(), p.DestinationAddress.ToString(), p.SourceAddress.ToString()));
                }
                else if (t == typeof(IPv6Packet))
                {
                    IPv6Packet p = (IPv6Packet)packet;
                    list.Add(new ArrivedPacket("IPv6", e.Packet.Timeval.ToString(), p.DestinationAddress.ToString(), p.SourceAddress.ToString()));
                }
                else if (t == typeof(EthernetPacket))
                {

                    EthernetPacket p = (EthernetPacket)packet;
                    list.Add(new ArrivedPacket("Ethernet", e.Packet.Timeval.ToString(), p.DestinationHwAddress.ToString(), p.SourceHwAddress.ToString()));
                }
                //else
                //{

                //}
                packet = packet.PayloadPacket;
                MainWindow.mainWindow.listViewPackets.ItemsSource = list;
            }
        }
    }
}
