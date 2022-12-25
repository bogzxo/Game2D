using Game2D.Rendering;
using Game2D.Rendering.UI;
using OpenTK.Mathematics;
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
        Sword
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
        private UISprite hotbarSprite, selectedHotbarItemSprite, itemSprite;
        private Texture backgroundTexture, selectedRectangle;

        public Dictionary<ItemType, Texture> ItemTextures { get; private set; }

        public int SelectedIndex { get; private set; } = 0;
        public Inventory Inventory { get; private set; }


        public InventoryRenderer(ref Inventory inventory)
        {
            Inventory = inventory;
            backgroundTexture = new Texture(new System.Drawing.Bitmap("Assets/UI/hotbar_background.png"), false, false);
            selectedRectangle = new Texture(new System.Drawing.Bitmap("Assets/UI/selected_outline.png"), false, false);


            hotbarSprite = new UISprite(GameManager.Instance.AssetManager.GetShader("basic_shader"), backgroundTexture)
            {
                Position = new Vector2(0, -0.8f),
                Scale = new Vector2(0.1f)
            };
            selectedHotbarItemSprite = new UISprite(GameManager.Instance.AssetManager.GetShader("basic_shader"), selectedRectangle)
            {
                Position = new Vector2(-0.5f, -0.8f),
                Scale = new Vector2(0.1f)
            };
            itemSprite = new UISprite(GameManager.Instance.AssetManager.GetShader("basic_shader"), selectedRectangle)
            {
                Position = new Vector2(-0.5f, -0.79f),
                Scale = new Vector2(0.05f)
            };
            hotbarSprite.Update(0.0f);
            selectedHotbarItemSprite.Update(0.0f);
            itemSprite.Update(0.0f);

            ItemTextures = new Dictionary<ItemType, Texture>()
            {
                { ItemType.Sword, new Texture(new System.Drawing.Bitmap("Assets/Items/sword.png"), false, false) }
            };
        }

        public void Draw(float dt)
        {
            hotbarSprite.Draw(dt);
            selectedHotbarItemSprite.Draw(dt);

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i].Type != ItemType.None)
                {
                    itemSprite.Texture = ItemTextures[Inventory[i].Type];
                    itemSprite.Position.X = (i - 2.5f) * 0.1f;
                    itemSprite.Update(0.0f);
                    itemSprite.Draw(0.0f);
                }
            }
        }

        public void Update(float dt)
        {
            SelectedIndex += (int)(-GameManager.Instance.MouseState.ScrollDelta.Y);
            SelectedIndex = Math.Clamp(SelectedIndex, 0, 5);
            selectedHotbarItemSprite.Position.X = (SelectedIndex - 2.5f) * 0.1f;


            hotbarSprite.Update(dt);
            selectedHotbarItemSprite.Update(dt);
        }
    }
    public class Inventory
    {
        public Item[] Items { get; set; }

        public static readonly int Length = 5;

        public bool IsVisible { get; private set; } = false;

        public Inventory()
        {
            Items = new Item[Length];
            for (int i = 0; i < Length; i++)
                Items[i] = new Item(ItemType.None);

            Items[0] = new Item(ItemType.Sword);
            Items[2] = new Item(ItemType.Sword);
        }

        public ref Item this[int index]

        {
            get => ref Items[index];
        }

        public void Update(float dt)
        {

        }
    }
}
