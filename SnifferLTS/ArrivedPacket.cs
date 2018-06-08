using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferLTS
{
    public class ArrivedPacket
    {
        public ArrivedPacket(string protocol, string time, string from, string to)
        {
            Protocol = protocol;
            Time = time;
            From = from;
            To = to;
        }

        public Packet PacketData
        {
            get;
            set;
        }

        public ArrivedPacket()
        {

        }

        public string Protocol
        {
            get; set;
            
        }

        public string Time { get; set; }

        public string From { get; set; }

        public string To { get; set; }
    }
}
