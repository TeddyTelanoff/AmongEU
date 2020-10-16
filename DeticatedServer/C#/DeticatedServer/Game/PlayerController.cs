using System.Collections.Generic;
using System.Numerics;

namespace DeticatedServer.Game
{
    class PlayerController
    {
        public int clientID;
        public Vector2 position;

        public Dictionary<int, int> Input;

        public PolygonCollider polygonCollider;

        public PlayerController(ref Vector2 position, int id)
        {
            this.position = position;
            clientID = id;

            Input = new Dictionary<int, int>();
            for (int i = 0; i <= (int)PlayerInput.Last; i++)
                Input.Add(i, (int)ButtonMode.None);

            polygonCollider = new PolygonCollider(ref position, new Vector2[]
            {
                new Vector2(-2.322897f, -2.586864f),
                new Vector2( 2.322897f, -2.586864f),
                new Vector2( 2.322897f,  2.586864f),
                new Vector2(-2.322897f,  2.586864f)
            });
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

            polygonCollider.position += direction;

            if (Server.Clients[clientID].tcp.socket == null)
                polygonCollider.position = Vector2.Zero;
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