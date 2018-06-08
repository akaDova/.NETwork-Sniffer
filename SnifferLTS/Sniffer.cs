using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPcap;
using PacketDotNet;
using System.Windows.Controls;
using SharpPcap.WinPcap;
using SharpPcap.LibPcap;
using SharpPcap.AirPcap;
using System.Windows;
using System.Collections;

namespace SnifferLTS
{
    class Sniffer
    {
        public static Dictionary<ComboBoxItem, ICaptureDevice> DevicesList
        {
            get
            {
                if (_DevicesList != null)
                    return _DevicesList;
                else
                    return _DevicesList = GetDevicesList();
            }
        }

        private static Dictionary<ComboBoxItem, ICaptureDevice> _DevicesList;

        private static Dictionary<ComboBoxItem, ICaptureDevice> GetDevicesList()
        {
            var devices = CaptureDeviceList.Instance;
            var dictionary = new Dictionary<ComboBoxItem, ICaptureDevice>();
            foreach (var device in devices)
            {
                PcapDevice dev = (PcapDevice)device;
                var item = new ComboBoxItem
                {
                    Content = dev.Interface.FriendlyName
                    
                };
                dictionary.Add(item, device);
            }
            return dictionary;
        }

        public static void main()
        {
            var devs = GetDevicesList();
            
        }

        public static PcapDevice Device
        {
            get;
            set;
        }

        public static void StopCapturing()
        {
            if (Device == null || !IsCapturing)
                return;
            
            IsCapturing = false;
            Device.StopCapture();
            Device.Close();
        }

        public static void StartCapturing()
        {
            if (Device == null || IsCapturing)
                return;

            IsCapturing = true;
            Device.OnPacketArrival += new PacketArrivalEventHandler(OnPacketArrivalDevice); ;
            Device.Open(DeviceMode.Promiscuous, 1000);
            Device.StartCapture();
            
        }

        private static bool IsCapturing = false;


        private static void OnPacketArrivalDevice(object sender, CaptureEventArgs e)
        {
            if (!IsCapturing)
                return;
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
                    AddItem(new ArrivedPacket { PacketData = p, Protocol = "UDP", Time = e.Packet.Timeval.ToString(), From = p.DestinationPort.ToString(), To = p.SourcePort.ToString() });
                }
                else if (t == typeof(TcpPacket))
                {
                    TcpPacket p = (TcpPacket)packet;
                    AddItem(new ArrivedPacket { PacketData = p, Protocol = "TCP", Time = e.Packet.Timeval.ToString(), From = p.DestinationPort.ToString(), To = p.SourcePort.ToString() });
                }
                else if (t == typeof(ARPPacket))
                {
                    ARPPacket p = (ARPPacket)packet;
                    AddItem(new ArrivedPacket { PacketData = p, Protocol = "ARP", Time = e.Packet.Timeval.ToString(), From = p.SenderProtocolAddress.ToString(), To = p.TargetProtocolAddress.ToString() });
                }


                else if (t == typeof(IPv4Packet))
                {
                    IPv4Packet p = (IPv4Packet)packet;
                    AddItem(new ArrivedPacket { PacketData = p, Protocol = "IPv4", Time = e.Packet.Timeval.ToString(), From = p.DestinationAddress.ToString(), To = p.SourceAddress.ToString() });
                }
                else if (t == typeof(IPv6Packet))
                {
                    IPv6Packet p = (IPv6Packet)packet;
                    AddItem(new ArrivedPacket{ PacketData = p, Protocol = "IPv6", Time = e.Packet.Timeval.ToString(), From = p.DestinationAddress.ToString(), To = p.SourceAddress.ToString() });
                }
                else if (t == typeof(EthernetPacket))
                {

                    EthernetPacket p = (EthernetPacket)packet;
                    AddItem(new ArrivedPacket { PacketData = p, Protocol = "Ethernet", Time = e.Packet.Timeval.ToString(), From = p.DestinationHwAddress.ToString(), To = p.SourceHwAddress.ToString() });
                }
                //else if (t == typeof(ICMPv4Packet))
                //{

                //    ICMPv4Packet p = (ICMPv4Packet)packet;
                //    AddItem(new ArrivedPacket { Protocol = "ICMPv4", Time = e.Packet.Timeval.ToString(), From = p..ToString(), To = p.SourceHwAddress.ToString() });
                //}
                //else
                //{

                //}
                packet = packet.PayloadPacket;
                /*MainWindow.mainWindow.listViewPackets.Items.Add*/;
            }

            //MainWindow.ListViewPackets.Resources.

            

        }

        static void AddItem(ArrivedPacket packet)
        {
            if (!MainWindow.ListViewPackets.Items.CheckAccess())
            {
                MainWindow.ListViewPackets.Items.Dispatcher.Invoke(new Action<ArrivedPacket>(AddItem), packet);
            }
            else
            {
                MainWindow.ListViewPackets.Items.Add(packet);               
            }
        }


    }

}
