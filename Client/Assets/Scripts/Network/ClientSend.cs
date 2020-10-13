using UnityEngine;

public class ClientSend
{
    public static void SendTCPPacket(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.tcp.SendPacket(packet);
    }

    public static void SendUDPPacket(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.udp.SendPacket(packet);
    }

    #region Packets
    public static void SendWelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            packet.Write(Client.Instance.Id);
            packet.Write(UIManager.Instance.usernameField.text);

            SendTCPPacket(packet);
        }
    }

    public static void SendUDPTestAccepted()
    {
        using (Packet packet = new Packet((int)ClientPackets.UDPTestAccepted))
        {
            packet.Write("UDP Test Accepted!");

            SendUDPPacket(packet);
        }
    }
    #endregion
}