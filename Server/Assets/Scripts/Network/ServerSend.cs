using UnityEngine;

public class ServerSend
{
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

    public static void SendWelcome(int clientID, string msg, string amsg)
    {
        Debug.Log("Sending Welcome...");
        using (Packet packet = new Packet((int)ServerPackets.Welcome))
        {
            packet.Write(msg);
            packet.Write(clientID);

            SendTCPData(clientID, packet);
        }
    }
}