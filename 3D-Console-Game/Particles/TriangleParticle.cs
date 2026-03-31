using _3D_Console_Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Particles
{
    internal class TriangleParticle : Particle
    {
        protected Vector3 velocity;
        protected Vector3 accel;
        protected Triangle triangle;

        public TriangleParticle(float duration, Triangle triangle, Vector3 velocity, Vector3 accel) : base(duration)
        {
            this.triangle = triangle;
            this.velocity = velocity;
            this.accel = accel;
        }

        public override void Update(double dt)
        {
            triangle.UpdatePos(velocity * (float)dt);
            velocity += accel * (float)dt;
        }

        public override void Draw(Display display)
        {
            triangle.Draw(display);
        }
    }
}
