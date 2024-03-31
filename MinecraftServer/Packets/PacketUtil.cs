using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServer.Packets
{
    public class PacketUtil
    {
        public static Packet? CreatePacketFromID(int packetID, ConnectionState state, byte[] packetData)
        {
            if (state == ConnectionState.Handshaking)
            {
                switch(packetID)
                {
                    case 0x00:
                        return new PacketInHandshake(packetData);
                }
            }else if (state == ConnectionState.Status)
            {
                switch(packetID)
                {
                    case 0x00:
                        return new PacketInStatusRequest(packetData);
                }
            }else if (state == ConnectionState.Login)
            {
                switch (packetID)
                {
                    case 0x00:
                        return new PacketInLoginStart(packetData);
                }
            }

            return null;
        }
    }
}
