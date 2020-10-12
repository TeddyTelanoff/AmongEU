using System;
using System.Collections.Generic;
using System.Text;

namespace DeticatedServer
{
    class ServerHandle
    {
        public static void HandleWelcomeReceived(int clientID, Packet packet)
        {
            int packetClientID = packet.ReadInt();
            string username = packet.ReadString();

            if (packetClientID == clientID)
                Console.WriteLine($"{Server.Clients[clientID].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {clientID}.");
            else
                Console.WriteLine($"Player \"{username}\" (ID: {clientID}) has assumed the wrong client ID ({packetClientID})!");
        }

        public static void HandleUDPTestAccepted(int clientID, Packet packet)
        {
            string msg = packet.ReadString();

            Console.WriteLine(msg);
        }
    }
}
