using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Hudstuff
{
    internal abstract class HudElement
    {
        protected int x;
        protected int y;
        protected Origin origin;
        protected int width;
        protected int height;

        protected HudElement(int x, int y, Origin origin, int width, int height)
        {
            this.x = x; 
            this.y = y;
            this.origin = origin;
            this.width = width;
            this.height = height;
        }

        public abstract void Draw((char c, ConsoleColor col)[,] display);

        protected void ClearArea((char c, ConsoleColor col)[,] display)
        {
            int xlen = display.GetLength(0);
            int ylen = display.GetLength(1);
            if (origin == Origin.TopLeft)
            {
                for (int i = x; i < x + width && i < xlen; i++)
                {
                    for (int j = y; j < y + height && j < ylen; j++)
                    {
                        display[i, j] = (' ', ConsoleColor.Black);
                    }
                }
            }
            else if (origin == Origin.BottomRight)
            {
                for (int i = xlen - x; i > xlen - x - width && i >= 0; i--)
                {
                    for (int j = ylen - y; j < ylen - y - height && j >= 0; j--)
                    {
                        display[i, j] = (' ', ConsoleColor.Black);
                    }
                }
            }


        }

        protected void DrawOutline((char c, ConsoleColor col)[,] display, ConsoleColor col)
        {
            int xlen = display.GetLength(0);
            int ylen = display.GetLength(1);

            if (origin == Origin.TopLeft)
            {
                for (int i = x; i < x + width && i < xlen; i++)
                {
                    if (y < ylen)
                        display[i, y] = ('─', col);
                    if (y + height < ylen)
                        display[i, y + height] = ('─', col);
                }
                for (int i = y; i < y + height && i < ylen; i++)
                {
                    if (x < xlen)
                        display[x, i] = ('│', col);
                    if (x + width < xlen)
                        display[x + width, i] = ('│', col);
                }

                display[x + width, y] = ('┐', col);
                display[x + width, y + height] = ('┘', col);
                display[x, y] = ('┌', col);
                display[x, y + height] = ('└', col);
            }
            if (origin == Origin.BottomRight)
            {
                for (int i = xlen - x; i > xlen - x - width && i >= 0; i--)
                {
                    if (y < ylen)
                        display[i, y] = ('─', col);
                    if (y + height < ylen)
                        display[i, y + height] = ('─', col);
                }
                for (int i = ylen - y; i < ylen - y - height && i >= 0; i--)
                {
                    if (x < xlen)
                        display[x, i] = ('│', col);
                    if (x + width < xlen)
                        display[x + width, i] = ('│', col);
                }
            }

        }

        protected enum Origin { TopLeft, BottomRight };
    }
}
