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
        Vector3 camPos = Vector3.Zero;
        Vector3 playerPos = Vector3.Zero; 
        public Matrix4x4 ViewMatrix { 
            get {
                // View matrix = inverse of camera transform
                // First undo rotation (conjugate), then undo translation (negate)
                Matrix4x4 rot = Matrix4x4.CreateFromQuaternion(Quaternion.Conjugate(view));
                Matrix4x4 trans = Matrix4x4.CreateTranslation(-camPos);
                return trans * rot;
            } 
        }

        float moveSpeed = 5f;
        float rotSpeed = 2f;

        public void Update(double deltaTime)
        {
            float dt = (float)deltaTime;
            Vector3 forward = Vector3.Transform(-Vector3.UnitZ, view);
            Vector3 left = Vector3.Transform(-Vector3.UnitX, view);

            if (InputManager.IsKeyDown(ConsoleKey.W))
            {
                playerPos += forward * moveSpeed * dt;
            }
            if (InputManager.IsKeyDown(ConsoleKey.S))
            {
                playerPos -= forward * moveSpeed * dt;
            }
            if (InputManager.IsKeyDown(ConsoleKey.A))
            {
                playerPos += left * moveSpeed * dt;
            }
            if (InputManager.IsKeyDown(ConsoleKey.D))
            {
                playerPos -= left * moveSpeed * dt;
            }

            camPos = playerPos + new Vector3(0, 0.2f, 0);

            if (InputManager.IsKeyDown(ConsoleKey.UpArrow))
            {
                view = Quaternion.Multiply(view, Quaternion.CreateFromYawPitchRoll(0, -rotSpeed * dt, 0));
            }
            if (InputManager.IsKeyDown(ConsoleKey.DownArrow))
            {
                view = Quaternion.Multiply(view, Quaternion.CreateFromYawPitchRoll(0, rotSpeed * dt, 0));
            }
            if (InputManager.IsKeyDown(ConsoleKey.RightArrow))
            {
                view = Quaternion.Multiply(view, Quaternion.CreateFromYawPitchRoll(-rotSpeed * dt, 0, 0));
            }
            if (InputManager.IsKeyDown(ConsoleKey.LeftArrow))
            {
                view = Quaternion.Multiply(view, Quaternion.CreateFromYawPitchRoll(rotSpeed * dt, 0, 0));
            }

        }

    }
}
