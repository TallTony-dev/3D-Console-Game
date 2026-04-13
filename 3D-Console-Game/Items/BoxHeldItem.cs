using _3D_Console_Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Items
{
    internal class BoxHeldItem : InventoryItem
    {
        protected Box box;
        protected Vector3 dirOffset;
        Vector3 posOffset;


        public BoxHeldItem(Player player, ConsoleColor col, Vector3 posOffset, char[,] tex, float width = 0.5f, float height = 0.5f, float length = 1f) : base(col, tex, player)
        {
            box = new Box(Vector3.Zero, width, height, length, col);
            this.posOffset = posOffset;
        }


        public override void Update(float dt)
        {
            if (isSelected)
            {
                base.Update(dt);
                Vector3 dir = owner.Forward;

                Vector3 adjGun = Vector3.Transform(new Vector3(0, 0.2f, -5f), owner.view);
                box.SetPos(owner.CamPos + Vector3.Transform(new Vector3(1f, -0.7f, 0f) + posOffset, owner.view),
                    Vector3.Normalize(dir + adjGun + dirOffset));
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
