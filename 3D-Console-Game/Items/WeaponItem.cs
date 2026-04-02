using _3D_Console_Game.Engine;
using _3D_Console_Game.Particles;
using System.IO;
using System.Numerics;

namespace _3D_Console_Game.Items
{
    internal class WeaponItem : InventoryItem
    {
        static readonly char[,] texture =
        {
            { 'G', '▓', '▓', },
            { 'U', '▓', '▓', },
            { 'N', '▓', '▓', },
            { '!', '▓', '▓', }
        };

        protected Box box;
        Vector3 posOffset;
        protected float damage;
        protected float duration;
        protected float velocity;
        protected float timeBetweenShots;

        public WeaponItem(ConsoleColor col, Vector3 posOffset, float damage = 1, float duration = 4, float velocity = 8, float timeBetweenShots = 0.2f, float width = 0.5f, float height = 0.5f, float length = 1f) : base(col, texture)
        {
            box = new Box(Vector3.Zero, width, height, length, col);
            this.posOffset = posOffset;
            this.damage = damage;
            this.duration = duration;
            this.velocity = velocity;
            this.timeBetweenShots = timeBetweenShots;
        }

        protected virtual void Shoot(Vector3 dir, Vector3 pos)
        {
            Vector3 gunFront = box.MidBack;
            ParticleManager.AddParticle(new Bullet(duration, damage, velocity, pos, dir, col, false, true));
            for (int i = 0; i < 3; i++)
            {
                ParticleManager.AddParticle(new SplortParticle(1, gunFront, ConsoleColor.DarkGreen, 0.1f, 0.1f, 0.5f, 0.5f, 0f, dir, 5, 0));
            }
        }

        float timeSinceShot;
        public override void Update(float dt)
        {
            if (isSelected)
            {
                base.Update(dt);
                timeSinceShot += dt;
                Vector3 dir = Program.game.player.Forward;

                Vector3 adjGun = Vector3.Transform(new Vector3(0, 0.2f + 0.4f / (timeSinceShot * 4 + 0.3f), -5f), Program.game.player.view);
                box.SetPos(Program.game.player.CamPos + Vector3.Transform(new Vector3(1f,-0.7f,0f) + posOffset, Program.game.player.view), 
                    Vector3.Normalize(dir + adjGun));

                if (InputManager.IsCharPressedAsync('R'))
                {
                    //reload here maybe
                }

                if (InputManager.IsLeftMouseDown() && timeSinceShot > timeBetweenShots)
                {
                    Shoot(dir + 0.3f * adjGun, box.MidBack * 0.3f + Program.game.player.CamPos * 0.7f);
                    timeSinceShot = 0;
                }

            }
        }

        public override void Draw(Display display)
        {
            if (isSelected)
            {
                box.Draw(display);
            }
        }


    }
}
