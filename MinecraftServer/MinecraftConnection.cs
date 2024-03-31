using MinecraftServer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServer
{
    public class MinecraftConnection
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private Server server;
        private byte[] receiveBuffer;
        private PacketReader packetReader;
        public ConnectionState ConnectionState { get; private set; }
        
        public MinecraftConnection(TcpClient tcpClient, Server server)
        {
            this.tcpClient = tcpClient;
            this.server = server;

            packetReader = new PacketReader(this);
            stream = tcpClient.GetStream();
            receiveBuffer = new byte[8192];
            ConnectionState = ConnectionState.Handshaking;
        }

        public void StartListening()
        {
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, OnReceiveData, null);
        }

        public void OnReceiveData(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength == 0)
                {
                    Console.WriteLine("Client disconnected.");
                    DisconnectClient();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, Math.Min(receiveBuffer.Length, data.Length));
                packetReader.OnReceiveData(data);

                stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, OnReceiveData, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to properly process received data from client, disconnecting it.");
                Console.WriteLine(ex.Message);
                Console.Write(ex.StackTrace);
                DisconnectClient();
            }
        }
        /// <summary>
        /// Writes the packet data to the NetworkStream
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void SendPacket(Packet packet)
        {
            packet.Send(stream);
        }

        public void ProccessPacket(Packet packet)
        {
            if (packet is PacketInHandshake handshakePacket)
            {
                Console.WriteLine($"Handshake packet: Protocol Version: {handshakePacket.protocolVersion}, Server Address: {handshakePacket.serverAddress}:{handshakePacket.serverPort}, Next State: {handshakePacket.nextState}");
                ConnectionState = (ConnectionState)handshakePacket.nextState;
            }else if (packet is PacketInStatusRequest)
            {
                PacketOutStatusResponse packetOutStatusResponse = new PacketOutStatusResponse();
                Console.WriteLine($"Status request packet.");
                SendPacket(packetOutStatusResponse);
            }else if (packet is PacketInLoginStart loginPacket)
            {
                PacketOutLoginSuccess loginSuccess = new PacketOutLoginSuccess(loginPacket.PlayerName);
                SendPacket(loginSuccess);
                ConnectionState = ConnectionState.Play;

                PacketOutJoinGame joinPacket = new PacketOutJoinGame();
                SendPacket(joinPacket);
            }
        }

        public void DisconnectClient()
        {
            server.minecraftConnections.Remove(this);
            tcpClient.Close();
        }
    }

    public enum ConnectionState : byte
    {
        Handshaking = 0,
        Status = 1,
        Login = 2,
        Configuration = 3,
        Play = 4,
    }
}
