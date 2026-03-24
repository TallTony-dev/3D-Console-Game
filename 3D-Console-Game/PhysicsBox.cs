using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal class PhysicsBox : Box
    {
        public PhysicsBox(Vector3 pos, float width, float height, float length, ConsoleColor col, float pitch = 0) : base(pos, width, height, length, col, pitch)
        {

        }

        private Vector3 velocity;
        private float velocityRoll;
        private float velocityPitch;
        private float velocityYaw;

        public void CollideWithPhysics(Vector3 forceDir, float strength)
        {
            velocity += forceDir * strength;
        }

        public void Update(double deltaTime)
        {
            UpdatePos(velocity * (float)deltaTime, velocityRoll * (float)deltaTime, velocityPitch * (float)deltaTime, velocityYaw * (float)deltaTime);
        }

        private void CheckCollision()
        {
            foreach (object obj in Game.activeWalls)
            {
                if (obj is ICollidable collidable && obj != this)
                {
                    (bool collides, Vector3 dirOut, float penetration) = collidable.CollidesWith(hitbox);
                    if (collides)
                    {
                        if (penetration > 0)
                        {
                            UpdatePos(Pos + dirOut * (penetration + 0.001f));
                        }

                        // Remove velocity component along the resolution direction
                        float velAlongNormal = Vector3.Dot(velocity, dirOut);
                        if (velAlongNormal < 0)
                        {
                            //try bouncing too?
                            float bounciness = 1.8f;
                            velocity -= velAlongNormal * dirOut * bounciness;
                        }
                    }
                }
            }
        }
    }
}
