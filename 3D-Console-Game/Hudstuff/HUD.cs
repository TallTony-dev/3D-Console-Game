using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Hudstuff
{
    internal static class HUD
    {
        static List<HudElement> inGameHudElements = new List<HudElement>();
        static List<HudElement> pauseMenuHudElements = new List<HudElement>();
        static List<HudElement> mainMenuHudElements = new List<HudElement>();


        public static void InitializeHud()
        {
            inGameHudElements.Add(new HealthBar());
            inGameHudElements.Add(Program.game.player.inventory);

            pauseMenuHudElements.Add(new PauseElement());

            mainMenuHudElements.Add(new PauseElement());
        }

        public static void Update(float dt)
        {
            if (Program.game.state == Game.GameState.inGame) 
                foreach (var element in inGameHudElements) { element.Update(dt); }
            else if (Program.game.state == Game.GameState.Paused) 
                foreach (var element in pauseMenuHudElements) { element.Update(dt); }
        }

        public static void DrawHUD((char c, ConsoleColor col)[,] display)
        {
            Game.GameState state = Program.game.state;
            switch (state)
            {
                case (Game.GameState.inGame):
                    foreach (var element in inGameHudElements) { element.Draw(display); }
                    break;
                case (Game.GameState.Paused):
                    foreach (var element in pauseMenuHudElements) { element.Draw(display); }
                    break;
                case (Game.GameState.Menu):
                    foreach (var element in mainMenuHudElements) { element.Draw(display); }
                    break;

            }
        }

    }
}
