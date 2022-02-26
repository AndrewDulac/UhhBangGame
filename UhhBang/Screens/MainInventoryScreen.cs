using UhhBang.StateManagement;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UhhBang.Screens
{
    // The pause menu comes up over the top of the game,
    // giving the player options to resume or quit.
    public class MainInventoryScreen : InventoryScreen
    {
        private const float ITEM_SIZE = 50f;
        private ContentManager _content;
        private List<(Color, Texture2D, Rectangle)> _inventoryTextures;
        private List<Color> _playerInventory;

        //look into passing in textures?
        public MainInventoryScreen(List<(Color, Texture2D, Rectangle)> textures, List<Color> playerInventory) : base("Inventory")
        {
            _inventoryTextures = textures;
            _playerInventory = playerInventory;
        }

        public override void Activate()
        {
            foreach(var texture in _inventoryTextures)
            {
                var entry = new InventoryEntry(texture.Item1, texture.Item2, ITEM_SIZE, texture.Item3);
                entry.Selected += AddItemToInventory;
                InventoryEntries.Add(entry);
            }
        }

        private void AddItemToInventory(object sender, PlayerIndexEventArgs e)
        {
            _playerInventory.Add(((InventoryEntry)sender).Color);
            
        }

        // This uses the loading screen to transition from the game back to the main menu screen.
        private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }
    }
}
