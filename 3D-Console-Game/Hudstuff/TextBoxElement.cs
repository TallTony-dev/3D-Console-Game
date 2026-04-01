using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Hudstuff
{
    internal class TextBoxElement : HudElement
    {
        string text;


        public TextBoxElement(int x, int y, Origin origin, string text) : base(x, y, origin, text.Length + 1, 2)
        {
            this.text = text;
        }

        public override void Draw((char c, ConsoleColor col)[,] display)
        {
            ClearArea(display);
            DrawOutline(display, ConsoleColor.DarkGreen);


            for (int i = 0; i < text.Length && i < width; i++)
            {
                display[x + i + 1, y + 1] = (text[i], ConsoleColor.Magenta);
            }
        }
    }
}
