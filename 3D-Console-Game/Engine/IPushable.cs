using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Engine
{
    internal interface IPushable
    {
        public void ApplyForce(Vector3 forceDir, float forceStrength, Vector3 source);

    }
}
