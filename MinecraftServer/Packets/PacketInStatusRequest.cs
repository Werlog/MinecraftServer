using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServer.Packets
{
    public class PacketInStatusRequest : Packet
    {
        public PacketInStatusRequest(byte[] data) : base(data)
        {

        }

        public override int GetID()
        {
            return 0x00;
        }

        public override ConnectionState GetTargetState()
        {
            return ConnectionState.Status;
        }
    }
}
