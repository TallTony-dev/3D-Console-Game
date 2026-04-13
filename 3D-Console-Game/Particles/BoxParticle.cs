using _3D_Console_Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Particles
{
    internal class BoxParticle : Particle
    {
        public Box box { get; protected set; }
        Vector3 velocity;

        public BoxParticle(float duration, Box box, Vector3 velocity) : base(duration)
        {
            this.box = box;
            this.velocity = velocity;
        }

        public override void Update(double dt)
        {
            box.UpdatePos(velocity * (float)dt);
        }

        public override void Draw(Display display)
        {
            box.Draw(display);
        }
    }
}
