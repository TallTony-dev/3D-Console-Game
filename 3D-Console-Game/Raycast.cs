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

        public static ICollidable? CastRay(Vector3 origin, Vector3 direction, float dist)
        {
            foreach (ICollidable obj in Game.activeWalls)
            {
                if (obj.CollidesWith(new Prism(origin,origin, origin + direction * dist, origin + direction * dist)).collides)
                {
                    return obj;
                }
            }
            return null;
        }


    }
}
