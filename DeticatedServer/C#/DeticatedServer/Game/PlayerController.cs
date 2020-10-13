using System;
using System.Collections.Generic;
using System.Numerics;

namespace DeticatedServer.Game
{
    class PlayerController : IDisposable
    {
        public int clientID;
        public Vector2 position;

        public Dictionary<int, int> Input;

        public PolygonCollider polygonCollider;

        public PlayerController(int id)
        {
            clientID = id;

            Input = new Dictionary<int, int>();
            for (int i = 0; i <= (int)PlayerInput.Last; i++)
                Input.Add(i, (int)ButtonMode.None);

            polygonCollider = new PolygonCollider(ref position, new Vector2[]
            {
                new Vector2(-1, -1),
                new Vector2( 1, -1),
                new Vector2( 1,  1),
                new Vector2(-1,  1)
            });
        }

        public void Dispose()
        {
            polygonCollider.Dispose();
        }

        public void Update()
        {
            Vector2 direction = new Vector2();

            if (Input[(int)PlayerInput.Right] == (int)ButtonMode.Held)
                direction += Vector2.UnitX;
            if (Input[(int)PlayerInput.Left] == (int)ButtonMode.Held)
                direction -= Vector2.UnitX;
            if (Input[(int)PlayerInput.Forward] == (int)ButtonMode.Held)
                direction += Vector2.UnitY;
            if (Input[(int)PlayerInput.Back] == (int)ButtonMode.Held)
                direction -= Vector2.UnitY;

            position += direction;

            if (Server.Clients[clientID].tcp.socket == null)
                position = new Vector2();
            else
                ServerSend.SendPosition(clientID);
        }
    }

    enum ButtonMode
    {
        None,
        Pressed,
        Held,
        Release,

        Last = Release
    }

    enum PlayerInput
    {
        Left,
        Right,
        Forward,
        Back,

        Last = Back
    }
}