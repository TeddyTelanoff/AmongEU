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

        if (!GameManager.Instance.Players[id].gameObject.activeInHierarchy)
            GameManager.Instance.Players[id].gameObject.SetActive(true);

        ClientSend.SendWelcomeReceived();
        Client.Instance.udp.Connect(((IPEndPoint)Client.Instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void HandleUDPTest(Packet packet)
    {
        string msg = packet.ReadString();

        Debug.Log(msg);

        ClientSend.SendUDPTestAccepted();
    }

    public static void HandlePosition(Packet packet)
    {
        int clientID = packet.ReadInt();
        Vector2 position = packet.ReadVector2();

        if (clientID == Client.Instance.Id)
            Client.Instance.playerController.transform.position = new Vector3(position.x, 3, position.y);
        else
        {
            if (!GameManager.Instance.Players[clientID].gameObject.activeInHierarchy)
                GameManager.Instance.Players[clientID].gameObject.SetActive(true);
            GameManager.Instance.Players[clientID].playerController.transform.position = new Vector3(position.x, 3, position.y);
        }
    }

    public static void HandleDisconnect(Packet packet)
    {
        int clientID = packet.ReadInt();

        if (GameManager.Instance.Players[clientID].gameObject.activeInHierarchy)
            GameManager.Instance.Players[clientID].gameObject.SetActive(false);
    }
}