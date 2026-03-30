using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Hudstuff
{
    internal static class HUD
    {
        static List<HudElement> hudElements = new List<HudElement>();


        public static void InitializeHud()
        {
            hudElements.Add(new HealthBar());
        }

        public static void DrawHUD((char c, ConsoleColor col)[,] display)
        {
            foreach (var element in hudElements)
            {
                element.Draw(display);
            }
        }

    }
}
