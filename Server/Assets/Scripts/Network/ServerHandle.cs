using UnityEngine;

public class ServerHandle
{
    public static void HandleWelcomeReceived(int clientID, Packet packet)
    {
        int packetClientID = packet.ReadInt();
        string username = packet.ReadString();

        if (packetClientID == clientID)
            Debug.Log($"{Server.Clients[clientID].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {clientID}.");
        else
            Debug.Log($"Player \"{username}\" (ID: {clientID}) has assumed the wrong client ID ({packetClientID})!");
    }
}