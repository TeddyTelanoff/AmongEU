using System.Net;
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
        Client.Instance.udp.Connect(((IPEndPoint)Client.Instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void HandleUDPTest(Packet packet)
    {
        string msg = packet.ReadString();

        Debug.Log(msg);

        ClientSend.SendUDPTestAccepted();
    }
}