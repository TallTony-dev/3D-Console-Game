using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Engine
{
    internal static class Raycast
    {
        private const float Epsilon = 0.001f;

        //LLM: use prism that isnt degenerate
        private static Prism CreateRayPrism(Vector3 origin, Vector3 direction, float dist)
        {
            Vector3 dir = Vector3.Normalize(direction);
            Vector3 ray = dir * dist;

            // Find a vector not parallel to dir to use for cross product
            Vector3 arbitrary = MathF.Abs(dir.Y) < 0.9f ? Vector3.UnitY : Vector3.UnitX;
            Vector3 perp1 = Vector3.Normalize(Vector3.Cross(dir, arbitrary)) * Epsilon;
            Vector3 perp2 = Vector3.Normalize(Vector3.Cross(dir, perp1)) * Epsilon;

            return new Prism(origin, origin + perp1, origin + perp2, origin + ray);
        }

        public static List<ICollidable> CastRay(Vector3 origin, Vector3 direction, float dist)
        {
            List<ICollidable> result = new List<ICollidable>();
            Prism rayPrism = CreateRayPrism(origin, direction, dist);

            foreach (ICollidable obj in Game.activeObjects)
            {
                if (obj.CollidesWith(rayPrism).collides)
                {
                    result.Add(obj);
                }
            }
            return result;
        }

        public static ICollidable ?GetFirstObject(Vector3 origin, Vector3 direction, float dist)
        {
            ICollidable ?closest = null;
            float closestDist = float.MaxValue;
            Prism rayPrism = CreateRayPrism(origin, direction, dist);

            foreach (object obj in Game.activeObjects)
            {
                if (obj is ICollidable c)
                {
                    (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = c.CollidesWith(rayPrism);
                    if (collides)
                    {
                        float distance = (collisionPoint - origin).Length();
                        if (distance < closestDist)
                        {
                            closestDist = distance;
                            closest = c;
                        }
                    }
                }
            }
            return closest;
        }
        public static Vector3? GetFirstObjectCollisionPos(Vector3 origin, Vector3 direction, float dist)
        {
            Vector3? closest = null;
            float closestDist = float.MaxValue;
            Prism rayPrism = CreateRayPrism(origin, direction, dist);

            foreach (object obj in Game.activeObjects)
            {
                if (obj is ICollidable c)
                {
                    (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = c.CollidesWith(rayPrism);
                    if (collides)
                    {
                        float distance = (collisionPoint - origin).Length();
                        if (distance < closestDist)
                        {
                            closestDist = distance;
                            closest = collisionPoint + dirOut * penetration;
                        }
                    }
                }
            }
            return closest;
        }
    }
}
