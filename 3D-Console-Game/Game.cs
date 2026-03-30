using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using _3D_Console_Game.Entities;

namespace _3D_Console_Game
{
    internal class Game
    {
        public Game() { display = new Display(50, 50); }

        public Player player = new Player(100);

        public static List<object> activeObjects = new List<object>();
        
        int score = 0;

        GameState state = GameState.Menu;


        public void InitializeGame()
        {
            activeObjects.Add(player);
            activeObjects.Add(new Box(new Vector3(3, 0, 0), 3, 1, 4, ConsoleColor.Magenta));
            activeObjects.Add(new Box(new Vector3(-3, 0, 0), 3, 1, 10, ConsoleColor.Magenta, -0.385398f));
            for (int i = 0; i < 15; i++)
                activeObjects.Add(new PhysicsBox(new Vector3(i * 0.6f, i * 0.6f, -2), 0.5f, 0.5f, 0.5f, ConsoleColor.Blue, 0f) { isPickable = true });
            activeObjects.Add(new Box(new Vector3(-1000, 0f, -1000), 2000, 0, 2000, ConsoleColor.White));
            activeObjects.Add(new Slime(new Vector3(1, 4f, 0)));
            activeObjects.Add(new Gunner(new Vector3(2, 3, 0)));
            Hudstuff.HUD.InitializeHud();
        }


        public void UpdateGame(double deltaTime)
        {
            InputManager.UpdateKey();
            display.fps = (int)(1 / deltaTime);
            if (state == GameState.inGame)
            {
                InputManager.UpdateMousePos();
                display.Update(deltaTime, player);
                ParticleManager.UpdateParticles(deltaTime);

                for (int i = 0; i < activeObjects.Count; i++)
                {
                    object obj = activeObjects[i];
                    if (obj is IUpdatable updatable)
                    {
                        updatable.Update(deltaTime);
                        if (obj is Entity e)
                        {
                            if (e.isDead) activeObjects.Remove(e);
                        }
                    }
                }

                if (InputManager.IsKeyPressed(ConsoleKey.C))
                {
                    Random rand = new Random();
                    activeObjects.Add(new PhysicsBox(new Vector3(-3 + rand.NextSingle() * 2, 5, 3), 0.5f, 0.5f, 0.5f, ConsoleColor.Yellow, 0f) { isPickable = true });
                }
                if (InputManager.IsKeyPressed(ConsoleKey.F))
                {
                    activeObjects.Add(new Slime(new Vector3(1, 4f, 0)));
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
                foreach (object obj in activeObjects)
                {
                    if (obj is IDrawable draw)
                    {
                        draw.Draw(display);
                    }
                }
                ParticleManager.DrawParticles(display);

                display.DrawGameToConsole(score);
                display.Clear();
            }
        }


        enum GameState { Menu, inGame, Loss, Victory }
    }
}
