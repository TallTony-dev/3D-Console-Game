using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal static class InputManager
    {

        static ConsoleKeyInfo downKey = default;
        static ConsoleKeyInfo pressedKey = default;
        static ConsoleKeyInfo previousKey = default;

        public static bool IsKeyPressed(ConsoleKey key)
        {
            return key == pressedKey.Key;
        }
        public static bool IsKeyDown(ConsoleKey key)
        {
            return key == downKey.Key;
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
