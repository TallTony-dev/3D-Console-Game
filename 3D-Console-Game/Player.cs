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
                Vector3 pos = playerPos - new Vector3(width / 2, 0, depth / 2);
                return new Prism(pos, width, height, depth);
            }
        }
        Quaternion view = Quaternion.Identity;
        float yaw = 0f;
        float pitch = 0f;
        Vector3 camPos = Vector3.Zero;
        Vector3 playerPos = new Vector3(7,0,0); 
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
                if (obj is ICollidable collidable)
                {
                    (bool collides, Vector3 dirOut, float penetration) = collidable.CollidesWith(Hitbox);
                    if (collides)
                    {
                        if (obj is PhysicsBox phys)
                        {
                            phys.CollideWithPhysics(-dirOut, penetration);
                        }

                        if (dirOut.Y > 0.7f)
                        {
                            isTouchingGround = true;
                        }

                        if (penetration > 0)
                        {
                            playerPos += dirOut * (penetration + 0.001f);
                        }

                        // Remove velocity component along the resolution direction
                        float velAlongNormal = Vector3.Dot(playerVel, dirOut);
                        if (velAlongNormal < 0)
                        {
                            playerVel -= velAlongNormal * dirOut;
                        }
                    }
                }
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
