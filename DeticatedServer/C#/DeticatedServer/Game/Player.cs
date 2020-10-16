using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DeticatedServer.Game
{
    class Player
    {
        public readonly int Id;

        public PlayerController playerController;
        public PolygonCollider polygonCollider;

        public Vector2 Position;

        public Player(int id)
        {
            Id = id;

            playerController = new PlayerController(ref Position, Id);
            polygonCollider = playerController.polygonCollider;
        }

        public void Enable()
        {
            Position = Vector2.Zero;
            polygonCollider.Enable();
        }

        public void Disable()
        {
            Position = Vector2.Zero;
            polygonCollider.Disable();
        }

        public void Update()
        {
            playerController.Update();
        }
    }
}
