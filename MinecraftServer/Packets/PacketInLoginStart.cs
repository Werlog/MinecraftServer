using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServer.Packets
{
    public class PacketInLoginStart : Packet
    {
        public string PlayerName { get; private set; }
        public PacketInLoginStart(byte[] data) : base(data)
        {
            PlayerName = ReadString();
        }

        public override int GetID()
        {
            return 0x00;
        }

        public override ConnectionState GetTargetState()
        {
            return ConnectionState.Login;
        }
    }
}
