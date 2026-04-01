using _3D_Console_Game.Engine;
using _3D_Console_Game.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Console_Game.Hudstuff
{
    internal class Inventory : HudElement
    {
        public List<InventoryItem> items = new();
        public int selectedItem = 0;
        private int prevSelectedItem = 0;

        public Inventory() : base((Console.WindowWidth) / 2 - 40, 0, Origin.BottomLeft, 80, 4)
        {
            items.Add(new BoxItem(ConsoleColor.Black));
            items.Add(new BoxItem(ConsoleColor.Blue));
            items.Add(new BoxItem(ConsoleColor.DarkGreen));
            items.Add(new WeaponItem(ConsoleColor.DarkMagenta));
        }

        public override void Update(float dt)
        {
            if (InputManager.IsKeyPressed(ConsoleKey.LeftArrow))
            {
                selectedItem = selectedItem > 0 ? --selectedItem : items.Count() - 1;
            }
            else if (InputManager.IsKeyPressed(ConsoleKey.RightArrow))
            {
                selectedItem = items.Count - 1 > selectedItem ? ++selectedItem : 0;
            }

            if (selectedItem != prevSelectedItem)
            {
                items[prevSelectedItem].Deselect();
                prevSelectedItem = selectedItem;
                items[selectedItem].Select();
            }

            foreach (InventoryItem item in items)
            {
                item.Update(dt);
            }
        }

        public void Draw3DItems(Display display)
        {
            foreach (InventoryItem item in items)
            {
                item.Draw(display);
            }
        }

        public override void Draw((char c, ConsoleColor col)[,] display)
        {
            ClearArea(display);
            DrawOutline(display, ConsoleColor.DarkGreen);

            int height = display.GetLength(1);
            for (int x = 0; x < items.Count; x++)
            {
                int actualX = x * 8 + this.x + 5;
                int actualY = height - y - this.height;
                items[x].DrawItem(actualX, actualY, display);
                if (x == selectedItem)
                {
                    DrawBox(display, actualX - 1, actualY - 1, 5, 4, ConsoleColor.Green);
                }
            }
        }
    }
}
