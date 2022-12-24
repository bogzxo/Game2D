using Game2D.Rendering;
using Game2D.Rendering.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.Data.Inventory
{
    public enum ItemType
    {
        None,
        Cum
    }
    public struct Item
    {
        public ItemType Type { get; set; }

        public Item(ItemType type)
        {
            this.Type = type;
        }
    }
    public class InventoryRenderer
    {
        private UISprite backgroundSprite;

        public Inventory Inventory { get; private set; }


        public InventoryRenderer(ref Inventory inventory)
        {
            Inventory = inventory;

            backgroundSprite = new UISprite(GameManager.Instance.AssetManager.GetShader("basic_shader"), new Texture(new System.Drawing.Bitmap("Assets/UI/inventory_background.png"), false, false))
            {
                Scale = new OpenTK.Mathematics.Vector2(0.75f),
                Position = new OpenTK.Mathematics.Vector2(0, 0.25f)
            };

            backgroundSprite.Update(0);
        }

        public void Draw(float dt)
        {
            if (!Inventory.IsVisible) return;

            backgroundSprite.Draw(dt);
        }
    }
    public class Inventory
    {
        public Item[] Items { get; set; }

        public static readonly int Width = 5;
        public static readonly int Height = 5;
        public static readonly int Length = Width * Height;

        public bool IsVisible { get; private set; } = false;

        public Inventory()
        {
            Items = new Item[Length];
            for (int i = 0; i < Length; i++)
                Items[i] = new Item(ItemType.None);
        }

        public void Update(float dt)
        {
            if (GameManager.Instance.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Tab))
                IsVisible = !IsVisible;
        }
    }
}
