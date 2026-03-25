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
        private const float Bounciness = 1f;
        private const float AirFriction = 1f;
        public void CollideWithPhysics(Vector3 forceDir, float strength, Vector3 collisionPoint)
        {
            Vector3 thisMid = hitbox.MidPoint;
            Vector3 force = forceDir * strength;

            Vector3 tangentialVelocity = velocity - Vector3.Dot(velocity, forceDir) * forceDir;

            

            Vector3 r = collisionPoint - thisMid;
            Vector3 torque = Vector3.Cross(r, tangentialVelocity + force);

            velocityYaw -= torque.Y;
            velocityPitch -= torque.X;
            velocityRoll -= torque.Z;


            velocity += force;
        }

        public void Update(double deltaTime)
        {
            float dt = (float)deltaTime;

            velocity.Y -= Gravity * dt;
            velocity.X -= Math.Sign(velocity.X) * Friction * dt;
            velocity.Z -= Math.Sign(velocity.Z) * Friction * dt;
            velocityRoll -= (velocityRoll * 5) * AirFriction * dt;
            velocityPitch -= (velocityPitch * 5) * AirFriction * dt;
            velocityYaw -= (velocityYaw * 5) * AirFriction * dt;


            //UpdatePos(Pos + velocity * dt);
            UpdatePos(velocity * dt, velocityRoll * dt, velocityPitch * dt, velocityYaw * dt);
            CheckCollision();

            //if (Pos.Y < 0)
            //{
            //    UpdatePos(Pos * new Vector3(1, 0, 1));
            //    velocity.Y = 0;
            //}
        }

        private void CheckCollision()
        {
            foreach (object obj in Game.activeWalls)
            {
                if (obj is ICollidable collidable && obj != this)
                {
                    (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = collidable.CollidesWith(hitbox);
                    if (collides)
                    {
                        if (penetration > 0)
                        {
                            UpdatePos(Pos + dirOut * (penetration + 0.001f));
                        }

                        //some slop here
                        // Remove velocity component along the resolution direction
                        float velAlongNormal = Vector3.Dot(velocity, dirOut);
                        if (velAlongNormal < 0)
                        {
                            velocity -= velAlongNormal * dirOut * Bounciness;
                        }

                        // Apply torque to this box from the collision
                        CollideWithPhysics(dirOut, penetration * 100, collisionPoint);

                        if (obj is PhysicsBox phys)
                        {
                            phys.CollideWithPhysics(-dirOut, penetration * 100, collisionPoint);
                        }
                    }
                }
            }
        }
    }
}
