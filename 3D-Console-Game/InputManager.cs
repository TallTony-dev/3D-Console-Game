using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal static class InputManager
    {

        static ConsoleKeyInfo downKey = default;
        static ConsoleKeyInfo pressedKey = default;
        static ConsoleKeyInfo previousKey = default;

        static Point currentPos;
        static Vector2 mouseDelta;
        public static bool IsKeyPressed(ConsoleKey key)
        {
            return key == pressedKey.Key;
        }
        public static bool IsKeyDown(ConsoleKey key)
        {
            return key == downKey.Key;
        }

        public static bool IsCharPressedAsync(int c)
        {
            return GetAsyncKeyState(c) < 0;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        

        public static void UpdateMousePos()
        {
            GetCursorPos(ref currentPos);
            mouseDelta = new Vector2(500 - currentPos.X, 500 - currentPos.Y);
            SetCursorPos(500, 500);
        }

        public static Point GetMousePos()
        {
            return currentPos;
        }
        public static Vector2 GetMouseDelta()
        {
            return mouseDelta;
        }

        public static void UpdateKey()
        {
            if (Console.KeyAvailable)
            {
                downKey = Console.ReadKey(true);
                if (downKey.Key != previousKey.Key)
                {
                    pressedKey = downKey;
                }
                else
                {
                    pressedKey = default;
                }
                previousKey = downKey;

                while (Console.KeyAvailable) { Console.ReadKey(true); } //clear buf
            }
            else
            {
                downKey = default;
                pressedKey = default;
                previousKey = default;
            }
        }
                

    }
}
