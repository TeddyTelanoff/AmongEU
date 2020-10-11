using System;
using System.Net.Sockets;
using UnityEngine;

public struct ServerClient
{
    public const int DataBufferSize = 4096;

    public int Id;
    public TCP tcp;

    public ServerClient(int id)
    {
        Id = id;
        tcp = new TCP(Id);
    }

    public struct TCP
    {
        public TcpClient socket;

        private readonly int id;
        private NetworkStream stream;
        private Packet receivedPacket;
        private byte[] receiveBuffer;

        public TCP(int id)
        {
            this.id = id;

            socket = null;
            stream = null;
            receivedPacket = null;
            receiveBuffer = null;
        }

        public void Connect(TcpClient socket)
        {
            this.socket = socket;
            this.socket.ReceiveBufferSize = DataBufferSize;
            this.socket.SendBufferSize = DataBufferSize;

            stream = this.socket.GetStream();

            receivedPacket = new Packet();
            receiveBuffer = new byte[DataBufferSize];

            stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);

            Debug.Log($"Client {id} Connected to the Server.");
            ServerSend.SendWelcome(id, "Welcome to the Server!", $"{id} Joined the Server!");
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
                    Debug.LogError($"Error Packet Data to Player {id} via TCP: Socket Closed");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error Sending Packet to Player {id} via TCP: {e}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength < 1)
                {
                    // TODO: Disconnect

                    return;
                }

                byte[] buffer = new byte[byteLength];
                Array.Copy(receiveBuffer, buffer, byteLength);

                receivedPacket.Reset(HandleBuffer(buffer));
                stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error Receiving TCP Data: {e}");
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
                int id = this.id;
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetID = packet.ReadInt();
                        Server.packetHandlers[packetID](id, packet);
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
}