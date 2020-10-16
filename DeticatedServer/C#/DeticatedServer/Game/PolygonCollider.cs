using System;
using System.Collections.Generic;
using System.Numerics;

namespace DeticatedServer.Game
{
    class PolygonCollider
    {
        public static List<PolygonCollider> polygonColliders = new List<PolygonCollider>();

        public Vector2 position;
        public Vector2[] vertices
        {
            get
            {
                Vector2[] output = new Vector2[objectVertices.Length];
                for (int i = 0; i < output.Length; i++)
                    output[i] = position + objectVertices[i];

                return output;
            }
        }

        public readonly bool isStatic;

        public readonly Vector2[] objectVertices;

        public PolygonCollider(ref Vector2 position, Vector2[] objectVertices)
        {
            isStatic = false;

            this.position = position;
            this.objectVertices = new Vector2[objectVertices.Length];
            Array.Copy(objectVertices, this.objectVertices, objectVertices.Length);

            Enable();
        }

        public PolygonCollider(Vector2 position, Vector2[] objectVertices)
        {
            isStatic = true;

            this.position = position;
            this.objectVertices = new Vector2[objectVertices.Length];
            Array.Copy(objectVertices, this.objectVertices, objectVertices.Length);

            Enable();
        }

        public void Enable()
        {
            if (!polygonColliders.Contains(this))
                polygonColliders.Add(this);
        }

        public void Disable()
        {
            if (polygonColliders.Contains(this))
                polygonColliders.Remove(this);
        }

        public static void CheckCollisions()
        {
            for (int i = 0; i < polygonColliders.Count; i++)
            {
                HandleCollision(i);
            }
        }

        private static void HandleCollision(int i)
        {
            PolygonCollider poly1;
            PolygonCollider poly2;

            poly1 = polygonColliders[i];
            poly2 = polygonColliders[(i + 1) % polygonColliders.Count];

            float overlap = float.PositiveInfinity;

            for (int shape = 0; shape < 2; shape++)
            {
                if (shape == 1)
                {
                    poly1 = polygonColliders[(i + 1) % polygonColliders.Count];
                    poly2 = polygonColliders[i];
                }
                else
                {
                    poly1 = polygonColliders[i];
                    poly2 = polygonColliders[(i + 1) % polygonColliders.Count];
                }

                for (int a = 0; a < poly1.vertices.Length; a++)
                {
                    int b = (a + 1) % poly1.vertices.Length;
                    Vector2 axisProj = new Vector2(-(poly1.vertices[b].Y - poly1.vertices[a].Y), poly1.vertices[b].X - poly1.vertices[a].X);

                    float d = axisProj.Length();
                    axisProj /= d;

                    float min1 = float.NegativeInfinity, max1 = float.PositiveInfinity;
                    for (int p = 0; p < poly1.vertices.Length; p++)
                    {
                        float q = (poly1.vertices[p].X * axisProj.X + poly1.vertices[p].Y * axisProj.Y);
                        min1 = MathF.Min(min1, q);
                        max1 = MathF.Max(max1, q);
                    }

                    float min2 = float.NegativeInfinity, max2 = float.PositiveInfinity;
                    for (int p = 0; p < poly1.vertices.Length; p++)
                    {
                        float q = (poly2.vertices[p].X * axisProj.X + poly2.vertices[p].Y * axisProj.Y);
                        min2 = MathF.Min(min2, q);
                        max2 = MathF.Max(max2, q);
                    }

                    overlap = MathF.Min(MathF.Min(max1, max2) - MathF.Max(min1, min2), overlap);

                    if (!(max2 >= min1 && max1 >= min2))
                        return;
                }
            }

            if (!poly1.isStatic)
            {
                Vector2 dir = poly2.position - poly1.position;
                float s = dir.Length();
                poly1.position -= overlap * dir / s;
            }
        }
    }
}
