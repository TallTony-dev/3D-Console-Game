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

        public static List<IDrawable> activeWalls = new List<IDrawable>();
        
        double timeCount = 30;
        int score = 0;

        GameState state = GameState.Menu;
        public void UpdateGame(double deltaTime)
        {
            InputManager.UpdateKey();
            if (state == GameState.inGame)
            {
                InputManager.UpdateMousePos();
                display.Update(deltaTime, player);
                player.Update(deltaTime);

                foreach (IDrawable wall in activeWalls)
                {
                    if (wall is PhysicsBox phys)
                    {
                        phys.Update(deltaTime);
                    }
                }

                //timeCount += deltaTime;

                if (timeCount > 3)
                {
                    activeWalls.Add(new Barrier(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0,1,0), new Vector3(1,1,0), ConsoleColor.Red));
                    activeWalls.Add(new Barrier(new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 0), new Vector3(1, 1, 1), ConsoleColor.Green));
                    activeWalls.Add(new Barrier(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), new Vector3(0, 1, 1), ConsoleColor.Blue));
                    activeWalls.Add(new Barrier(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 0, 1), ConsoleColor.Black));
                    activeWalls.Add(new Barrier(new Vector3(0, 1, 0), new Vector3(0, 1, 1), new Vector3(1, 1, 0), new Vector3(1, 1, 1), ConsoleColor.DarkGray));
                    activeWalls.Add(new Barrier(new Vector3(2, 0, 1), new Vector3(2, 0, 0), new Vector3(2, 1, 1), new Vector3(2, 1, 0), ConsoleColor.DarkGray));
                    activeWalls.Add(new Box(new Vector3(3, 0, 0), 3, 1, 4, ConsoleColor.Magenta));
                    activeWalls.Add(new Box(new Vector3(-3, 0, 0), 3, 1, 4, ConsoleColor.Magenta, 0.785398f));
                }

            }
            else if (state == GameState.Menu)
            {
                display.DrawMenu();
                if (InputManager.IsKeyPressed(ConsoleKey.Enter))
                {
                    display = new Display(Console.WindowWidth - 1, Console.WindowHeight - 2);
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
                foreach (IDrawable wall in activeWalls)
                {
                    Task.Run(() => wall.Draw(display));
                }


                display.DrawGameToConsole(score);
                display.Clear();
            }
        }


        enum GameState { Menu, inGame, Loss, Victory }
    }
}
