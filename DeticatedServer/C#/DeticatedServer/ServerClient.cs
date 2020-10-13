using System;
using System.Net;
using System.Net.Sockets;
using DeticatedServer.Game;

namespace DeticatedServer
{
    struct ServerClient
    {
        public const int DataBufferSize = 4096;

        public int Id;
        public TCP tcp;
        public UDP udp;

        public PlayerController playerController;
        public string username;

        public ServerClient(int id)
        {
            Id = id;
            tcp = new TCP(Id);
            udp = new UDP(Id);

            playerController = new PlayerController(Id);
            username = "";
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private Packet receivedPacket;
            private byte[] receiveBuffer;

            public TCP(int id)
            {
                this.id = id;
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

                Console.WriteLine($"Client {id} Connected to the Server.");
                ServerSend.SendWelcome(id, "Welcome to the Server!");
            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receiveBuffer = null;
                receivedPacket = null;
                socket = null;
            }

            public void SendPacket(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                        Console.WriteLine("Sending Packet..");
                    }
                    else
                        Console.WriteLine($"Error Packet Data to Player {id} via TCP: Socket Closed");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error Sending Packet to Player {id} via TCP: {e}");
                }
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLength = stream.EndRead(result);
                    if (byteLength < 1)
                    {
                        Server.Clients[id].Disconnect();

                        return;
                    }

                    byte[] buffer = new byte[byteLength];
                    Array.Copy(receiveBuffer, buffer, byteLength);

                    receivedPacket.Reset(HandleBuffer(buffer));
                    stream.BeginRead(receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error Receiving TCP Data: {e}");
                    Server.Clients[id].Disconnect();
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

        public class UDP
        {
            public IPEndPoint endPoint;

            private readonly int id;

            public UDP(int id)
            {
                this.id = id;
            }

            public void Connect(IPEndPoint endPoint)
            {
                this.endPoint = endPoint;
                ServerSend.SendUDPTest(id);
            }

            public void Disconnect()
            {
                endPoint = null;
            }

            public void SendPacket(Packet packet)
            {
                Server.SendUDPPacket(endPoint, packet);
            }

            public void HandleBuffer(Packet packet)
            {
                int packetLength = packet.ReadInt();
                byte[] packetBytes = packet.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetID = packet.ReadInt();
                        Server.packetHandlers[packetID](id, packet);
                    }
                });
            }
        }

        public void Update()
        {
            if (playerController != null)
                playerController.Update();
        }

        public void Disconnect()
        {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has Disconnected.");

            tcp.Disconnect();
            udp.Disconnect();

            playerController.Dispose();

            ServerSend.SendDisconnect(Id);
        }

        public void SetUsername(string username)
        {
            this.username = username;
        }
    }
}
