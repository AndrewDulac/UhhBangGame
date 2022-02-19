using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UhhGame.StateManagement;

namespace UhhGame.Screens
{
    // Base class for screens that contain a inventory of options. The user can
    // move up and down to select an entry, or cancel to back out of the screen.
    public abstract class InventoryScreen : GameScreen
    {
        private const int NUM_COLS = 4;
        protected const float ITEM_SIZE = 64f;

        private readonly List<InventoryEntry> _inventoryEntries = new List<InventoryEntry>();
        private int _selectedEntry;
        private readonly string _inventoryTitle;
        private bool _mouseColliding;

        private readonly MouseSprite _mouse;
        private readonly InputAction _inventoryUp;
        private readonly InputAction _inventoryDown;
        private readonly InputAction _inventoryLeft;
        private readonly InputAction _inventoryRight;
        private readonly InputAction _inventorySelect;
        private readonly InputAction _inventoryCancel;
        private readonly InputAction _leftClick;

        // Gets the list of inventory entries, so derived classes can add or change the inventory contents.
        protected IList<InventoryEntry> InventoryEntries => _inventoryEntries;

        protected InventoryScreen(string inventoryTitle)
        {
            _inventoryTitle = inventoryTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _mouse = new MouseSprite();
            _inventoryUp = new InputAction(
                new[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new[] { Keys.Up }, true);
            _inventoryDown = new InputAction(
                new[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new[] { Keys.Down }, true);
            _inventoryLeft = new InputAction(
                new[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft },
                new[] { Keys.Left }, true);
            _inventoryRight = new InputAction(
                new[] { Buttons.DPadRight, Buttons.LeftThumbstickRight },
                new[] { Keys.Right }, true);
            _inventorySelect = new InputAction(
                new[] { Buttons.A, Buttons.Start },
                new[] { Keys.Enter, Keys.Space}, true);
            _inventoryCancel = new InputAction(
                new[] { Buttons.B, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);
            _leftClick = new InputAction(
                true, true);
        }

        // Responds to user input, changing the selected entry and accepting or cancelling the inventory.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            // For input tests we pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            _mouse.Update(input);

            if (_inventoryUp.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _selectedEntry -= NUM_COLS;
                if (_selectedEntry < 0)
                    _selectedEntry = _inventoryEntries.Count - NUM_COLS;
            }

            if (_inventoryDown.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _selectedEntry += NUM_COLS;

                if (_selectedEntry >= _inventoryEntries.Count)
                    _selectedEntry = _selectedEntry - _inventoryEntries.Count;
            }

            if (_inventoryLeft.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _selectedEntry -= 1;
                if ((_selectedEntry % NUM_COLS) == NUM_COLS - 1 || _selectedEntry < 0)
                {
                    _selectedEntry += NUM_COLS; //should bring to end of column
                }
            }

            if (_inventoryRight.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _selectedEntry += 1;
                if ((_selectedEntry % NUM_COLS) == 0)
                {
                    _selectedEntry -= NUM_COLS; //should bring to beginnning of column
                }
            }

            if (_inventorySelect.Occurred(input, ControllingPlayer, out playerIndex) || 
                (_mouseColliding && _leftClick.LeftClickOccurred(input, ControllingPlayer, out playerIndex)))
            {
                OnSelectEntry(_selectedEntry, playerIndex);
            }

            else if (_inventoryCancel.Occurred(input, ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }

        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            _inventoryEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }

        // Helper overload makes it easy to use OnCancel as a InventoryEntry event handler.
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        // Allows the screen the chance to position the inventory entries. By default,
        // all inventory entries are lined up in a vertical list, centered on the screen.
        protected virtual void UpdateInventoryEntryLocations()
        {
            // Make the inventory slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            var ystart = 175f;
            var position = new Vector2(0, 0);
            // update each inventory entry's location in turn
            for (int i = 0; i < _inventoryEntries.Count; i++)
            {
                var width = _inventoryEntries[i].GetWidth();
                var height = _inventoryEntries[i].GetHeight();
                var xloc = i % NUM_COLS;
                var yloc = i / NUM_COLS;
                // each entry is to be centered horizontally
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - width * NUM_COLS / 2 + (width * xloc);
                position.Y = yloc * height + ystart;


                if (ScreenState == ScreenState.TransitionOn)
                    position.Y -= transitionOffset * 128;
                else
                    position.Y += transitionOffset * 256;

                // set the entry's position
                _inventoryEntries[i].Position = position;

                // move down for the next entry the size of this entry
                
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested InventoryEntry object.
            _mouseColliding = false;
            for (int i = 0; i < _inventoryEntries.Count; i++)
            {
                if (_inventoryEntries[i].Bounds.CollidesWith(_mouse.Bounds))
                {
                    _selectedEntry = i;
                    _mouseColliding = true;
                }
                bool isSelected = IsActive && i == _selectedEntry;
                _inventoryEntries[i].Update(this, isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateInventoryEntryLocations();

            var graphics = ScreenManager.GraphicsDevice;
            var spriteBatch = ScreenManager.SpriteBatch;
            var font = ScreenManager.Fonts["gamefont"];

            spriteBatch.Begin();
            
            for (int i = 0; i < _inventoryEntries.Count; i++)
            {
                var InventoryEntry = _inventoryEntries[i];
                bool isSelected = IsActive && i == _selectedEntry;
                InventoryEntry.Draw(this, isSelected, gameTime);
            }

            // Make the inventory slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the inventory title centered on the screen
            var titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            var titleOrigin = font.MeasureString(_inventoryTitle) / 2;
            var titleColor = new Color(192, 192, 192) * TransitionAlpha;
            const float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, _inventoryTitle, titlePosition, titleColor,
                0, titleOrigin, titleScale, SpriteEffects.None, 0);
            _mouse.Draw(spriteBatch, ScreenManager.CursorTexture);
            spriteBatch.End();
        }
    }
}
