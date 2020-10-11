using System;
using System.Collections.Generic;
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
    }

    public void ConnectToServer()
    {
        InitializeClientHandles();
        tcp.Connect();
    }

    public struct TCP
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
                    // TODO: Disconnect

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
                Debug.LogError($"Error Receiving TCP Data: {e}; Disconnecting...");
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

    private void InitializeClientHandles()
    {
        packetHandlers = new Dictionary<int, PacketHandler>
        {
            { (int)ServerPackets.Welcome, ClientHandle.HandleWelcome }
        };

        Debug.Log("Initialized Handles.");
    }
}