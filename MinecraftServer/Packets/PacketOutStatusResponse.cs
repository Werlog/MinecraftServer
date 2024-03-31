using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServer.Packets
{
    public class PacketOutStatusResponse : Packet
    {

        public PacketOutStatusResponse() : base()
        {
            WriteString(@"{
    ""version"": {
        ""name"": ""1.8.9"",
        ""protocol"": 47
    },
    ""players"": {
        ""max"": 100,
        ""online"": 1,
        ""sample"": []
    },
    ""description"": {
        ""text"": ""Hello world""
    },
}");
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
