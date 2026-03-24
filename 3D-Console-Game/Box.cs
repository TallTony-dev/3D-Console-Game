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
        public Vector3 Pos { get; private set; }
        float width, height, depth;
        float pitch;
        float roll = 0;
        float yaw = 0;
        protected Prism hitbox;
        public Prism AABB
        {
            get
            {
                //barriers.MaxBy(t => t.AABB);
                return new Prism(Pos, width, height + MathF.Sin(pitch) * depth, depth);
            }
        }

        public Box(Vector3 pos, float width, float height, float length, ConsoleColor col, float pitch = 0)
        {
            this.pitch = pitch;
            this.Pos = pos;
            this.width = width;
            this.height = height;
            this.depth = length;
            MidPoint = new Vector3(pos.X + width / 2, pos.Y + length / 2, pos.Z + width / 2);
            Vector3 pt1 = pos;
            Vector3 pt2 = pos + new Vector3(width, 0, 0);
            Vector3 pt3 = pos + new Vector3(0, MathF.Sin(pitch) * length, length);
            Vector3 pt4 = pos + new Vector3(width, MathF.Sin(pitch) * length, length);
            Vector3 pt5 = pos + new Vector3(0, height, 0);
            Vector3 pt6 = pos + new Vector3(width, height, 0);
            Vector3 pt7 = pos + new Vector3(0, height + MathF.Sin(pitch) * length, length);
            Vector3 pt8 = pos + new Vector3(width, height + MathF.Sin(pitch) * length, length);


            barriers[0] = new Barrier(pt1, pt2, pt3, pt4, col); // bottom plane
            barriers[1] = new Barrier(pt1, pt2, pt5, pt6, col); // front plane
            barriers[2] = new Barrier(pt1, pt3, pt5, pt7, col); // left plane
            barriers[3] = new Barrier(pt2, pt4, pt6, pt8, col); // right plane
            barriers[4] = new Barrier(pt3, pt4, pt7, pt8, col); // back plane
            barriers[5] = new Barrier(pt5, pt6, pt7, pt8, col); // top plane

            // Build hitbox from actual corners: right=pt2, up=pt5, depth=pt3 relative to pt1
            hitbox = new Prism(pt1, pt2, pt5, pt3);
        }

        protected void UpdatePos(Vector3 deltaPos, float deltaRoll, float deltaPitch, float deltaYaw)
        {
            Pos += deltaPos;
            roll += deltaRoll;
            pitch += deltaPitch;
            yaw += deltaYaw;
            hitbox = new Prism(Pos, width, height, depth, roll, pitch, yaw);
        }
        protected void UpdatePos(Vector3 pos)
        {
            this.Pos = pos;
            hitbox.origin = pos;
        }

        public (bool collides, Vector3 dirOut, float penetration) CollidesWith(Prism prism)
        {
            if (!AABB.AABBIntersects(prism))
            {
                return (false, Vector3.Zero, 0);
            }

            return hitbox.SATCollision(prism);
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
