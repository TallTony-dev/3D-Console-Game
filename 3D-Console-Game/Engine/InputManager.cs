using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Engine
{
    internal static class InputManager
    {

        static ConsoleKeyInfo downKey = default;
        static ConsoleKeyInfo pressedKey = default;
        static ConsoleKeyInfo previousKey = default;

        static Point currentPos;
        static Vector2 mouseDelta;
        static bool isLeftMousePressed = false;
        static bool isLeftMouseDown = false;
        static bool lockMousePos = false;

        //static IntPtr consoleHandle = GetConsoleWindow();
        //public static float consoleWidth;
        //public static float consoleHeight;

        public static bool IsLeftMouseDown()
        {
            return isLeftMouseDown;
        }
        public static bool IsLeftMousePressed()
        {
            return isLeftMousePressed;
        }
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
        public static void LockMousePos() { lockMousePos = true; }
        public static void UnlockMousePos() { lockMousePos = false; }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        //[DllImport("user32.dll")]
        //private static extern int ShowCursor(bool bShow);

        //[StructLayout(LayoutKind.Sequential)]
        //public struct RECT
        //{
        //    public int Left;
        //    public int Top;
        //    public int Right;
        //    public int Bottom;
        //}

        //[DllImport("kernel32.dll", SetLastError = true)]
        //public static extern IntPtr GetConsoleWindow();

        //[DllImport("user32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static void UpdateMousePos()
        {
            if (InputManager.IsCharPressedAsync(0x01))
            {
                if (!isLeftMouseDown && !isLeftMousePressed)
                {
                    isLeftMousePressed = true;
                }
                else if (isLeftMousePressed)
                    isLeftMousePressed = false;

                isLeftMouseDown = true;
            }
            else { isLeftMouseDown = false; }
            GetCursorPos(ref currentPos);

            if (lockMousePos)
            {
                mouseDelta = new Vector2(500 - currentPos.X, 500 - currentPos.Y);
                SetCursorPos(500, 500);
            }

            //GetWindowRect(consoleHandle, out RECT rect);
            //currentPos.X -= rect.Left;
            //currentPos.Y -= rect.Top;
            ////currentPos.X /= 12;
        }

        //public static void HideCursor()
        //{
        //    ShowCursor(false);
        //}
        //public static void ShowCursor()
        //{
        //    ShowCursor(true);
        //}
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
