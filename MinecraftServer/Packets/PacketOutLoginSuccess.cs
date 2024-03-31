using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServer.Packets
{
    public class PacketOutLoginSuccess : Packet
    {
        public PacketOutLoginSuccess(string playerName) : base()
        {
            Guid guid = Guid.NewGuid();
            WriteString(guid.ToString());
            WriteString(playerName);
        }

        public override int GetID()
        {
            return 0x02;
        }

        public override ConnectionState GetTargetState()
        {
            return ConnectionState.Login;
        }
    }
}
