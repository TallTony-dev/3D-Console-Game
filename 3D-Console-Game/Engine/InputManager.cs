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
        public static Point relativeMousePos;
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

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        //[DllImport("user32.dll")]
        //private static extern int ShowCursor(bool bShow);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        //[DllImport("kernel32.dll", SetLastError = true)]
        //public static extern IntPtr GetConsoleWindow();

        //[DllImport("user32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static void UpdateMousePos()
        {
            IntPtr consoleWindowHandle = GetForegroundWindow();
            Rect screenRect;
            GetWindowRect(consoleWindowHandle, out screenRect);

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

            float widthScale = (float)(screenRect.Right - screenRect.Left) / Console.WindowWidth;
            float heightScale = (float)(screenRect.Bottom - screenRect.Top) / Console.WindowHeight;
            relativeMousePos = new Point((int)((currentPos.X - screenRect.Left) / widthScale),(int)((currentPos.Y - screenRect.Top) / heightScale));

            Point center = new Point((screenRect.Left + screenRect.Right) / 2, (screenRect.Top + screenRect.Bottom) / 2);
            if (lockMousePos)
            {
                mouseDelta = new Vector2(center.X - currentPos.X, center.Y - currentPos.Y);
                SetCursorPos(center.X, center.Y);
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
