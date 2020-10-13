using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public const int DataBufferSize = 4096;

    public static Client Instance { get; private set; }

    public string ip = "127.0.0.1";
    public int port = 42069;

    public int Id { get; set; }
    public TCP tcp;
    public UDP udp;

    public PlayerController playerController;

    private bool isConnected;
    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        InitializeClientHandles();

        isConnected = true;
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedPacket;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };

            receiveBuffer = new byte[DataBufferSize];
            socket.BeginConnect(Instance.ip, Instance.port, ConnectCallback, socket);
        }

        public void Disconnect()
        {
            Instance.Disconnect();

            stream = null;
            receiveBuffer = null;
            receivedPacket = null;
            socket = null;
        }

        public void ConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);
            
            if (!socket.Connected)
            {
                return;
            }

            Debug.Log("Connected!");

            stream = socket.GetStream();

            receivedPacket = new Packet();

            stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }

        public void SendPacket(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    Debug.Log("Sending Packet..");
                }
                else
                    Debug.LogError("Error sending data to server via TCP: Socket Closed");
            }
            catch (Exception e)
            {
                Debug.Log($"Error sending data to server via TCP: {e}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    Instance.Disconnect();

                    return;
                }

                byte[] buffer = new byte[byteLength];
                Array.Copy(receiveBuffer, buffer, byteLength);

                receivedPacket.Reset(HandleBuffer(buffer));

                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

                Debug.Log("Message Received");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error Receiving TCP Data: {e}; Disconnecting...");
                Disconnect();
            }
        }

        private bool HandleBuffer(byte[] buffer)
        {
            int packetLength = 0;

            receivedPacket.SetBytes(buffer);

            if (receivedPacket.UnreadLength() >= sizeof(int))
            {
                packetLength = receivedPacket.ReadInt();
                if (packetLength < 1)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receivedPacket.UnreadLength())
            {
                byte[] packetBytes = receivedPacket.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetID = packet.ReadInt();
                        packetHandlers[packetID](packet);
                    }
                });

                packetLength = 0;
                if (receivedPacket.UnreadLength() >= sizeof(int))
                {
                    packetLength = receivedPacket.ReadInt();
                    if (packetLength < 1)
                    {
                        return true;
                    }
                }
            }

            return packetLength <= 1;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(Instance.ip), Instance.port);
        }

        public void Connect(int localPort)
        {
            socket = new UdpClient(localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet packet = new Packet())
            {
                SendPacket(packet);
            }
        }

        public void Disconnect()
        {
            Instance.Disconnect();

            endPoint = null;
            socket = null;
        }

        public void SendPacket(Packet packet)
        {
            try
            {
                packet.InsertInt(Instance.Id);

                if (socket != null)
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error Sending Data via UDP: {e}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                byte[] buffer = socket.EndReceive(result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (buffer.Length < sizeof(int))
                {
                    Instance.Disconnect();
                    return;
                }

                HandleBuffer(buffer);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error Receiving Data via UDP: {e}; Disconecting...");
                Disconnect();
            }
        }

        private void HandleBuffer(byte[] buffer)
        {
            using (Packet packet = new Packet(buffer))
            {
                int packetLength = packet.ReadInt();
                buffer = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(buffer))
                {
                    int packetID = packet.ReadInt();
                    packetHandlers[packetID](packet);
                }
            });
        }
    }

    public void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected.");

            Application.Quit();
        }
    }

    private void InitializeClientHandles()
    {
        packetHandlers = new Dictionary<int, PacketHandler>
        {
            { (int)ServerPackets.Welcome, ClientHandle.HandleWelcome },
            { (int)ServerPackets.UDPTest, ClientHandle.HandleUDPTest },
            { (int)ServerPackets.Position, ClientHandle.HandlePosition },
            { (int)ServerPackets.Disconnect, ClientHandle.HandlePosition }
        };

        Debug.Log("Initialized Handles.");
    }
}