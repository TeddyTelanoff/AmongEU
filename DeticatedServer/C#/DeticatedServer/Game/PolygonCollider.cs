using System;
using System.Collections.Generic;
using System.Numerics;

namespace DeticatedServer.Game
{
    class PolygonCollider : IDisposable
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

        public readonly Vector2[] objectVertices;

        public PolygonCollider(ref Vector2 position, Vector2[] objectVertices)
        {
            this.position = position;
            this.objectVertices = new Vector2[objectVertices.Length];
            Array.Copy(objectVertices, this.objectVertices, objectVertices.Length);

            polygonColliders.Add(this);
        }

        public void Dispose()
        {
            polygonColliders.Remove(this);
        }

        public static void CheckCollisions()
        {
            for (int i = 0; i < polygonColliders.Count; i++)
            {
                PolygonCollider poly1;
                PolygonCollider poly2;
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

                    for (int v = 0; v < poly1.vertices.Length; v++)
                    {
                        Vector2 s1 = poly1.position;
                        Vector2 e1 = poly1.vertices[v];

                        Vector2 displacement = new Vector2();

                        for (int e = 0; e < poly2.vertices.Length; e++)
                        {
                            Vector2 s2 = poly2.vertices[e];
                            Vector2 e2 = poly2.vertices[(e + 1) % poly2.vertices.Length];

                            float h = (e2.X - s2.X) * (s1.Y - e2.Y) - (s1.X - e1.X) * (e2.Y - s2.Y);
                            float t1 = ((s2.Y - e2.Y) * (s1.X - s2.X) + (e2.X - s2.X) * (s1.Y - s2.Y)) / h;
                            float t2 = ((s1.Y - e1.Y) * (s1.X - s2.X) + (e1.X - s1.X) * (s1.Y - s2.Y)) / h;

                            if (t1 >= 0 && t1 < 1 && t2 >= 0 && t2 < 1)
                            {
                                displacement += (1 - t1) * (e1 - s1);
                            }
                        }

                        poly1.position += displacement * (shape == 0 ? -1 : +1);
                    }
                }
            }
        }
    }
}
