using _3D_Console_Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Particles
{
    internal class SplortParticle : TriangleParticle
    {
        public SplortParticle(float duration, Vector3 pos, ConsoleColor col, float height, float width, float pitchSpread, float yawSpread, float rollSpread, Vector3 target, float velMult, float gravityMult)
            : base(duration, new Triangle(Quaternion.Identity, pos, height, width, col), Vector3.Zero, Vector3.Zero)
        {
            Random r = new();
            float roll = (r.NextSingle() - 0.5f) * 2 * rollSpread;
            float pitch = (r.NextSingle() - 0.5f) * 2 * pitchSpread;
            float yaw = (r.NextSingle() - 0.5f) * 2 * yawSpread;
            Quaternion rot = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);

            Vector3 velocity = Vector3.Transform(target, rot) * velMult;
            this.accel = -Vector3.UnitY * gravityMult;
            this.velocity = velocity;
            this.triangle = new Triangle(rot, pos, height, width, col);
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            Quaternion look = Quaternion.Normalize(new Quaternion(Vector3.Cross(Vector3.UnitY, Vector3.Normalize(velocity)), 1 + Vector3.Dot(Vector3.UnitY, Vector3.Normalize(velocity))));
            triangle.SetHeading(look);
        }

    }
}
