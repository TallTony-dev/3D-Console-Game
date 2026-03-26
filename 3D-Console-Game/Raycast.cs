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

            foreach (ICollidable obj in Game.activeWalls)
            {
                if (obj.CollidesWith(new Prism(origin,origin, origin + direction * dist, origin + direction * dist)).collides)
                {
                    result.Add(obj);
                }
            }
            return result;
        }


    }
}
