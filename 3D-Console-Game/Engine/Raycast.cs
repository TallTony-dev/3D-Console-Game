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

        public static List<ICollidable> CastRay(Vector3 origin, Vector3 direction, float dist)
        {
            List<ICollidable> result = new List<ICollidable>();

            foreach (ICollidable obj in Game.activeObjects)
            {
                if (obj.CollidesWith(new Prism(origin,origin, origin + direction * dist, origin + direction * dist)).collides)
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

            foreach (object obj in Game.activeObjects)
            {
                if (obj is ICollidable c)
                {
                    (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = c.CollidesWith(new Prism(origin, origin, origin + direction * dist, origin + direction * dist));
                    if (collides)
                    {
                        float distance = (c.MidPoint - origin).Length();
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

    }
}
