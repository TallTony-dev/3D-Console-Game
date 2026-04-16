using _3D_Console_Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Items
{
    internal class TelescopeItem : BoxHeldItem
    {
        static readonly char[,] texture =
        {
            { 'T', '▓', '▓', },
            { 'E', '▓', '▓', },
            { 'L', '▓', '▓', },
            { 'E', '▓', '▓', }
        };

        public TelescopeItem(ConsoleColor col, Player player) : base(player, col, Vector3.Zero, texture, 0.5f, 0.5f, 1)
        {
        }

        bool isZoomed = false;
        public override void Update(float dt)
        {
            if (isSelected)
            {
                base.Update(dt);

                if (InputManager.IsLeftMousePressed())
                {
                    if (!isZoomed)
                    {
                        isZoomed = true;
                        owner.SetZoom(4);
                    }
                    else
                    {
                        isZoomed = false;
                        owner.SetZoom(1);
                    }
                }

            }
            else
            {
                isZoomed = false;
                owner.SetZoom(1);
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
