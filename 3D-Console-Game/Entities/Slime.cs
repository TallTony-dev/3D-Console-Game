using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using _3D_Console_Game.Engine;
using _3D_Console_Game.Particles;
using _3D_Console_Game.Sound;

namespace _3D_Console_Game.Entities
{
    internal class Slime : Enemy
    {

        public Slime(Vector3 pos) : base(new PhysicsBox(pos, 0.4f, 0.4f, 0.4f, ConsoleColor.Green, 0.3f, 0, true), 100)
        {
            body.Friction = 5;
        }

        float animTime = 0;

        public override void TakeDamage(float damage, Vector3 sourcePos)
        {
            base.TakeDamage(damage, sourcePos);
            if (timeSinceTakenDamage > 0.1)
            {
                SoundPlayer.PlaySound("smallhurt1.wav", 0.4f);
            }
        }

        public override void Update(double dt)
        {
            base.Update(dt);
            animTime += (float)dt;
            if (timeSinceTakenDamage < 0.1f)
            {
                body.col = ConsoleColor.Red;
            }
            else
            {
                body.col = ConsoleColor.Green;
            }

            Player player = Program.game.player;
            Vector3 target = player.Hitbox.MidPoint;

            if (player.ObjectCollidedWithPlayer(this))
            {
                player.TakeDamage(5, MidPoint);
                
            }

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
                Vector3 toTarget = target - body.MidPoint;
                if (toTarget.LengthSquared() > 0.0001f)
                {
                    body.velocity += Vector3.Normalize(toTarget) * 6f;
                }
                animTime = 0;
                Vector3 bot = body.MidBottom;
                for (int i = 0; i < 10; i++)
                {
                    ParticleManager.AddParticle(new SplortParticle(1, bot, ConsoleColor.Green, 0.4f, 0.4f, 1, MathF.PI, 10, Vector3.UnitY, 5, 15));
                }
                SoundPlayer.PlaySoundFromPos("slimebounce1.wav", body.Pos);
            }

        }

    }
}
