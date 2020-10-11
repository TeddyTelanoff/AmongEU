using UnityEngine;

public class ClientSend
{
    public static void SendTCPPacket(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.tcp.SendPacket(packet);
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
    #endregion
}