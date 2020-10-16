using System;
using System.Diagnostics;
using DeticatedServer.Game;

namespace DeticatedServer
{
    class ServerHandle
    {
        public static void HandleWelcomeReceived(int clientID, Packet packet)
        {
            int packetClientID = packet.ReadInt();
            string username = packet.ReadString();

            if (packetClientID == clientID)
            {
                Console.WriteLine($"{Server.Clients[clientID].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {clientID}.");
                Server.Clients[clientID].SetUsername(username);
            }
            else
                Console.WriteLine($"Player \"{username}\" (ID: {clientID}) has assumed the wrong client ID ({packetClientID})!");
        }

        public static void HandleUDPTestAccepted(int clientID, Packet packet)
        {
            string msg = packet.ReadString();

            Console.WriteLine(msg);
        }

        public static void HandleInput(int clientID, Packet packet)
        {
            int maxKeys = packet.ReadInt();
            for (int i = 0; i < maxKeys; i++)
            {
                int key = packet.ReadInt();
                int mode = packet.ReadInt();

                if (key < 0 || key > (int)PlayerInput.Last)
                {
                    Console.WriteLine($"Player \"{Server.Clients[clientID].username}\" (ID: {clientID}) Has Invalid Input Key: {key}");
                    return;
                }

                if (mode < 0 || mode > (int)ButtonMode.Last)
                {
                    Console.WriteLine($"Player \"{Server.Clients[clientID].username}\" (ID: {clientID}) Has Invalid Input Mode: {mode}");
                    return;
                }

                Server.Clients[clientID].player.playerController.Input[key] = mode;
            }
        }
    }
}