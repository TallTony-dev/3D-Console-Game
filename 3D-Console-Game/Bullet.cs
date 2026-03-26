using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal class Bullet : Particle
    {
        float damage = 0;
        public Bullet(float duration, float damage, float velocity, Vector3 pos, Vector3 dir, ConsoleColor col, float width = 0.1f, float height = 0.1f, float length = 0.3f) : base(duration, new Box(pos, width, height, length, col, dir), velocity * dir)
        {
            this.damage = damage;
        }

        public override void Update(double dt)
        {
            base.Update(dt);
            foreach (object obj in Game.activeDrawables)
            {
                if (obj is ICollidable collidable && obj is IDamagable damagable)
                {
                    (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = collidable.CollidesWith(box.hitbox);
                    if (collides)
                    {
                        damagable.TakeDamage(damage);
                        timeLeft = -1;
                        return;
                    }
                }
            }
        }
    }
}
