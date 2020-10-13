using System;
using System.Collections.Generic;
using System.Text;

namespace DeticatedServer
{
    class ServerSend
    {
        #region TCP
        public static void SendTCPData(int clientID, Packet packet)
        {
            packet.WriteLength();
            Server.Clients[clientID].tcp.SendPacket(packet);
        }

        public static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                Server.Clients[i].tcp.SendPacket(packet);
            }
        }

        public static void SendTCPDataToAll(int except, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                if (i != except)
                    Server.Clients[i].tcp.SendPacket(packet);
            }
        }

        public static void SendTCPDataToAll(int[] except, Packet packet)
        {
            packet.WriteLength();
            bool send;
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                send = true;
                foreach (int cID in except)
                    if (i == cID)
                        send = false;
                if (send)
                    Server.Clients[i].tcp.SendPacket(packet);
            }
        }
        #endregion

        #region UDP
        public static void SendUDPData(int clientID, Packet packet)
        {
            packet.WriteLength();
            Server.Clients[clientID].udp.SendPacket(packet);
        }

        public static void SendUDPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                Server.Clients[i].udp.SendPacket(packet);
            }
        }

        public static void SendUDPDataToAll(int except, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                if (i != except)
                    Server.Clients[i].udp.SendPacket(packet);
            }
        }

        public static void SendUDPDataToAll(int[] except, Packet packet)
        {
            packet.WriteLength();
            bool send;
            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                send = true;
                foreach (int cID in except)
                    if (i == cID)
                        send = false;
                if (send)
                    Server.Clients[i].udp.SendPacket(packet);
            }
        }
        #endregion

        #region Packets
        public static void SendWelcome(int clientID, string msg)
        {
            Console.WriteLine("Sending Welcome...");
            using (Packet packet = new Packet((int)ServerPackets.Welcome))
            {
                packet.Write(msg);
                packet.Write(clientID);

                SendTCPData(clientID, packet);
            }
        }

        public static void SendUDPTest(int clientID)
        {
            Console.WriteLine("Sending Welcome...");
            using (Packet packet = new Packet((int)ServerPackets.UDPTest))
            {
                packet.Write("A test for UDP");

                SendUDPData(clientID, packet);
            }
        }
        #endregion
    }
}
