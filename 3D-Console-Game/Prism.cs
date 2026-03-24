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
        public Vector3 origin; //from front bottom left
        public float width;
        public float height;
        public float depth;
        public Quaternion rot = Quaternion.Identity;

        // Edge vectors from origin (supports sheared geometry)
        private Vector3 edgeRight;
        private Vector3 edgeUp;
        private Vector3 edgeDepth;

        private void ComputeEdges()
        {
            edgeRight = Vector3.Transform(new Vector3(width, 0, 0), rot);
            edgeUp = Vector3.Transform(new Vector3(0, height, 0), rot);
            edgeDepth = Vector3.Transform(new Vector3(0, 0, depth), rot);
        }

        public Vector3 FrontBottomLeft
        { get { return origin; } }
        public Vector3 FrontBottomRight
        { get { return origin + edgeRight; } }
        public Vector3 FrontTopLeft
        { get { return origin + edgeUp; } }
        public Vector3 FrontTopRight
        { get { return origin + edgeRight + edgeUp; } }
        public Vector3 BackBottomLeft
        { get { return origin + edgeDepth; } }
        public Vector3 BackBottomRight
        { get { return origin + edgeRight + edgeDepth; } }
        public Vector3 BackTopLeft
        { get { return origin + edgeUp + edgeDepth; } }
        public Vector3 BackTopRight
        { get { return origin + edgeRight + edgeUp + edgeDepth; } }


        public Prism() 
        {
            edgeRight = Vector3.Zero;
            edgeUp = Vector3.Zero;
            edgeDepth = Vector3.Zero;
        }
        public Prism(Vector3 pos, float width, float height, float depth, float roll = 0, float pitch = 0, float yaw = 0)
        {
            this.origin = pos;
            this.width = width;
            this.height = height;
            this.depth = depth;
            rot = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            ComputeEdges();
        }
        public Prism(Vector3 frontBottomLeft, Vector3 frontBottomRight, Vector3 frontTopLeft, Vector3 backBottomLeft)
        {
            origin = frontBottomLeft;
            edgeRight = frontBottomRight - frontBottomLeft;
            edgeUp = frontTopLeft - frontBottomLeft;
            edgeDepth = backBottomLeft - frontBottomLeft;
            width = edgeRight.Length();
            height = edgeUp.Length();
            depth = edgeDepth.Length();
        }


        public Vector3 MidPoint
        {
            get
            {
                return origin + (edgeRight + edgeUp + edgeDepth) * 0.5f;
            }
        }


        public (bool collides, Vector3 dirOut, float penetration) SATCollision(Prism b)
        {
            Vector3[] cornersA = { FrontBottomLeft, FrontBottomRight, FrontTopLeft, FrontTopRight, BackBottomLeft, BackBottomRight, BackTopLeft, BackTopRight };
            Vector3[] cornersB = { b.FrontBottomLeft, b.FrontBottomRight, b.FrontTopLeft, b.FrontTopRight, b.BackBottomLeft, b.BackBottomRight, b.BackTopLeft, b.BackTopRight };

            Vector3[] axes = new Vector3[15];
            // Face normals from edge cross products for prism A
            axes[0] = Vector3.Cross(edgeUp, edgeDepth);
            axes[1] = Vector3.Cross(edgeDepth, edgeRight);
            axes[2] = Vector3.Cross(edgeRight, edgeUp);
            // Face normals from edge cross products for prism B
            axes[3] = Vector3.Cross(b.edgeUp, b.edgeDepth);
            axes[4] = Vector3.Cross(b.edgeDepth, b.edgeRight);
            axes[5] = Vector3.Cross(b.edgeRight, b.edgeUp);

            // Edge-edge cross products
            Vector3[] edgesA = { edgeRight, edgeUp, edgeDepth };
            Vector3[] edgesB = { b.edgeRight, b.edgeUp, b.edgeDepth };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    axes[6 + i * 3 + j] = Vector3.Cross(edgesA[i], edgesB[j]);
                }
            }

            int smallestOverlapAxis = -1;
            float smallestOverlap = float.MaxValue;
            for (int i = 0; i < 15; i++)
            {
                Vector3 axis = axes[i];

                if (axis.LengthSquared() < 1e-6f)
                    continue;

                axis = Vector3.Normalize(axis);
                axes[i] = axis;

                float minA = Vector3.Dot(cornersA[0], axis);
                float maxA = minA;
                float minB = Vector3.Dot(cornersB[0], axis);
                float maxB = minB;

                for (int c = 1; c < 8; c++)
                {
                    float projA = Vector3.Dot(cornersA[c], axis);
                    if (projA < minA) minA = projA;
                    else if (projA > maxA) maxA = projA;

                    float projB = Vector3.Dot(cornersB[c], axis);
                    if (projB < minB) minB = projB;
                    else if (projB > maxB) maxB = projB;
                }

                float overlap = Math.Min(maxA - minB, maxB - minA);

                if (overlap <= 0) 
                    return (false, Vector3.Zero, 0);

                if (overlap < smallestOverlap)
                {
                    smallestOverlap = overlap;
                    smallestOverlapAxis = i;
                }
            }

            float dir = Vector3.Dot(this.MidPoint - b.MidPoint, axes[smallestOverlapAxis]);
            if (dir > 0)
            {
                axes[smallestOverlapAxis] = -axes[smallestOverlapAxis];
            }

            return (true, Vector3.Normalize(axes[smallestOverlapAxis]), smallestOverlap);
        }

        public bool AABBIntersects(Prism prism)
        {
            return (prism.origin.X < origin.X + width && origin.X < prism.origin.X + prism.width
                && prism.origin.Y < origin.Y + height && origin.Y < prism.origin.Y + prism.height
                && prism.origin.Z < origin.Z + depth && origin.Z < prism.origin.Z + prism.depth);
        }
        public bool AABBContains(Vector3 point)
        {
            return (point.X < origin.X + width && origin.X < point.X
                && point.Y < origin.Y + height && origin.Y < point.Y
                && point.Z < origin.Z + depth && origin.Z < point.Z);
        }
    }
}
