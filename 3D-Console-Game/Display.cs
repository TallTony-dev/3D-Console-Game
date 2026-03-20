using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace _3D_Console_Game
{
    internal class Display
    {
        const string ESC = "\e";
        const string SUFFIX = "m";
        const string SEPARATOR = ";";
        const string RESET = ESC + "[0" + SUFFIX;
        private static readonly Dictionary<ConsoleColor, string> ForegroundMap = new Dictionary<ConsoleColor, string>
        {
            { ConsoleColor.Black, "30" },
            { ConsoleColor.DarkRed, "31" },
            { ConsoleColor.DarkGreen, "32" },
            { ConsoleColor.DarkYellow, "33" },
            { ConsoleColor.DarkBlue, "34" },
            { ConsoleColor.DarkMagenta, "35" },
            { ConsoleColor.DarkCyan, "36" },
            { ConsoleColor.Gray, "37" },
            { ConsoleColor.DarkGray, "90" },
            { ConsoleColor.Red, "91" },
            { ConsoleColor.Green, "92" },
            { ConsoleColor.Yellow, "93" },
            { ConsoleColor.Blue, "94" },
            { ConsoleColor.Magenta, "95" },
            { ConsoleColor.Cyan, "96" },
            { ConsoleColor.White, "97" }
        };
        public static string GetAnsiForegroundColor(ConsoleColor color)
        {
            if (ForegroundMap.ContainsKey(color))
            {
                return $"{ESC}[{ForegroundMap[color]}{SUFFIX}";
            }
            return "";
        }


        public ConsoleColor[] palette = { ConsoleColor.Black, ConsoleColor.DarkMagenta, ConsoleColor.DarkBlue };


        (float strength, ConsoleColor color, float distFromCam)[,] values;

        Matrix4x4 viewMatrix = Matrix4x4.Identity;
        Matrix4x4 projectionMatrix;

        public Display(int width, int height)
        {
            values = new (float strength, ConsoleColor color, float distFromCam)[width, height];
            projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(1f, width / height, 0.1f, 100f);
            InitializeDepthBuffer();
        }

        float elapsedTime = 0f;

        public void Update(double deltaTime, Player player)
        {
            elapsedTime += (float)deltaTime;
            viewMatrix = player.ViewMatrix;
        }


        //returns double signed area of triangle
        private float EdgeFunction(Vector4 a, Vector4 b, Vector4 c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }


        public void DrawSquare(Vector4 topLeft, Vector4 topRight, Vector4 bottomLeft, Vector4 bottomRight, ConsoleColor color)
        {
            (Vector4, ConsoleColor)[] vertices = new (Vector4, ConsoleColor)[36];
            vertices[0] = (bottomLeft, color);
            vertices[1] = (topLeft, color);
            vertices[2] = (topRight, color);
            vertices[3] = (bottomLeft, color);
            vertices[4] = (topRight, color);
            vertices[5] = (bottomRight, color);  
            DrawTriangle(vertices[0..3]);
            DrawTriangle(vertices[3..6]);
        }


        public void DrawTriangle((Vector4 vec, ConsoleColor col)[] vertices)
        {
            int maxXLen = values.GetLength(0);
            int maxYLen = values.GetLength(1);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].vec = Vector4.Transform(vertices[i].vec, viewMatrix);
                vertices[i].vec = Vector4.Transform(vertices[i].vec, projectionMatrix);

                // LLM:Perspective divide: convert from clip space to NDC
                if (vertices[i].vec.W <= 0) return; // behind camera, skip triangle
                float w = vertices[i].vec.W;
                vertices[i].vec = new Vector4(
                    vertices[i].vec.X / w,
                    vertices[i].vec.Y / w,
                    vertices[i].vec.Z / w,
                    w);

                // LLM: Convert from NDC [-1,1] to pixel space
                vertices[i].vec = new Vector4(
                    (vertices[i].vec.X + 1f) * 0.5f * maxXLen,
                    (vertices[i].vec.Y + 1f) * 0.5f * maxYLen,
                    vertices[i].vec.Z,
                    vertices[i].vec.W);
            }
            Vector4 a = vertices[0].vec;
            Vector4 b = vertices[1].vec;
            Vector4 c = vertices[2].vec;

            //LLM: Clamp to be the display bounds instead of my method which kinda sucked a lil
            int minX = Math.Max(0, (int)vertices.Min(v => v.vec.X));
            int maxX = Math.Min(maxXLen, (int)MathF.Ceiling(vertices.Max(v => v.vec.X)));
            int minY = Math.Max(0, (int)vertices.Min(v => v.vec.Y));
            int maxY = Math.Min(maxYLen, (int)MathF.Ceiling(vertices.Max(v => v.vec.Y)));

            for (int x = minX; x < maxX; x++)
            {
                    for (int y = minY; y < maxY; y++)
                    {
                            //go to all the values in between and set stuff

                            Vector4 p = new(x, y, 0, 0);
                            float ABP = EdgeFunction(a, b, p);
                            float BCP = EdgeFunction(b, c, p);
                            float CAP = EdgeFunction(c, a, p);

                            if (ABP <= 1f && BCP <= 1f && CAP <= 1f)
                            {
                                float dist = a.Z * a.W + b.Z * b.W + c.Z * c.W;
                                if (dist < values[x, y].distFromCam)
                                {
                                    values[x, y].distFromCam = dist;
                                    float nearest = Math.Max(Math.Max(ABP, BCP), CAP);
                                    float distOfNearest = float.NaN;
                                    if (nearest == ABP) { distOfNearest = Vector4.Distance(a, b); }
                                    else if (nearest == BCP) { distOfNearest = Vector4.Distance(b, c); }
                                    else if (nearest == CAP) { distOfNearest = Vector4.Distance(c, a); }

                                    values[x, y].strength = MathF.Pow((nearest * 2) / distOfNearest, 8) * 8;
                                    values[x, y].color = vertices[0].col;
                                }
                            }
                    }
            }
        }

        float lastPaletteSwapTime = 0;
        public void Clear()
        {
            values = new (float strength, ConsoleColor color, float distFromCam)[values.GetLength(0), values.GetLength(1)];

            if (elapsedTime -  lastPaletteSwapTime > 5)
            {
                lastPaletteSwapTime = elapsedTime;
                Random rand = new Random();

                palette[0] = (ConsoleColor)rand.Next(0, 15);
                palette[1] = (ConsoleColor)rand.Next(0, 15);
                while (palette[0] == ConsoleColor.Blue || palette[0] == ConsoleColor.Red || palette[0] == ConsoleColor.DarkBlue || palette[0] == ConsoleColor.DarkRed || palette[0] == ConsoleColor.DarkCyan || palette[0] == ConsoleColor.Cyan)
                {
                    palette[0] = (ConsoleColor)rand.Next(0, 15);
                }
                while (palette[1] == ConsoleColor.Blue || palette[1] == ConsoleColor.Red || palette[1] == ConsoleColor.DarkBlue || palette[1] == ConsoleColor.DarkRed || palette[1] == ConsoleColor.DarkCyan || palette[1] == ConsoleColor.Cyan)
                {
                    palette[1] = (ConsoleColor)rand.Next(0, 15);
                }
            }


            for (int x = 0; x < values.GetLength(0); x++)
            {
                for (int y = 0; y < values.GetLength(1); y++)
                {
                    int ux = x + (int)(MathF.Sin(elapsedTime) * 10);
                    int uy = y + (int)(elapsedTime * 4 + 5 * MathF.Sin((float)ux / 6f + elapsedTime * 0.5f));

                    bool cond = (uy % (5 + (int)(3 * MathF.Sin(elapsedTime * 0.6f + 2)))) == 0 && MathF.Sin(x * 0.2f + y * 0.3f + 40 * MathF.Sin(0.3f * elapsedTime + 1)) > MathF.Sin(elapsedTime * 0.72f);
                    values[x,y].color = cond ? palette[0] : palette[1];
                }
            }

            InitializeDepthBuffer();
        }

        private void InitializeDepthBuffer()
        {
            for (int x = 0; x < values.GetLength(0); x++)
                for (int y = 0; y < values.GetLength(1); y++)
                    values[x, y].distFromCam = float.MaxValue;
        }

        private char MapToChar(float fullness)
        {
            //takes in normalized 0-1 and maps to a char
            if (fullness == 0) return '█';
            if (fullness < 0.2) return '░';
            if (fullness < 0.4) return '▒';
            if (fullness < 0.7) return '▓';
            return '█';
        }

        public void DrawGameToConsole(int score)
        {
            StringBuilder buf = new StringBuilder();
            (char c, ConsoleColor col)[,] display = new (char, ConsoleColor)[values.GetLength(0), values.GetLength(1)];

            float maxDist = 0;
            for (int x = 0; x < values.GetLength(0); x++)
            {
                for (int y = 0; y < values.GetLength(1); y++)
                {
                    if (values[x, y].distFromCam != float.MaxValue && values[x, y].distFromCam > maxDist)
                        maxDist = values[x, y].distFromCam;
                }
            }

            //convert values to chars here
            for (int x = 0; x < values.GetLength(0); x++)
            {
                for (int y = 0; y < values.GetLength(1); y++)
                {
                    (float strength, ConsoleColor color, float distFromCam) value = values[x, y];
                    if (maxDist == 0 || value.distFromCam == float.MaxValue)
                        display[x, y].c = MapToChar(0);
                    else
                        display[x, y].c = MapToChar(((value.distFromCam / maxDist) + value.strength) / 2);
                    display[x, y].col = value.color;

                }
            }

            for (int y = 0; y < display.GetLength(1); y++)
            {
                for (int x = 0; x < display.GetLength(0); x++)
                {

                    buf.Append(GetAnsiForegroundColor(display[x, y].col));
                    buf.Append(display[x, y].c);
                    buf.Append(RESET);
                }
                buf.AppendLine();
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(buf.ToString());
        }

        public void DrawMenu()
        {
            StringBuilder buf = new StringBuilder();
            int width = values.GetLength(0);
            int height = values.GetLength(1);
            buf.Append(GetAnsiForegroundColor(ConsoleColor.Cyan));
            buf.Append("Welcome to super hexagon, press enter to start");
            buf.Append(RESET);

            Console.Clear();
            Console.Write(buf.ToString());
        }

        public void DrawWinMenu()
        {
            StringBuilder buf = new StringBuilder();
            int width = values.GetLength(0);
            int height = values.GetLength(1);
            buf.Append(GetAnsiForegroundColor(ConsoleColor.Green));
            buf.Append("YAY YOU WON WOOHOO");
            buf.Append(RESET);

            Console.Clear();
            Console.Write(buf.ToString());
        }

        public void DrawLossMenu(int score)
        {
            StringBuilder buf = new StringBuilder();
            int width = values.GetLength(0);
            int height = values.GetLength(1);
            buf.Append(GetAnsiForegroundColor(ConsoleColor.Red));
            buf.AppendLine("DAMNNNNNN U LOST TWIN");
            buf.Append(RESET);
            buf.Append(GetAnsiForegroundColor(ConsoleColor.Green));
            buf.AppendLine($"Its ok twin atleast u got {score} points :3");
            buf.Append(RESET);

            Console.Clear();
            Console.Write(buf.ToString());
        }
    }
}
