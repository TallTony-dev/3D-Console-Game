using _3D_Console_Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Particles
{
    internal abstract class Particle : IUpdatable, IDrawable
    {

        public float timeLeft;

        public Particle(float duration)
        {
            timeLeft = duration;
        }

        public abstract void Update(double dt);

        public abstract void Draw(Display display);
    }
}
