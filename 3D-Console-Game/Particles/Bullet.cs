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
        public object? LastCollidedObj { get; private set; }
        public Bullet(float duration, float damage, float velocity, Vector3 pos, Vector3 dir, ConsoleColor col, bool hitsPlayer, bool hitsEnemies, float width = 0.1f, float height = 0.1f, float length = 0.3f) : base(duration, new Box(pos + new Vector3(width / 2f, height / 2f, length/2f), width, height, length, col, dir), velocity * Vector3.Normalize(dir))
        {
            this.damage = damage;
            this.hitsPlayer = hitsPlayer;
            this.hitsEnemies = hitsEnemies;
        }

        private const float MaxStepTime = 1f / 60f;

        public override void Update(double deltaTime)
        {
            float remaining = (float)deltaTime;
            while (remaining > 0)
            {
                float dt = Math.Min(remaining, MaxStepTime);
                remaining -= dt;
                base.Update(dt);

                foreach (object obj in Game.activeObjects)
                {
                    if (obj is Player p && hitsPlayer)
                    {
                        (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = box.CollidesWith(p.Hitbox);
                        if (collides)
                        {
                            LastCollidedObj = obj;
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
                            LastCollidedObj = obj;
                            e.TakeDamage(damage, collisionPoint);
                            timeLeft = -1;
                            return;
                        }
                    }
                    if (obj is Box b)
                    {
                        (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = b.CollidesWith(box.hitbox);

                        if (collides)
                        {
                            LastCollidedObj = obj;
                            timeLeft = -1;
                            return;
                        }
                    }
                }
                remaining -= dt;
            }
        }
    }
}
