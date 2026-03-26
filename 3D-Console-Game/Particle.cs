using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal class Particle : IUpdatable
    {

        public float timeLeft;
        Vector3 velocity;
        Box box;

        public Particle(float duration, Box box, Vector3 velocity)
        {
            timeLeft = duration;
            this.box = box;
            this.velocity = velocity;
        }

        public void Update(double dt)
        {
            box.UpdatePos(velocity * (float)dt);
        }

        public void Draw(Display display)
        {
            box.Draw(display);
        }
    }
}
