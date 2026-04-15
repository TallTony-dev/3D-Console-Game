using _3D_Console_Game.Engine;
using _3D_Console_Game.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Entities
{
    internal class Entity : ICollidable, IUpdatable, IDrawable, IPushable
    {

        public float health { get; protected set; }
        protected double timeSinceTakenDamage { get; private set; } = 10;

        protected PhysicsBox body;
        public PhysicsBox Body => body;
        public Vector3 MidPoint { get { return body.MidPoint; } }
        public bool isDead { get; protected set; } = false;

        protected Entity(float health, PhysicsBox body)
        {
            this.health = health;
            this.body = body;
        }

        public void ApplyForce(Vector3 forceDir, float forceStrength, Vector3 source)
        {
            body.CollideWithPhysics(forceDir, forceStrength, source);
        }

        public virtual void Update(double dt)
        {
            timeSinceTakenDamage += dt;
            body.Update(dt);
        }


        public virtual void Draw(Display display)
        {
            body.Draw(display);
        }

        public (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) CollidesWith(Prism prism)
        {
            return body.CollidesWith(prism);
        }

        public virtual void TakeDamage(float damage, Vector3 sourcePos)
        {
            health -= damage;
            timeSinceTakenDamage = 0;
            if (health <= 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    ParticleManager.AddParticle(new SplortParticle(1, MidPoint, ConsoleColor.Red, 0.3f, 0.3f, 1, MathF.PI, 10, Vector3.UnitY, 6.5f, 15));
                }
                isDead = true;
            }
        }


    }
}
