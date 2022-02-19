using UhhGame.StateManagement;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace UhhGame.Screens
{
    // The pause menu comes up over the top of the game,
    // giving the player options to resume or quit.
    public class MainInventoryScreen : InventoryScreen
    {
        
        private ContentManager _content;
        private Dictionary<string, string> _assets = new Dictionary<string, string>()
        {
            {"crate", "Sprites/crate" },
            {"crate1", "Sprites/crate" },
            {"crate2", "Sprites/crate" },
            {"crate3", "Sprites/crate" },
            {"crate4", "Sprites/crate" },
            {"crate5", "Sprites/crate" },
            {"crate6", "Sprites/crate" },
            {"crate7", "Sprites/crate" },
        };

        public MainInventoryScreen() : base("Inventory")
        {

            
        }

        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            foreach (var item in _assets)
            {
                var entry = new InventoryEntry(_content.Load<Texture2D>(item.Value), ITEM_SIZE);
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
