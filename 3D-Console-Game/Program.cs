using System.Text;
using System.Xml.Linq;

namespace _3D_Console_Game
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double prevTime = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000;

            Game game = new Game();

            Console.CursorVisible = false;
            //Console.ForegroundColor = ConsoleColor.Green;

            while (true)
            {
                double deltaTime = ((double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000) - prevTime;

                if (deltaTime >= 0.09)
                {
                    prevTime += deltaTime;

                    InputManager.UpdateKey();
                    game.UpdateGame(deltaTime);

                    game.DrawGame();


                    

                }
            }
        }
    }
}
