using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal class Slime : Enemy
    {

        public Slime(Vector3 pos) : base(new PhysicsBox(pos, 0.4f, 0.4f, 0.4f, ConsoleColor.Green, 0, true), 100)
        {
            body.Friction = 5;
        }

        double timeSinceTakenDamage = 10;
        float animTime = 0;

        public override void TakeDamage(float damage)
        {
            health -= damage;
            timeSinceTakenDamage = 0;
        }

        public override void Update(double dt)
        {
            animTime += (float)dt;
            if (timeSinceTakenDamage < 0.1f)
            {
                body.col = ConsoleColor.Red;
            }
            else
            {
                body.col = ConsoleColor.Green;
            }
            timeSinceTakenDamage += dt;

            body.Update(dt);
            Player player = Program.game.player;
            Vector3 target = player.Hitbox.MidPoint;

            if (animTime > 3 && animTime < 4)
            {
                Vector3 left = player.Left;
                body.velocity = 5 * left * (float)Math.Sin(animTime * 35) + new Vector3(0,body.velocity.Y, 0);
            }
            if (animTime > 3.5)
            {
                float zeroToOne = (animTime - 3.5f) * 2;
                body.SetSize(0.4f + zeroToOne * 0.5f, 0.4f - zeroToOne * 0.16f, 0.4f + zeroToOne * 0.5f);
            }

            if (animTime >= 4)
            {
                body.SetSize(0.4f, 0.4f, 0.4f);
                body.velocity.X = 0;
                body.velocity.Z = 0;
                body.velocity += new Vector3(0, 5, 0);
                body.velocity += Vector3.Normalize(target - body.MidPoint) * 6f;
                animTime = 0;
            }

        }

    }
}
