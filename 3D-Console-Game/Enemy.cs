using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal abstract class Enemy : ICollidable, IDamagable, IUpdatable, IDrawable
    {
        protected PhysicsBox body;

        public PhysicsBox Body => body;

        protected float health;

        public Enemy(PhysicsBox body, float health)
        {
            this.body = body;
            this.health = health;
        }
        public abstract void Update(double dt);

        public abstract void TakeDamage(float damage);

        public void Draw(Display display)
        {
            body.Draw(display);
        }


        public (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) CollidesWith(Prism prism)
        {
            return body.CollidesWith(prism);
        }

        public Vector3 MidPoint { get { return body.MidPoint; } }
    }
}
