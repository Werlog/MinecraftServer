using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace MinecraftServer
{
    public class Server
    {
        private TcpListener listener;

        public List<MinecraftConnection> minecraftConnections = new List<MinecraftConnection>();

        public Server()
        {
            listener = new TcpListener(IPAddress.Any, 25565);
        }

        public void StartServer()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptClient, null);
            Console.WriteLine($"Started on port: {((IPEndPoint)listener.Server.LocalEndPoint).Port}");
        }

        public void StopServer()
        {
            foreach (MinecraftConnection conn in minecraftConnections)
            {
                conn.DisconnectClient();
            }
            Console.WriteLine("Server stopped.");
        }

        public void AcceptClient(IAsyncResult result)
        {
            TcpClient client = listener.EndAcceptTcpClient(result);

            Console.WriteLine($"Received connection from {client.Client.RemoteEndPoint}");
            MinecraftConnection connection = new MinecraftConnection(client, this);
            connection.StartListening();
            minecraftConnections.Add(connection);


            listener.BeginAcceptTcpClient(AcceptClient, null);
        }
    }
}
