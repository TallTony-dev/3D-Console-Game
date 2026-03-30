using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Hudstuff
{
    internal class HealthBar : HudElement
    {

        public HealthBar() : base(0,0,Origin.TopLeft,5,4)
        {
            
        }

        public override void Draw((char c, ConsoleColor col)[,] display)
        {
            ClearArea(display);
            DrawOutline(display, ConsoleColor.DarkGreen);

            string health = Program.game.player.health.ToString();
            for (int i = 0; i < health.Length && i < width; i++)
            {
                display[x + i + 1, 2] = (health[i], ConsoleColor.Magenta);
            }
        }


    }
}
