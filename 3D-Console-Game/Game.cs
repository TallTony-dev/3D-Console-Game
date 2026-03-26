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

        public static List<IDrawable> activeDrawables = new List<IDrawable>();
        
        int score = 0;

        GameState state = GameState.Menu;


        public void InitializeGame()
        {
            activeDrawables.Add(new Box(new Vector3(3, 0, 0), 3, 1, 4, ConsoleColor.Magenta));
            activeDrawables.Add(new Box(new Vector3(-3, 0, 0), 3, 1, 10, ConsoleColor.Magenta, -0.385398f));
            for (int i = 0; i < 15; i++)
                activeDrawables.Add(new PhysicsBox(new Vector3(i * 0.6f, i * 0.6f, -2), 0.5f, 0.5f, 0.5f, ConsoleColor.Blue, 0f) { isPickable = true });
            activeDrawables.Add(new Box(new Vector3(-1000, 0f, -1000), 2000, 0, 2000, ConsoleColor.White));
            //activeDrawables.Add(new Barrier(new Vector3(-1000,0,-1000), new Vector3(1000, 0, -1000), new Vector3(-1000, 0, 1000), new Vector3(1000, 0, 1000), ConsoleColor.White));
            activeDrawables.Add(new Slime(new Vector3(1, 4f, 0)));
        }


        public void UpdateGame(double deltaTime)
        {
            InputManager.UpdateKey();
            display.fps = (int)(1 / deltaTime);
            if (state == GameState.inGame)
            {
                InputManager.UpdateMousePos();
                display.Update(deltaTime, player);
                player.Update(deltaTime);
                ParticleManager.UpdateParticles(deltaTime);

                foreach (IDrawable obj in activeDrawables.ToList())
                {
                    if (obj is IUpdatable updatable)
                    {
                        updatable.Update(deltaTime);
                    }
                }

                if (InputManager.IsKeyPressed(ConsoleKey.C))
                {
                    Random rand = new Random();
                    activeDrawables.Add(new PhysicsBox(new Vector3(-3 + rand.NextSingle() * 2, 5, 3), 0.5f, 0.5f, 0.5f, ConsoleColor.Yellow, 0f) { isPickable = true });
                }
                if (InputManager.IsKeyPressed(ConsoleKey.F))
                {
                    activeDrawables.Add(new Slime(new Vector3(1, 4f, 0)));
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
                foreach (IDrawable wall in activeDrawables)
                {
                    wall.Draw(display);
                }
                ParticleManager.DrawParticles(display);

                display.DrawGameToConsole(score);
                display.Clear();
            }
        }


        enum GameState { Menu, inGame, Loss, Victory }
    }
}
