using _3D_Console_Game.Engine;
using _3D_Console_Game.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Entities
{
    internal class Gunner : Enemy
    {
        public Gunner(Vector3 pos) : base(new PhysicsBox(pos, 0.4f, 1.8f, 0.4f, ConsoleColor.Black, 0, true), 100)
        {
            body.Friction = 1.5f;
            gun = new Box(pos + new Vector3(0, 1.4f, 0), 0.3f, 0.1f, 1f, ConsoleColor.Gray, Vector3.Zero);
        }

        double timeSinceAttacked = -0.5;
        double timeSinceTakenDamage = 10;

        Box gun;

        public override void TakeDamage(float damage, Vector3 sourcePos)
        {
            health -= damage;
            timeSinceTakenDamage = 0;
            base.TakeDamage(damage, sourcePos);
        }

        public override void Update(double dt)
        {
            Player player = Program.game.player;
            Vector3 target = player.Hitbox.MidPoint;
            timeSinceTakenDamage += dt;
            timeSinceAttacked += dt;

            Vector3 toTargetFromGun = target - gun.MidPoint;
            Vector3 toTargetFromBody = target - body.Pos;
            Vector3 toTargetFromMid = target - body.MidPoint;

            Vector3 gunDir = toTargetFromGun.LengthSquared() > 0.0001f ? Vector3.Normalize(toTargetFromGun) : Vector3.UnitZ;
            Vector3 targetDir = toTargetFromBody.LengthSquared() > 0.0001f ? Vector3.Normalize(toTargetFromBody) : Vector3.UnitZ;
            Vector3 gunAimDir = toTargetFromMid.LengthSquared() > 0.0001f ? Vector3.Normalize(toTargetFromMid) : Vector3.UnitZ;

            if (timeSinceAttacked > 1)
            {
                timeSinceAttacked = 0;
                ParticleManager.AddParticle(new Bullet(4, 4, 3, gun.MidPoint, gunDir, ConsoleColor.DarkRed, true, false));
            }
            body.velocity = Vector3.Clamp(targetDir * ( toTargetFromBody.LengthSquared() - 23) * 0.2f, new Vector3(-10,0,-10),new Vector3(10,0,10)) + new Vector3(0,body.velocity.Y,0);

            gun.SetPos(body.Pos + new Vector3(0, 1.4f, 0), gunAimDir);
            body.Update(dt);
        }

        public override void Draw(Display display)
        {
            base.Draw(display);
            gun.Draw(display);
        }

    }
}
