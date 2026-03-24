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
        float velocityRoll;
        float velocityPitch;
        float velocityYaw;

        private const float Gravity = 8f;
        private const float Friction = 2f;
        private const float Bounciness = 1.8f;

        public void CollideWithPhysics(Vector3 forceDir, Vector3 forceSource, float strength)
        {
            //theoretically dont need forcedir once you have forceSource here




            velocity += forceDir * strength;
        }

        public void Update(double deltaTime)
        {
            float dt = (float)deltaTime;


            velocity.Y -= Gravity * dt;
            velocity.X -= Math.Sign(velocity.X) * Friction * dt;
            velocity.Z -= Math.Sign(velocity.Z) * Friction * dt;

            UpdatePos(Pos + velocity * dt);
            CheckCollision();

            if (Pos.Y < 0)
            {
                UpdatePos(Pos * new Vector3(1, 0, 1));
                velocity.Y = 0;
            }
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
                            velocity -= velAlongNormal * dirOut * Bounciness;
                        }

                        if (obj is PhysicsBox phys)
                        {
                            phys.CollideWithPhysics(-dirOut, hitbox.MidPoint, penetration * 100);
                        }
                    }
                }
            }
        }
    }
}
