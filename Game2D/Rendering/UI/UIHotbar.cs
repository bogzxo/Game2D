using Game2D.Data;
using Game2D.Data.Inventory;
using ImGuiNET;
using Newtonsoft.Json.Bson;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.Rendering.UI
{
    public class UIHotbar
    {
        private UISprite hotbarSprite, selectedHotbarItemSprite;
        private Texture backgroundTexture, selectedRectangle;

        public int SelectedIndex { get; private set; } = 0;
        public Inventory InventoryInventory { get; private set; }


        public UIHotbar(ref Inventory inventory)
        {
            InventoryInventory = inventory;
            backgroundTexture = new Texture(new System.Drawing.Bitmap("Assets/UI/hotbar_background.png"), false, false);
            selectedRectangle = new Texture(new System.Drawing.Bitmap("Assets/UI/selected_outline.png"), false, false);

            hotbarSprite = new UISprite(GameManager.Instance.AssetManager.GetShader("basic_shader"), backgroundTexture)
            {
                Position = new Vector2(0, -8),
                Scale = new Vector2(0.1f)
            };
            selectedHotbarItemSprite = new UISprite(GameManager.Instance.AssetManager.GetShader("basic_shader"), selectedRectangle)
            {
                Position = new Vector2(-5, -8),
                Scale = new Vector2(0.1f)
            };

            hotbarSprite.Update(0.0f);
            selectedHotbarItemSprite.Update(0.0f);
        }

        public void Draw(float dt)
        {
            hotbarSprite.Draw(dt);
            selectedHotbarItemSprite.Draw(dt);
        }

        public void Update(float dt)
        {
            SelectedIndex += (int)(-GameManager.Instance.MouseState.ScrollDelta.Y);
            SelectedIndex = Math.Clamp(SelectedIndex, 0, 5);
            selectedHotbarItemSprite.Position.X = (SelectedIndex * 2.0f - 5);


            hotbarSprite.Update(dt);
            selectedHotbarItemSprite.Update(dt);
        }
    }
}
