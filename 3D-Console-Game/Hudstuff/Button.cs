using _3D_Console_Game.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Hudstuff
{
    internal class Button : TextBoxElement
    {
        Action onClick;

        public Button(int x, int y, Origin origin, string text, Action onClick) : base(x, y, origin, text)
        {
            this.onClick = onClick;
        }

        static bool Intersects(Point p, int x, int y, int width, int height)
        {
            return (p.X >= x && p.X <= x + width && p.Y >= y && p.Y <= y + height);
        }

        public override void Update(float dt)
        {
            Point mousePos = InputManager.relativeMousePos;

            if (Intersects(mousePos, x, y, text.Length + 2, 3) && InputManager.IsLeftMousePressed())
            {
                onClick.Invoke();
            }
            base.Update(dt);
        }

    }
}
