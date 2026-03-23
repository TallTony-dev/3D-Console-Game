using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal class Box : IDrawable, ICollidable
    {
        Barrier[] barriers = new Barrier[6];
        public Vector3 MidPoint { get; private set; }
        Vector3 pos;
        float width, height, length;

        public Prism AABB
        {
            get
            {
                return new Prism(pos, width, height, length);
            }
        }

        public Box(Vector3 pos, float width, float height, float length, ConsoleColor col)
        {
            this.pos = pos;
            this.width = width;
            this.height = height;
            this.length = length;
            MidPoint = new Vector3(pos.X + width / 2, pos.Y + length / 2, pos.Z + width / 2);
            Vector3 pt1 = pos;
            Vector3 pt2 = pos + new Vector3(width, 0, 0);
            Vector3 pt3 = pos + new Vector3(0, 0, length);
            Vector3 pt4 = pos + new Vector3(width, 0, length);
            Vector3 pt5 = pos + new Vector3(0, height, 0);
            Vector3 pt6 = pos + new Vector3(width, height, 0);
            Vector3 pt7 = pos + new Vector3(0, height, length);
            Vector3 pt8 = pos + new Vector3(width, height, length);


            barriers[0] = new Barrier(pt1, pt2, pt3, pt4, col); // bottom plane
            barriers[1] = new Barrier(pt1, pt2, pt5, pt6, col); // front plane
            barriers[2] = new Barrier(pt1, pt3, pt5, pt7, col); // left plane
            barriers[3] = new Barrier(pt2, pt4, pt6, pt8, col); // right plane
            barriers[4] = new Barrier(pt3, pt4, pt7, pt8, col); // back plane
            barriers[5] = new Barrier(pt5, pt6, pt7, pt8, col); // top plane
        }

        public (bool, Vector3) CollidesWith(Prism prism)
        {
            foreach (Barrier barrier in barriers)
            {
                (bool b, Vector3 v) = barrier.CollidesWith(prism);
                if (b)
                {
                    return (b, v);
                }
            }
            return (false, Vector3.Zero);
        }

        public void Draw(Display display)
        {
            foreach (Barrier barrier in barriers)
            {
                barrier.Draw(display);
            }
        }
    }
}
