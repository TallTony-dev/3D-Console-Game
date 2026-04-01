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

        public virtual void Update(float dt)
        {

        }

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
            else if (origin == Origin.BottomLeft)
            {
                for (int i = x; i < x + width && i < xlen; i++)
                {
                    for (int j = ylen - y - height; j < ylen - y && j < ylen; j++)
                    {
                        if (j >= 0)
                            display[i, j] = (' ', ConsoleColor.Black);
                    }
                }
            }


        }

        protected void DrawOutline((char c, ConsoleColor col)[,] display, ConsoleColor col)
        {
            int xlen = display.GetLength(0);
            int ylen = display.GetLength(1);

            int bx, by;
            if (origin == Origin.TopLeft)
            {
                bx = x;
                by = y;
            }
            else if (origin == Origin.BottomRight)
            {
                bx = xlen - x - width;
                by = ylen - y - height;
            }
            else // BottomLeft
            {
                bx = x;
                by = ylen - y - height - 1;
            }

            DrawBox(display, bx, by, width, height, col);
        }

        protected static void DrawBox((char c, ConsoleColor col)[,] display, int bx, int by, int w, int h, ConsoleColor col)
        {
            int xlen = display.GetLength(0);
            int ylen = display.GetLength(1);

            for (int i = bx; i < bx + w && i < xlen; i++)
            {
                if (i < 0) continue;
                if (by >= 0 && by < ylen)
                    display[i, by] = ('─', col);
                if (by + h >= 0 && by + h < ylen)
                    display[i, by + h] = ('─', col);
            }
            for (int j = by; j < by + h && j < ylen; j++)
            {
                if (j < 0) continue;
                if (bx >= 0 && bx < xlen)
                    display[bx, j] = ('│', col);
                if (bx + w >= 0 && bx + w < xlen)
                    display[bx + w, j] = ('│', col);
            }

            if (bx + w >= 0 && bx + w < xlen && by >= 0 && by < ylen)
                display[bx + w, by] = ('┐', col);
            if (bx + w >= 0 && bx + w < xlen && by + h >= 0 && by + h < ylen)
                display[bx + w, by + h] = ('┘', col);
            if (bx >= 0 && bx < xlen && by >= 0 && by < ylen)
                display[bx, by] = ('┌', col);
            if (bx >= 0 && bx < xlen && by + h >= 0 && by + h < ylen)
                display[bx, by + h] = ('└', col);
        }

        public enum Origin { TopLeft, BottomRight, BottomLeft };
    }
}
