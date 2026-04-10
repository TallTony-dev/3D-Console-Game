using _3D_Console_Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Engine
{
    internal class PhysicsBox : Box, IUpdatable
    {
        public PhysicsBox(Vector3 pos, float width, float height, float length, ConsoleColor col, float pitch = 0, bool isDamped = true) : base(pos, width, height, length, col, pitch)
        {
            this.isDamped = isDamped;
        }

        List<object> collidedObjects = new();
        public Vector3 velocity;
        float velocityRoll;
        float velocityPitch;
        float velocityYaw;

        public bool isDamped;

        private float Gravity = 8f;
        public float Friction = 2f;
        private const float Bounciness = 1f;
        private const float AirFriction = 1f;
        public void CollideWithPhysics(Vector3 forceDir, float strength, Vector3 collisionPoint)
        {
            Vector3 thisMid = hitbox.MidPoint;
            Vector3 force = forceDir * strength;
            Vector3 tangentialVelocity = velocity - Vector3.Dot(velocity, forceDir) * forceDir;
            Vector3 r = collisionPoint - thisMid;
            Vector3 torque = Vector3.Cross(r, tangentialVelocity + force);

            velocityYaw += torque.Y;
            velocityPitch += torque.X;
            velocityRoll += torque.Z;

            velocity += force;
        }
        public void SetVelocity(Vector3 xyz)
        {
            velocity = xyz;
        }

        public bool CollidedWithObject(object obj)
        {
            return collidedObjects.Contains(obj);
        }

        //LLM: subdivide physics steps to not clip anymore
        private const float MaxStepTime = 1f / 60f;

        public void Update(double deltaTime)
        {
            collidedObjects.Clear();
            float remaining = (float)deltaTime;

            while (remaining > 0f)
            {
                float dt = Math.Min(remaining, MaxStepTime);
                remaining -= dt;

                velocity.Y -= Gravity * dt;
                if (isDamped)
                {
                    velocity.X -= Math.Sign(velocity.X) * Friction * dt;
                    velocity.Z -= Math.Sign(velocity.Z) * Friction * dt;
                    velocityRoll -= velocityRoll * AirFriction * dt;
                    velocityPitch -= velocityPitch * AirFriction * dt;
                    velocityYaw -= velocityYaw * AirFriction * dt;
                }

                UpdatePos(velocity * dt, velocityRoll * dt, velocityPitch * dt, velocityYaw * dt);
                CheckCollision();
            }
        }

        private void CheckCollision()
        {
            foreach (object obj in Game.activeObjects)
            {
                if (obj is ICollidable collidable && obj != this && !(obj is Enemy enemy && enemy.Body == this))
                {
                    (bool collides, Vector3 dirOut, float penetration, Vector3 collisionPoint) = collidable.CollidesWith(hitbox);
                    if (collides)
                    {
                        collidedObjects.Add(obj);
                        if (penetration > 0)
                        {
                            SetPos(Pos + dirOut * (penetration + 0.001f));
                        }

                        //some slop here
                        // Remove velocity component along the resolution direction
                        float velAlongNormal = Vector3.Dot(velocity, dirOut);
                        if (velAlongNormal < 0)
                        {
                            velocity -= velAlongNormal * dirOut * Bounciness;
                        }

                        // Apply torque to this box from the collision
                        //CollideWithPhysics(dirOut, penetration * 100, collisionPoint);

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
