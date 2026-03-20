using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game
{
    internal class Game
    {
        public Game() { display = new Display(50, 50); }

        public Player player = new Player();

        List<Wall> activeWalls = new List<Wall>();
        
        double timeCount = 30;
        int score = 0;

        GameState state = GameState.Menu;
        public void UpdateGame(double deltaTime)
        {
            if (state == GameState.inGame)
            {
                display.Update(deltaTime, player);
                player.Update(deltaTime);
                //timeCount += deltaTime;

                if (timeCount > 3)
                {
                    activeWalls.Add(new Wall(1, new Vector3(0, 0, 0), new Vector3(1, 0, 0), ConsoleColor.Red));
                    activeWalls.Add(new Wall(1, new Vector3(0, 0, 0), new Vector3(0, 1, 0), ConsoleColor.Green));
                    activeWalls.Add(new Wall(1, new Vector3(0, 0, 0), new Vector3(0, 0, 1), ConsoleColor.Blue));


                }
                foreach (var wall in activeWalls)
                {
                    if (wall.CollidesWith(player.hitbox))
                    {
                        state = GameState.Loss;
                        activeWalls.Clear();
                        break;
                    }
                }
            
            }
            else if (state == GameState.Menu)
            {
                display.DrawMenu();
                if (InputManager.IsKeyPressed(ConsoleKey.Enter))
                {
                    display = new Display(Console.WindowWidth - 10, Console.WindowHeight - 7);
                    state = GameState.inGame;
                }
            }
            else if (state == GameState.Loss)
            {
                display.DrawLossMenu(score);
                if (InputManager.IsKeyPressed(ConsoleKey.Enter))
                {
                    state = GameState.Menu;
                    score = 0;
                }
            }
            else if (state == GameState.Victory)
            {
                display.DrawWinMenu();
                if (InputManager.IsKeyPressed(ConsoleKey.Enter))
                {
                    state = GameState.Menu;
                }
            }
        }


        
        Display display;
        public void DrawGame()
        {

            if (state == GameState.inGame)
            {
                for (int i = activeWalls.Count - 1; i >= 0; i--)
                {
                    var wall = activeWalls[i];
                    wall.Draw(display);
                }


                display.DrawGameToConsole(score);
                display.Clear();
            }
        }


        enum GameState { Menu, inGame, Loss, Victory }
    }
}
