using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Engine
{
    internal class Triangle : IDrawable
    {
        private Quaternion heading;
        private float height;
        private float width;
        public Vector4 Pos { get; private set; }
        private Vector4[] verts;

        private ConsoleColor col;

        public void SetHeading(Quaternion heading)
        {
            this.heading = heading;
            RebuildVerts();
        }
        public void UpdatePos(Vector3 deltaPos)
        {
            Pos += deltaPos.AsVector4();
            RebuildVerts();
        }
        public void SetPos(Vector3 pos)
        {
            this.Pos = new Vector4(pos, 1);
            RebuildVerts();
        }

        private void RebuildVerts()
        {
            verts[0] = new Vector4(Vector3.Transform(new Vector3(-width, 0, 0), heading), 0) + Pos;
            verts[1] = new Vector4(Vector3.Transform(new Vector3(width, 0, 0), heading), 0) + Pos;
            verts[2] = new Vector4(Vector3.Transform(new Vector3(0, height, 0), heading), 0) + Pos;
        }
        public Triangle(Quaternion heading, Vector3 pos, float height, float width, ConsoleColor col)
        {
            verts = new Vector4[3];
            this.heading = heading;
            this.height = height;
            this.width = width;
            this.col = col;
            this.Pos = new Vector4(pos, 1);
            RebuildVerts();
        }

        public void Draw(Display display)
        {
            var triangleVerts = new (Vector4 vec, ConsoleColor col)[3]
            {
                (verts[0], col),
                (verts[1], col),
                (verts[2], col)
            };
            display.DrawTriangle(triangleVerts);
            //var flippedVerts = new (Vector4 vec, ConsoleColor col)[3]
            //{
            //    (verts[1], col),
            //    (verts[0], col),
            //    (verts[2], col)
            //};
            //display.DrawTriangle(flippedVerts);
        }



    }
}
