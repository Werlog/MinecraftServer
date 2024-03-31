using System;
using System.Security.Principal;

namespace MinecraftServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Minecraft Server";
            Server server = new Server();
            server.StartServer();

            Console.ReadLine();
        }
    }
}