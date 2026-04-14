using _3D_Console_Game.Engine;
using _3D_Console_Game.Particles;
using System.IO;
using System.Numerics;

namespace _3D_Console_Game.Items
{
    internal class WeaponItem : BoxHeldItem
    {
        static readonly char[,] texture =
        {
            { 'G', '▓', '▓', },
            { 'U', '▓', '▓', },
            { 'N', '▓', '▓', },
            { '!', '▓', '▓', }
        };

        protected float damage;
        protected float duration;
        protected float velocity;
        protected float timeBetweenShots;

        public WeaponItem(Player player, ConsoleColor col, Vector3 posOffset, float damage = 1, float duration = 4, float velocity = 8, float timeBetweenShots = 0.2f, float width = 0.5f, float height = 0.5f, float length = 1f) : base(player, col, Vector3.Zero, texture)
        {
            box = new Box(Vector3.Zero, width, height, length, col);
            this.damage = damage;
            this.duration = duration;
            this.velocity = velocity;
            this.timeBetweenShots = timeBetweenShots;
        }

        protected virtual void Shoot(Vector3 dir, Vector3 pos)
        {
            Vector3 gunFront = box.MidBack;
            ParticleManager.AddParticle(new Bullet(duration, damage, velocity, pos, dir, col, false, true));
            for (int i = 0; i < 6; i++)
            {
                ParticleManager.AddParticle(new SplortParticle(0.4f, gunFront, ConsoleColor.DarkGreen, 0.1f, 0.1f, 0.5f, 0.5f, 0f, dir, 18, 0));
            }
        }

        public float TimeSinceShot => timeSinceShot;
        protected float timeSinceShot;
        public override void Update(float dt)
        {
            if (isSelected)
            {
                base.Update(dt);
                timeSinceShot += dt;
                Vector3 adjGun = Vector3.Transform(new Vector3(0, 0.4f / (TimeSinceShot * 4 + 0.3f), 0), owner.view);
                dirOffset = adjGun * 3;
                if (InputManager.IsCharPressedAsync('R'))
                {
                    //reload here maybe
                }

                if (InputManager.IsLeftMouseDown() && timeSinceShot > timeBetweenShots)
                {
                    Vector3 shotPos = box.MidBack * 0.3f + owner.CamPos * 0.7f;
                    Vector3 recoil = Vector3.Transform(new Vector3(-0.2f, 0, 0f), owner.view) + adjGun;
                    Vector3 shotDir = Vector3.Normalize(owner.Forward + 0.3f * recoil);
                    Shoot(shotDir, shotPos);
                    timeSinceShot = 0;
                }

            }
        }

        public override void Draw(Display display)
        {
            base.Draw(display);
        }


    }
}
