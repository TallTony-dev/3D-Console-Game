using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Engine
{
    internal class Box : IDrawable, ICollidable
    {
        Barrier[] barriers = new Barrier[6];
        public Vector3 MidPoint { get; private set; }
        public Vector3 Pos { get; private set; }

        float width, height, depth;
        public float pitch { get; private set; }
        public float roll { get; private set; }
        public float yaw { get; private set; }

        public ConsoleColor col;
        public Prism hitbox { get; protected set; }

        public bool isPickable = false;

        public Vector3 MidBottom
        {
            get
            {
                return Pos + Vector3.Transform(new Vector3(width / 2, 0, depth / 2), Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll));
            }
        }


        public Box(Vector3 pos, float width, float height, float length, ConsoleColor col, float pitch = 0, float roll = 0, float yaw = 0)
        {
            this.yaw = yaw;
            this.roll = roll;
            this.pitch = pitch;
            Pos = pos;
            this.width = width;
            this.height = height;
            depth = length;
            this.col = col;
            hitbox = new Prism(pos, width, height, length, roll, pitch, yaw);
            RebuildBarriers();
        }

        public Box(Vector3 pos, float width, float height, float length, ConsoleColor col, Vector3 dir) :
            this(pos, width, height, length, col, -MathF.Asin(dir.Y), 0, MathF.Atan2(dir.X, dir.Z))
        {

        }

        
        public void UpdatePos(Vector3 deltaPos, float deltaRoll = 0, float deltaPitch = 0, float deltaYaw = 0)
        {
            Pos += deltaPos;
            roll += deltaRoll;
            pitch += deltaPitch;
            yaw += deltaYaw;
            hitbox = new Prism(Pos, width, height, depth, roll, pitch, yaw);
            RebuildBarriers();
        }

        public void UpdateSize(float deltaWidth, float deltaHeight, float deltaDepth)
        {
            width += deltaWidth;
            height += deltaHeight;
            depth += deltaDepth;
            hitbox = new Prism(Pos, width, height, depth, roll, pitch, yaw);
            RebuildBarriers();
        }
        public void SetSize(float width, float height, float depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            hitbox = new Prism(Pos, width, height, depth, roll, pitch, yaw);
            RebuildBarriers();
        }

        public void SetPos(Vector3 pos)
        {
            Pos = pos;
            hitbox.origin = pos;
            RebuildBarriers();
        }
        public void SetPos(Vector3 pos, Vector3 dir)
        {
            SetPos(pos, -MathF.Asin(dir.Y), 0, MathF.Atan2(dir.X, dir.Z));
        }
        public void SetPos(Vector3 pos, float pitch, float roll, float yaw)
        {
            Pos = pos;
            hitbox.origin = pos;
            this.roll = roll;
            this.pitch = pitch;
            this.yaw = yaw;
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
