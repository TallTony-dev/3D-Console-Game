using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace _3D_Console_Game.Engine
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

        public int fps;

        public ConsoleColor[] palette = { ConsoleColor.Black, ConsoleColor.DarkMagenta, ConsoleColor.DarkBlue };


        (float strength, ConsoleColor color, float distFromCam)[,] values;

        Matrix4x4 viewMatrix = Matrix4x4.Identity;
        Matrix4x4 projectionMatrix;

        private static readonly StreamWriter _writer = new StreamWriter(
            Console.OpenStandardOutput(), new UTF8Encoding(false), 1 << 16)
        { AutoFlush = false };

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
        private void RasterizeClipSpaceTriangle(Vector4 a, Vector4 b, Vector4 c, ConsoleColor color)
        {
            RasterizeClipSpaceTriangle(new (Vector4, ConsoleColor)[] { (a, color), (b, color), (c, color) });
        }
        private void RasterizeClipSpaceTriangle((Vector4 vec, ConsoleColor col)[] vertices)
        {
            int maxXLen = values.GetLength(0);
            int maxYLen = values.GetLength(1);

            for (int i = 0; i < vertices.Length; i++)
            {

                // LLM:Perspective divide: convert from clip space to NDC
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

            float triArea = EdgeFunction(a, b, c);
            if (triArea == 0) return;

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    //go to all the values in between and set stuff

                    Vector4 p = new(x, y, 0, 0);
                    float ABP = EdgeFunction(a, b, p);
                    float BCP = EdgeFunction(b, c, p);
                    float CAP = EdgeFunction(c, a, p);

                    if (ABP <= 0 && BCP <= 0 && CAP <= 0 || ABP >= 0 && BCP >= 0 && CAP >= 0)
                    {
                        // LLM: Barycentric weights for perspective-correct interpolation
                        float w0 = BCP / triArea;
                        float w1 = CAP / triArea;
                        float w2 = ABP / triArea;

                        float dist = w0 * a.Z + w1 * b.Z + w2 * c.Z;
                        if (dist < values[x, y].distFromCam)
                        {
                            values[x, y].distFromCam = dist;
                            float absABP = MathF.Abs(ABP);
                            float absBCP = MathF.Abs(BCP);
                            float absCAP = MathF.Abs(CAP);
                            float nearest = MathF.Min(MathF.Min(absABP, absBCP), absCAP);
                            float distOfNearest = float.NaN;
                            if (nearest == absABP) { distOfNearest = Vector4.Distance(a, b); }
                            else if (nearest == absBCP) { distOfNearest = Vector4.Distance(b, c); }
                            else if (nearest == absCAP) { distOfNearest = Vector4.Distance(c, a); }

                            values[x, y].strength = MathF.Pow(nearest * 2 / distOfNearest, 8) * 8;
                            values[x, y].color = vertices[0].col;
                        }
                    }
                }
            }
        }


        private static Vector4 ClipEdge(Vector4 front, Vector4 back, float near = 0.001f)
        {
            float t = (near - front.W) / (back.W - front.W);
            return Vector4.Lerp(front, back, t);
        }

        public void DrawTriangle((Vector4 vec, ConsoleColor col)[] vertices)
        {

            //check for culling and stuff
            bool[] behindOnes = new bool[vertices.Length];
            int behindCount = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].vec = Vector4.Transform(vertices[i].vec, viewMatrix);
                vertices[i].vec = Vector4.Transform(vertices[i].vec, projectionMatrix);
                if (vertices[i].vec.W < 0.001f)
                {
                    behindCount++;
                    behindOnes[i] = true;
                } 
            }
            if (behindCount == 0) RasterizeClipSpaceTriangle(vertices);
            else if (behindCount == 3) return;
            else if (behindCount == 1)
            {
                Vector4 newA;
                Vector4 newB;
                if (behindOnes[0])
                {
                    newA = ClipEdge(vertices[1].vec, vertices[0].vec);
                    newB = ClipEdge(vertices[2].vec, vertices[0].vec);
                    RasterizeClipSpaceTriangle(vertices[1].vec, vertices[2].vec, newA, vertices[1].col);
                    RasterizeClipSpaceTriangle(vertices[2].vec, newB, newA, vertices[1].col);
                }
                else if (behindOnes[1])
                {
                    newA = ClipEdge(vertices[0].vec, vertices[1].vec);
                    newB = ClipEdge(vertices[2].vec, vertices[1].vec);
                    RasterizeClipSpaceTriangle(vertices[0].vec, vertices[2].vec, newA, vertices[1].col);
                    RasterizeClipSpaceTriangle(vertices[2].vec, newB, newA, vertices[1].col);
                }
                else if (behindOnes[2])
                {
                    newA = ClipEdge(vertices[0].vec, vertices[2].vec);
                    newB = ClipEdge(vertices[1].vec, vertices[2].vec);
                    RasterizeClipSpaceTriangle(vertices[0].vec, vertices[1].vec, newA, vertices[1].col);
                    RasterizeClipSpaceTriangle(vertices[1].vec, newB, newA, vertices[1].col);
                }
                
            }
            else if (behindCount == 2)
            {
                Vector4 newA;
                Vector4 newB;
                int b0 = -1;
                int b1 = -1;
                int a = Array.FindIndex(behindOnes, x => x == false);
                if (behindOnes[0])
                {
                    b0 = 0;
                }
                if (behindOnes[1])
                {
                    if (b0 == -1)
                    {
                        b0 = 1;
                    }
                    else
                    {
                        b1 = 1;
                    }
                }
                if (behindOnes[2])
                {
                    b1 = 2;
                }

                newA = ClipEdge(vertices[a].vec, vertices[b0].vec);
                newB = ClipEdge(vertices[a].vec, vertices[b1].vec);
                RasterizeClipSpaceTriangle(vertices[a].vec, newA, newB, vertices[1].col);
            }
        }

        public void Clear()
        {
            values = new (float strength, ConsoleColor color, float distFromCam)[values.GetLength(0), values.GetLength(1)];

            //if (elapsedTime -  lastPaletteSwapTime > 5)
            //{
            //    lastPaletteSwapTime = elapsedTime;
            //    Random rand = new Random();

            //    palette[0] = (ConsoleColor)rand.Next(0, 15);
            //    palette[1] = (ConsoleColor)rand.Next(0, 15);
            //    while (palette[0] == ConsoleColor.Blue || palette[0] == ConsoleColor.Red || palette[0] == ConsoleColor.DarkBlue || palette[0] == ConsoleColor.DarkRed || palette[0] == ConsoleColor.DarkCyan || palette[0] == ConsoleColor.Cyan)
            //    {
            //        palette[0] = (ConsoleColor)rand.Next(0, 15);
            //    }
            //    while (palette[1] == ConsoleColor.Blue || palette[1] == ConsoleColor.Red || palette[1] == ConsoleColor.DarkBlue || palette[1] == ConsoleColor.DarkRed || palette[1] == ConsoleColor.DarkCyan || palette[1] == ConsoleColor.Cyan)
            //    {
            //        palette[1] = (ConsoleColor)rand.Next(0, 15);
            //    }
            //}


            //for (int x = 0; x < values.GetLength(0); x++)
            //{
            //    for (int y = 0; y < values.GetLength(1); y++)
            //    {
            //        int ux = x + (int)(MathF.Sin(elapsedTime) * 10);
            //        int uy = y + (int)(elapsedTime * 4 + 5 * MathF.Sin((float)ux / 6f + elapsedTime * 0.5f));

            //        bool cond = (uy % (5 + (int)(3 * MathF.Sin(elapsedTime * 0.6f + 2)))) == 0 && MathF.Sin(x * 0.2f + y * 0.3f + 40 * MathF.Sin(0.3f * elapsedTime + 1)) > MathF.Sin(elapsedTime * 0.72f);
            //        values[x, y].color = cond ? palette[0] : palette[1];
            //    }
            //}

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

            Hudstuff.HUD.DrawHUD(display);

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
                    if (display[x, y] == default)
                    {
                        (float strength, ConsoleColor color, float distFromCam) value = values[x, y];
                        if (maxDist == 0 || value.distFromCam == float.MaxValue)
                            display[x, y].c = MapToChar(0);
                        else
                            display[x, y].c = MapToChar((value.distFromCam / maxDist + value.strength) / 2);
                        display[x, y].col = value.color;
                    }
                }
            }

            //LLM: optimize calls by reducing escape sequence count
            ConsoleColor prevFg = (ConsoleColor)(-1);
            bool prevHasBg = false;

            for (int y = 0; y < display.GetLength(1); y++)
            {
                for (int x = 0; x < display.GetLength(0); x++)
                {
                    bool needsBg = display[x, y].c < 176;
                    ConsoleColor fg = display[x, y].col;

                    if (needsBg != prevHasBg || fg != prevFg)
                    {
                        buf.Append(RESET);
                        if (needsBg)
                            buf.Append($"{ESC}[47{SUFFIX}");
                        buf.Append(GetAnsiForegroundColor(fg));
                        prevFg = fg;
                        prevHasBg = needsBg;
                    }

                    buf.Append(display[x, y].c);
                }
                buf.Append(RESET);
                prevFg = (ConsoleColor)(-1);
                prevHasBg = false;
                buf.AppendLine();
            }
            buf.Append(fps.ToString());

            Console.SetCursorPosition(0, 0);
            _writer.Write(buf);
            _writer.Flush();
        }

        
    }
}
