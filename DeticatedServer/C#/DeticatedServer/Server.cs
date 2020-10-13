using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace DeticatedServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, ServerClient> Clients = new Dictionary<int, ServerClient>();
        public delegate void PacketHandler(int clientID, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;
        private static UdpClient udpClient;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Console.WriteLine("Starting Server...");
            InitializeServerClients();
            InitializeServerHandles();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            udpClient = new UdpClient(Port);
            udpClient.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server Active on Port {Port}");
        }

        private static void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($"Connection Found at {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (Clients[i].tcp.socket == null)
                {
                    Clients[i].tcp.Connect(client);
                    return;
                }
            }

            Console.WriteLine($"{client.Client.RemoteEndPoint} Failed to Connect: Server Full!");
        }

        private static void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.EndReceive(result, ref clientEndPoint);
                udpClient.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < sizeof(int))
                    return;

                using (Packet packet = new Packet(data))
                {
                    int clientID = packet.ReadInt();

                    if (clientID <= 0 || clientID > MaxPlayers)
                        return;

                    if (Clients[clientID].udp.endPoint == null)
                    {
                        Clients[clientID].udp.Connect(clientEndPoint);
                        return;
                    }

                    if (Clients[clientID].udp.endPoint.ToString() == clientEndPoint.ToString())
                        Clients[clientID].udp.HandleBuffer(packet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Receiving UDP data: {e}");
            }
        }

        public static void SendUDPPacket(IPEndPoint endPoint, Packet packet)
        {
            try
            {
                if (endPoint != null)
                    udpClient.BeginSend(packet.ToArray(), packet.Length(), endPoint, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Sending Data to {endPoint} via UDP: {e}");
            }
        }

        private static void InitializeServerClients()
        {
            for (int i = 1; i <= MaxPlayers; i++)
                Clients.Add(i, new ServerClient(i));
            Console.WriteLine("Initialized Server Clients.");
        }

        private static void InitializeServerHandles()
        {
            packetHandlers = new Dictionary<int, PacketHandler>
        {
            { (int)ClientPackets.WelcomeReceived, ServerHandle.HandleWelcomeReceived },
            { (int)ClientPackets.UDPTestAccepted, ServerHandle.HandleUDPTestAccepted }
        };
            Console.WriteLine("Initialized packets.");
        }
    }
}
