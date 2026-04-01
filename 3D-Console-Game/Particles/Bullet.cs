using _3D_Console_Game.Engine;
using _3D_Console_Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Particles
{
    internal class Bullet : BoxParticle
    {
        float damage = 0;
        bool hitsEnemies;
        bool hitsPlayer;
        public Bullet(float duration, float damage, float velocity, Vector3 pos, Vector3 dir, ConsoleColor col, bool hitsPlayer, bool hitsEnemies, float width = 0.1f, float height = 0.1f, float length = 0.3f) : base(duration, new Box(pos, width, height, length, col, dir), velocity * dir)
        {
            this.damage = damage;
            this.hitsPlayer = hitsPlayer;
            this.hitsEnemies = hitsEnemies;
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            foreach (object obj in Game.activeObjects)
            {
                if (obj is Player p && hitsPlayer)
                {
                    (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = box.CollidesWith(p.Hitbox);
                    if (collides)
                    {
                        p.TakeDamage(damage, collisionPoint);
                        timeLeft = -1;
                        return;
                    }
                }

                if (obj is Entity e && hitsEnemies && obj is Enemy)
                {

                    (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = e.CollidesWith(box.hitbox);
                    if (collides)
                    {
                        e.TakeDamage(damage, collisionPoint);
                        timeLeft = -1;
                        return;
                    }
                }
                if (obj is Box b)
                {
                    if (b.CollidesWith(box.hitbox).collides)
                    {
                        timeLeft = -1;
                        return;
                    }
                }
            }
        }
    }
}
