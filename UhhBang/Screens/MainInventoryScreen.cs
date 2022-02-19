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
            {"star", "Sprites/star" },
            {"star1", "Sprites/star" },
            {"star2", "Sprites/star" },
            {"star3", "Sprites/star" },
            {"star4", "Sprites/star" },
            {"star5", "Sprites/star" },
            {"star6", "Sprites/star" },
            {"star7", "Sprites/star" },
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
