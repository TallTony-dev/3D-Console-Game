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
        protected float pitch { get; private set; }
        protected float roll { get; private set; }
        protected float yaw { get; private set; }

        ConsoleColor col;
        protected Prism hitbox;

        public Box(Vector3 pos, float width, float height, float length, ConsoleColor col, float pitch = 0, float roll = 0, float yaw = 0)
        {
            this.yaw = yaw;
            this.roll = roll;
            this.pitch = pitch;
            this.Pos = pos;
            this.width = width;
            this.height = height;
            this.depth = length;
            this.col = col;
            hitbox = new Prism(pos, width, height, length, roll, pitch, yaw);
            RebuildBarriers();
        }

        protected void UpdatePos(Vector3 deltaPos, float deltaRoll, float deltaPitch, float deltaYaw)
        {
            Pos += deltaPos;
            roll += deltaRoll;
            pitch += deltaPitch;
            yaw += deltaYaw;
            hitbox = new Prism(Pos, width, height, depth, roll, pitch, yaw);
            RebuildBarriers();
        }
        protected void UpdatePos(Vector3 pos)
        {
            this.Pos = pos;
            hitbox.origin = pos;
            RebuildBarriers();
        }

        private void RebuildBarriers()
        {
            Vector3 pos = Pos;
            float length = depth;

            Quaternion rot = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);

            Vector3 pt1 = pos; // front bottom left
            Vector3 pt2 = pos + Vector3.Transform(new Vector3(width, 0, 0), rot); //front bottom right
            Vector3 pt3 = pos + Vector3.Transform(new Vector3(0, 0, length), rot); // back bottom left
            Vector3 pt4 = pos + Vector3.Transform(new Vector3(width, 0, length), rot); // back bottom right
            Vector3 pt5 = pos + Vector3.Transform(new Vector3(0, height, 0), rot); // front top left
            Vector3 pt6 = pos + Vector3.Transform(new Vector3(width, height, 0), rot); // front top right
            Vector3 pt7 = pos + Vector3.Transform(new Vector3(0, height, length), rot); // back top left
            Vector3 pt8 = pos + Vector3.Transform(new Vector3(width, height, length), rot); // back top right

            barriers[0] = new Barrier(pt1, pt2, pt3, pt4, col);
            barriers[1] = new Barrier(pt1, pt2, pt5, pt6, col);
            barriers[2] = new Barrier(pt1, pt3, pt5, pt7, col);
            barriers[3] = new Barrier(pt2, pt4, pt6, pt8, col);
            barriers[4] = new Barrier(pt3, pt4, pt7, pt8, col);
            barriers[5] = new Barrier(pt5, pt6, pt7, pt8, col);

            MidPoint = hitbox.MidPoint;
        }

        public (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) CollidesWith(Prism prism)
        {
            if (!hitbox.AABBIntersects(prism))
            {
                return (false, Vector3.Zero, 0, Vector3.Zero);
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
