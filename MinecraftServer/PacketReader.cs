using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftServer.Packets;

namespace MinecraftServer
{
    public class PacketReader
    {
        private MinecraftConnection connection;
        private List<byte> receiveBuffer;

        public PacketReader(MinecraftConnection connection)
        {
            this.connection = connection;
            receiveBuffer = new List<byte>(50);
        }

        public void OnReceiveData(byte[] data)
        {
            receiveBuffer.AddRange(data);
            TryReadPacket();
        }

        public void TryReadPacket()
        {
            int[] ints = VarIntUtil.DecodeVarIntIncludeLength(receiveBuffer.ToArray());
            int packetLength = ints[0];
            int varIntLength = ints[1];

            while (receiveBuffer.Count >= packetLength + varIntLength)
            {
                byte[] packetData = receiveBuffer.GetRange(varIntLength, packetLength).ToArray();
                int packetID = VarIntUtil.DecodeVarInt(packetData);

                Packet? packet = PacketUtil.CreatePacketFromID(packetID, connection.ConnectionState, packetData);

                if (packet != null)
                    connection.ProccessPacket(packet);
                else
                    Console.WriteLine($"Received unknown packet: Packet ID: {packetID}, Connection State: {connection.ConnectionState}");

                receiveBuffer.RemoveRange(0, packetLength + varIntLength);

                if (receiveBuffer.Count == 0) break;
                ints = VarIntUtil.DecodeVarIntIncludeLength(receiveBuffer.ToArray());
                packetLength = ints[0];
                varIntLength = ints[1];
            }
        }
    }
}
