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

    public static void HandlePosition(Packet packet)
    {
        int clientID = packet.ReadInt();
        Vector2 position = packet.ReadVector2();

        if (clientID == Client.Instance.Id)
            Client.Instance.playerController.transform.position = new Vector3(position.x, 3, position.y);
        else
        {
            bool hasPlayer = false;
            int playerIndex = 0;
            for (int i = 0; i < GameManager.Instance.Players.Count; i++)
            {
                if (GameManager.Instance.Players[i].Id == clientID)
                {
                    hasPlayer = true;
                    playerIndex = i;
                }
            }

            if (!hasPlayer)
            {
                GameManager.Instance.CreatePlayer(clientID);

                playerIndex = GameManager.Instance.Players.Count - 1;
            }
            GameManager.Instance.Players[playerIndex].playerController.transform.position = new Vector3(position.x, 3, position.y);
        }
    }

    public static void HandleDisconnect(Packet packet)
    {
        int clientID = packet.ReadInt();

        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            if (GameManager.Instance.Players[i].Id == clientID)
            {
                GameManager.Instance.DestroyPlayer(i);
            }
        }
    }
}