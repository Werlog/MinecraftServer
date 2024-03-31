using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServer.Packets
{
    public class PacketInHandshake : Packet
    {
        public int protocolVersion;
        public string serverAddress;
        public ushort serverPort;
        public int nextState;

        public PacketInHandshake(byte[] data) : base(data)
        {
            protocolVersion = ReadVarInt();
            serverAddress = ReadString();
            serverPort = ReadUShort();
            nextState = ReadVarInt();
        }

        public override int GetID()
        {
            return 0x00;
        }

        public override ConnectionState GetTargetState()
        {
            return ConnectionState.Handshaking;
        }
    }
}
