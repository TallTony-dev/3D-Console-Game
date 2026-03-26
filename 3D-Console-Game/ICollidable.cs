using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal interface ICollidable
    {
        /// <summary>
        /// Returns true on collision with Vector3 being the normal from surface
        /// </summary>
        public (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) CollidesWith(Prism prism);

        public Vector3 MidPoint { get; }
        
    }
}
