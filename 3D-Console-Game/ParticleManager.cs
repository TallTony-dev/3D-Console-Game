using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal static class ParticleManager
    {
        static List<Particle> particles = new List<Particle>();

        public static void AddParticle(Particle particle)
        {
            particles.Add(particle);
        }

        public static void DrawParticles(Display display)
        {
            foreach (var particle in particles)
            {
                particle.Draw(display);
            }
        }
        public static void UpdateParticles(double dt)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle particle = particles[i];

                if ((particle.timeLeft -= (float)dt) < 0)
                {
                    particles.Remove(particle);
                    continue;
                }
                particle.Update(dt);
            }
        }
    }
}
