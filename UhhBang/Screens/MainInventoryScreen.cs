using UhhBang.StateManagement;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace UhhBang.Screens
{
    // The pause menu comes up over the top of the game,
    // giving the player options to resume or quit.
    public class MainInventoryScreen : InventoryScreen
    {
        private const float ITEM_SIZE = 50f;
        private ContentManager _content;
        private List<Texture2D> _inventoryTextures;

        //look into passing in textures?
        public MainInventoryScreen(List<Texture2D> textures) : base("Inventory")
        {
            _inventoryTextures = textures;
        }

        public override void Activate()
        {
            foreach(var texture in _inventoryTextures)
            {
                var entry = new InventoryEntry(texture, ITEM_SIZE);
                entry.Selected += QuitGameMenuEntrySelected;
                InventoryEntries.Add(entry);
            }
        }

        private void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";
            var confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        // This uses the loading screen to transition from the game back to the main menu screen.
        private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }
    }
}
