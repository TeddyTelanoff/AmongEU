using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, ServerClient> Clients = new Dictionary<int, ServerClient>();
    public delegate void PacketHandler(int clientID, Packet packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;

    public static void Start(int maxPlayers, int port)
    {
        MaxPlayers = maxPlayers;
        Port = port;

        Debug.Log("Starting Server...");
        InitializeServerClients();
        InitializeServerHandles();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

        Debug.Log($"Server Active on Port {Port}");
    }

    private static void TCPConnectCallback(IAsyncResult result)
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(result);
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        Debug.Log($"Connection Found at {client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (Clients[i].tcp.socket == null)
            {
                Clients[i].tcp.Connect(client);
                return;
            }
        }

        Debug.Log($"{client.Client.RemoteEndPoint} Failed to Connect: Server Full!");
    }

    private static void InitializeServerClients()
    {
        for (int i = 1; i <= MaxPlayers; i++)
            Clients.Add(i, new ServerClient(i));
        Debug.Log("Initialized Server Clients.");
    }

    private static void InitializeServerHandles()
    {
        packetHandlers = new Dictionary<int, PacketHandler>
        {
            { (int)ClientPackets.WelcomeReceived, ServerHandle.HandleWelcomeReceived }
        };
        Debug.Log("Initialized packets.");
    }
}