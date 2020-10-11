using UnityEngine;

public class ClientHandle
{
    public static void HandleWelcome(Packet packet)
    {
        string msg = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log(msg);
        Client.Instance.Id = id;

        ClientSend.SendWelcomeReceived();
    }
}