using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal class Prism
    {
        public Vector3 pos; //from bottom left
        public float width;
        public float height;
        public float depth;

        public Prism() { }
        public Prism(Vector3 pos, float width, float height, float depth)
        {
            this.pos = pos;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }
        public Vector3 MidPoint
        {
            get
            {
                return new Vector3(pos.X + width/2, pos.Y + height/2, pos.Z + depth/2);
            }
        }

        public bool Intersects(Prism prism)
        {
            return (prism.pos.X < pos.X + width && pos.X < prism.pos.X + prism.width
                && prism.pos.Y < pos.Y + height && pos.Y < prism.pos.Y + prism.height
                && prism.pos.Z < pos.Z + depth && pos.Z < prism.pos.Z + prism.depth);
        }
        public bool Contains(Vector3 point)
        {
            return (point.X < pos.X + width && pos.X < point.X
                && point.Y < pos.Y + height && pos.Y < point.Y
                && point.Z < pos.Z + depth && pos.Z < point.Z);
        }
    }
}
