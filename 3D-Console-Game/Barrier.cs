using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal class Barrier : IDrawable, ICollidable
    {

        Vector3 TopLeft;
        Vector3 TopRight;
        Vector3 BottomLeft;
        Vector3 BottomRight;

        private Vector3 Normal
        {
            get
            {
                return Vector3.Cross((BottomLeft - TopLeft), (TopRight - TopLeft));
            }
        }

        public Vector3 GetNormalFromPoint(Vector3 point)
        {
            Vector3 norm = Normal;
            if (Vector3.Dot(norm, point - TopLeft) > 0)
            {
                return norm;
            }
            else
            {
                return -norm;
            }
        }
        public Vector3 MidPoint
        {
            get
            {
                return (TopLeft + TopRight + BottomRight + BottomLeft) * 0.25f;
            }
        }

        public Plane plane
        {
            get
            {
                return new Plane(Normal, MidPoint.Length());
            }
        }

        ConsoleColor color;

        /// <summary>
        /// Rectangle between two points
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="color"></param>
        public Barrier(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight, ConsoleColor color)
        {
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            TopLeft = topLeft;
            TopRight = topRight;
            this.color = color;
        }

        public Prism AABB
        {
            get
            {
                float maxX = Math.Max(TopLeft.X, Math.Max(TopRight.X, Math.Max(BottomLeft.X, BottomRight.X)));
                float maxY = Math.Max(TopLeft.Y, Math.Max(TopRight.Y, Math.Max(BottomLeft.Y, BottomRight.Y)));
                float maxZ = Math.Max(TopLeft.Z, Math.Max(TopRight.Z, Math.Max(BottomLeft.Z, BottomRight.Z)));
                float minX = Math.Min(TopLeft.X, Math.Min(TopRight.X, Math.Min(BottomLeft.X, BottomRight.X)));
                float minY = Math.Min(TopLeft.Y, Math.Min(TopRight.Y, Math.Min(BottomLeft.Y, BottomRight.Y)));
                float minZ = Math.Min(TopLeft.Z, Math.Min(TopRight.Z, Math.Min(BottomLeft.Z, BottomRight.Z)));
                return new Prism(new Vector3(minX, minY, minZ), maxX - minX, maxY - minY, maxZ - minZ);
            }
        }

        /// <summary>
        /// Returns true on collision with Vector3 being the normal from surface
        /// </summary>
        public (bool, Vector3) CollidesWith(Prism prism)
        {
            // first do aabbs cause its cheap relatively
            Prism aabb = AABB;

            //TEMP: only use aabb for now
            if (aabb.Intersects(prism))
            {
                return (true, GetNormalFromPoint(prism.MidPoint));
            }


            // then do point check to see if the points of rect are inside the prism
            if (aabb.Contains(TopLeft) || aabb.Contains(TopRight) || aabb.Contains(BottomRight) || aabb.Contains(BottomLeft))
            {
                return (true, GetNormalFromPoint(prism.MidPoint));
            }

            // then go to axis projection
            

           



            return (false, Vector3.Zero);
        }

        public void Draw(Display display)
        {
            display.DrawSquare(new Vector4(TopLeft, 1), new Vector4(TopRight, 1), new Vector4(BottomLeft, 1), new Vector4(BottomRight, 1), color);
            //display.DrawSquare(new Vector4(BottomLeft, 1), new Vector4(BottomRight, 1), new Vector4(TopLeft, 1), new Vector4(TopRight, 1), color);
        }
    }
}
