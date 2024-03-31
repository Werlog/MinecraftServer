using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServer.Packets
{
    public class PacketOutJoinGame : Packet
    {
        public PacketOutJoinGame() : base()
        {
            WriteInt(0);
            WriteInt(1);
            WriteByte(0);
            WriteByte(10);
            WriteString("default");
            WriteBoolean(false);
        }

        public override int GetID()
        {
            return 0x01;
        }

        public override ConnectionState GetTargetState()
        {
            return ConnectionState.Play;
        }
    }
}
