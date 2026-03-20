using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal class Wall : Drawable
    {
        Vector3 pos;
        Vector3 TopLeft { get { return pos + HeightVec; } }
        Vector3 TopRight { get { return pos + HeightVec + length * dir; } }
        Vector3 BottomLeft { get { return pos; } }
        Vector3 BottomRight { get { return pos + length * dir; } }



        Vector3 HeightVec { get { return new Vector3(0, height, 0); } }
        Vector3 dir;
        float length;
        float height;
        ConsoleColor color;

        public Wall(float height, Vector3 startPoint, Vector3 endPoint, ConsoleColor color)
        {
            this.height = height;
            pos = startPoint;
            length = (endPoint - startPoint).Length();
            dir = Vector3.Normalize(endPoint - startPoint);
            this.color = color;
        }

        public bool CollidesWith(Prism prism)
        {
            return false;
        }

        public override void Draw(Display display)
        {
            display.DrawSquare(new Vector4(TopLeft, 1), new Vector4(TopRight, 1), new Vector4(BottomLeft, 1), new Vector4(BottomRight, 1), color);
            display.DrawSquare(new Vector4(BottomLeft, 1), new Vector4(BottomRight, 1), new Vector4(TopLeft, 1), new Vector4(TopRight, 1), color);
        }

    }
}
