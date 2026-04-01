using _3D_Console_Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Items
{
    internal class BoxItem : InventoryItem, IDrawable
    {
        static readonly char[,] tex =
        {
            { 'B', '▓', '▓', },
            { 'O', '▓', '▓', },
            { 'X', '▓', '▓', },
            { '!', '▓', '▓', }
        };

        Box box;

        public BoxItem(ConsoleColor boxCol) : base(boxCol, tex)
        {
            box = new Box(Vector3.Zero, 1, 1, 1, boxCol);
        }

        public override void Update(float dt)
        {
            if (isSelected)
            {
                base.Update(dt);
                box.SetPos(Program.game.player.nearestLookCollision);
                if (InputManager.IsCharPressedAsync('R'))
                {
                    box.UpdatePos(Vector3.Zero, 0, 0, 2 * dt);
                }

                if (InputManager.IsLeftMousePressed())
                {
                    Game.activeObjects.Add(new Box(box.Pos, box.hitbox.width, box.hitbox.height, box.hitbox.depth, col, box.pitch, box.roll, box.yaw));
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
