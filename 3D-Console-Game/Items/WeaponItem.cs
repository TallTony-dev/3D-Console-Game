using _3D_Console_Game.Engine;
using _3D_Console_Game.Particles;
using System.Numerics;

namespace _3D_Console_Game.Items
{
    internal class WeaponItem : InventoryItem
    {
        static readonly char[,] tex =
        {
            { 'G', '▓', '▓', },
            { 'U', '▓', '▓', },
            { 'N', '▓', '▓', },
            { '!', '▓', '▓', }
        };

        Box box;

        public WeaponItem(ConsoleColor col) : base(col, tex)
        {
            box = new Box(Vector3.Zero, 1, 1, 1, col);
        }

        float timeSinceShot;
        public override void Update(float dt)
        {
            if (isSelected)
            {
                base.Update(dt);
                timeSinceShot += dt;
                Vector3 dir = Program.game.player.Forward;

                box.SetPos(Program.game.player.CamPos + Program.game.player.Forward + Vector3.Transform(new Vector3(1,-1,0), Program.game.player.view), dir);

                if (InputManager.IsCharPressedAsync('R'))
                {
                    //reload here maybe
                }

                if (InputManager.IsLeftMouseDown() && timeSinceShot > 0.2f)
                {
                    ParticleManager.AddParticle(new Bullet(4, 1, 5, dir + Program.game.player.CamPos, dir, col, false, true));
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
