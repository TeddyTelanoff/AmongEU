using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace DeticatedServer.Game
{
    static class Map
    {
        public static PolygonCollider polygonCollider;

        public static void Initialize()
        {
            polygonCollider = new PolygonCollider(new Vector2(), null);
        }
    }
}
