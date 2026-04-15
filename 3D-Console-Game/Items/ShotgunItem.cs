using _3D_Console_Game.Particles;
using _3D_Console_Game.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Items
{
    internal class ShotgunItem : WeaponItem
    {
        static readonly char[,] texture =
        {
            { 'S', '▓', '▓', },
            { 'H', '▓', '▓', },
            { 'O', '▓', '▓', },
            { 'T', '▓', '▓', }
        };

        Random rand = new Random();
        int numShots;
        float spread;
        public ShotgunItem(Player player, ConsoleColor col, float damage, float spread = 0.2f, int numShots = 6) : base(player, col, Vector3.Zero, damage, 1, 12, 0.5f)
        {
            this.numShots = numShots;
            this.spread = spread;
            tex = texture;
        }

        protected override void Shoot(Vector3 dir, Vector3 pos)
        {
            SoundPlayer.PlaySound("shotgun1.wav", 0.4f);
            Vector3 gunFront = box.MidBack;
            for (int i = 0; i < numShots; i++)
            {
                Quaternion spreadRot = Quaternion.CreateFromYawPitchRoll((rand.NextSingle() - 0.5f) * spread, (rand.NextSingle() - 0.5f) * spread, 0);
                ParticleManager.AddParticle(new Bullet(duration, damage, velocity, pos, Vector3.Transform(dir, spreadRot), col, false, true));
            }
        }

    }
}
