using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal class Player
    {
        float width = 0.8f;
        float height = 2;
        float depth = 0.8f;
        public Prism Hitbox
        {
            get
            {
                Prism prism = new Prism();
                prism.pos = playerPos;
                prism.pos -= new Vector3(width / 2, 0, depth / 2);
                prism.width = width;
                prism.height = height;
                prism.depth = depth;
                return prism;
            }
        }
        Quaternion view = Quaternion.Identity;
        float yaw = 0f;
        float pitch = 0f;
        Vector3 camPos = Vector3.Zero;
        Vector3 playerPos = Vector3.Zero; 
        public Matrix4x4 ViewMatrix { 
            get {
                // View matrix = inverse of camera transform
                // First undo rotation (conjugate), then undo translation (negate)
                Matrix4x4 rot = Matrix4x4.CreateFromQuaternion(Quaternion.Conjugate(view));
                Matrix4x4 trans = Matrix4x4.CreateTranslation(-camPos);
                Matrix4x4 vertInvert = Matrix4x4.Identity;
                vertInvert.M22 = -1;
                return trans * rot * vertInvert;
            } 
        }

        float moveSpeed = 5f;
        float rotSpeed = 2f;

        Vector3 playerVel;

        public void Update(double deltaTime)
        {
            float dt = (float)deltaTime;
            Vector3 forward = Vector3.Transform(-Vector3.UnitZ, view);
            Vector3 left = Vector3.Transform(-Vector3.UnitX, view);

            Vector3 xzForward = Vector3.Normalize(forward * new Vector3(1, 0, 1));
            Vector3 xzLeft = Vector3.Normalize(left * new Vector3(1, 0, 1));

            bool isTouchingGround = playerPos.Y == 0;

            

            if (InputManager.IsCharPressedAsync('W'))
            {
                playerVel += xzForward * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, xzForward)) + 0.1f);
            }
            if (InputManager.IsCharPressedAsync('S'))
            {
                playerVel -= xzForward * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, xzForward)) + 0.1f);
            }
            if (InputManager.IsCharPressedAsync('A'))
            {
                playerVel += xzLeft * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, xzLeft)) + 0.1f);
            }
            if (InputManager.IsCharPressedAsync('D'))
            {
                playerVel -= xzLeft * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, xzLeft)) + 0.1f);
            }


            playerVel -= new Vector3(Math.Sign(playerVel.X) * 2f * dt, dt * 8, Math.Sign(playerVel.Z) * 2f * dt);
            playerPos += Vector3.Multiply((float)deltaTime, playerVel);

            if (playerPos.Y < 0)
            {
                playerPos.Y = 0;
                playerVel.Y = 0;
            }

            Prism hitbox = Hitbox;
            foreach (object obj in Game.activeWalls)
            {
                Task.Run(() =>
                {
                    if (obj is ICollidable collidable)
                    {
                        (bool collides, Vector3 norm) = collidable.CollidesWith(Hitbox);
                        if (collides)
                        {
                            hitbox = Hitbox;
                            Vector3 hitboxMin = hitbox.pos;
                            Vector3 hitboxMax = hitbox.pos + new Vector3(hitbox.width, hitbox.height, hitbox.depth);

                            Prism aabb = collidable.AABB;
                            Vector3 aabbMin = aabb.pos;
                            Vector3 aabbMax = aabb.pos + new Vector3(aabb.width, aabb.height, aabb.depth);

                            // Compute overlap on each axis
                            float overlapX = Math.Min(hitboxMax.X - aabbMin.X, aabbMax.X - hitboxMin.X);
                            float overlapY = Math.Min(hitboxMax.Y - aabbMin.Y, aabbMax.Y - hitboxMin.Y);
                            float overlapZ = Math.Min(hitboxMax.Z - aabbMin.Z, aabbMax.Z - hitboxMin.Z);

                            if (overlapX <= 0 || overlapY <= 0 || overlapZ <= 0)
                                return;

                            // Resolve along the axis of minimum penetration (MTV)
                            Vector3 resolution;
                            float penetration;
                            Vector3 hitboxCenter = hitbox.MidPoint;
                            Vector3 aabbCenter = aabb.MidPoint;

                            if (overlapX <= overlapY && overlapX <= overlapZ)
                            {
                                float sign = Math.Sign(hitboxCenter.X - aabbCenter.X);
                                if (sign == 0) sign = 1;
                                resolution = new Vector3(sign, 0, 0);
                                penetration = overlapX;
                            }
                            else if (overlapY <= overlapX && overlapY <= overlapZ)
                            {
                                float sign = Math.Sign(hitboxCenter.Y - aabbCenter.Y);
                                if (sign == 0) sign = 1;
                                resolution = new Vector3(0, sign, 0);
                                penetration = overlapY;
                            }
                            else
                            {
                                float sign = Math.Sign(hitboxCenter.Z - aabbCenter.Z);
                                if (sign == 0) sign = 1;
                                resolution = new Vector3(0, 0, sign);
                                penetration = overlapZ;
                            }

                            if (resolution.Y > 0.7f)
                            {
                                isTouchingGround = true;
                            }

                            if (penetration > 0)
                            {
                                playerPos += resolution * (penetration + 0.001f);
                            }

                            // Remove velocity component along the resolution direction
                            float velAlongNormal = Vector3.Dot(playerVel, resolution);
                            if (velAlongNormal < 0)
                            {
                                playerVel -= velAlongNormal * resolution;
                            }
                        }
                    }
                });
            }

            if (InputManager.IsCharPressedAsync(0x20) && isTouchingGround)
            {
                playerVel.Y = 5f;
            }

            camPos = playerPos + new Vector3(0, 1.3f, 0);

            Vector2 delta = InputManager.GetMouseDelta();
            
            yaw += rotSpeed * dt * delta.X / 20;
            pitch += rotSpeed * dt * delta.Y / 20;
            pitch = Math.Clamp(pitch, -MathF.PI / 2 + 0.01f, MathF.PI / 2 - 0.01f);

            view = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, pitch);


        }

    }
}
