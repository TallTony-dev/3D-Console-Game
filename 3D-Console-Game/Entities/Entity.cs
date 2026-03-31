using _3D_Console_Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Entities
{
    internal class Entity : ICollidable, IUpdatable, IDrawable
    {

        public float health { get; protected set; }

        protected PhysicsBox body;
        public PhysicsBox Body => body;
        public Vector3 MidPoint { get { return body.MidPoint; } }
        public bool isDead { get; protected set; } = false;

        protected Entity(float health, PhysicsBox body)
        {
            this.health = health;
            this.body = body;
        }


        public virtual void Update(double dt)
        {
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
            if (health <= 0)
            {
                isDead = true;
            }
        }


    }
}
