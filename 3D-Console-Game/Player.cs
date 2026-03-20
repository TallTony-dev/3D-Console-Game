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
        public Prism hitbox = new Prism();
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

            bool isTouchingGround = playerPos.Y == 0;

            if (InputManager.IsCharPressedAsync('W'))
            {
                playerVel += forward * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, forward)) + 0.1f);
            }
            if (InputManager.IsCharPressedAsync('S'))
            {
                playerVel -= forward * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, forward)) + 0.1f);
            }
            if (InputManager.IsCharPressedAsync('A'))
            {
                playerVel += left * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, left)) + 0.1f);
            }
            if (InputManager.IsCharPressedAsync('D'))
            {
                playerVel -= left * moveSpeed * dt / (Math.Abs(Vector3.Dot(playerVel, left)) + 0.1f);
            }
            if (InputManager.IsCharPressedAsync(0x20) && isTouchingGround)
            {
                playerVel.Y = 3f;
            }


            playerVel -= new Vector3(Math.Sign(playerVel.X) * 2f * dt, dt * 4, Math.Sign(playerVel.Z) * 2f * dt);
            playerPos += Vector3.Multiply((float)deltaTime, playerVel);

            if (playerPos.Y < 0)
            {
                playerPos.Y = 0;
                playerVel.Y = 0;
            }

            camPos = playerPos + new Vector3(0, 1f, 0);

            Vector2 delta = InputManager.GetMouseDelta();
            
            yaw += rotSpeed * dt * delta.X / 20;
            pitch += rotSpeed * dt * delta.Y / 20;
            pitch = Math.Clamp(pitch, -MathF.PI / 2 + 0.01f, MathF.PI / 2 - 0.01f);

            view = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw) * Quaternion.CreateFromAxisAngle(Vector3.UnitX, pitch);


        }

    }
}
