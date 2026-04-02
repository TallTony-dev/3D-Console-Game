using _3D_Console_Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Items
{
    internal class InventoryItem
    {
        public int count = 1;
        protected bool isSelected = false;

        protected char[,] tex =
        {
            { 'E', 'E', 'E', },
            { '1', 'E', 'E', },
            { '2', '3', '4', },
            { '2', '3', '4', }
        };
        
        protected ConsoleColor col;

        public InventoryItem(ConsoleColor col, char[,] tex)
        {
            this.col = col;
            this.tex = tex;
        }

        public void Deselect()
        {
            isSelected = false;
        }
        public void Select()
        {
            isSelected = true;
        }

        public virtual void Update(float dt)
        {

        }

        public virtual void DrawItem(int x, int y, (char c, ConsoleColor col)[,] display)
        {
            for (int ix = 0; ix < tex.GetLength(0); ix++)
            {
                for (int iy = 0; iy < tex.GetLength(1); iy++)
                {
                    display[ix + x, iy + y] = (tex[ix, iy], col);
                }
            }
        }
        public virtual void Draw(Display display)
        {

        }
    }
}
