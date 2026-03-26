using System.Text;
using System.Xml.Linq;

namespace _3D_Console_Game
{
    internal class Program
    {
        public static Game game = new Game();
        static void Main(string[] args)
        {
            double prevTime = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000;

            Console.CursorVisible = false;
            game.InitializeGame();

            while (true)
            {
                double deltaTime = ((double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000) - prevTime;

                if (deltaTime >= 0.01)
                {
                    prevTime += deltaTime;

                    game.UpdateGame(deltaTime);

                    game.DrawGame();
                    

                    

                }
            }
        }
    }
}
