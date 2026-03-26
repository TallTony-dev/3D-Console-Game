using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal static class Raycast
    {

        public static List<ICollidable> CastRay(Vector3 origin, Vector3 direction, float dist)
        {
            List<ICollidable> result = new List<ICollidable>();

            foreach (ICollidable obj in Game.activeDrawables)
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

            foreach (ICollidable obj in Game.activeDrawables)
            {
                (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = obj.CollidesWith(new Prism(origin, origin, origin + direction * dist, origin + direction * dist));
                if (collides)
                {
                    float distance = (obj.MidPoint - origin).Length();
                    if (distance < closestDist)
                    {
                        closestDist = distance;
                        closest = obj;
                    }
                }
            }
            return closest;
        }

    }
}
