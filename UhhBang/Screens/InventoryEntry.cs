﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UhhBang.StateManagement;
using UhhBang.Collisions;

namespace UhhBang.Screens
{
    // Helper class represents a single entry in a MenuScreen. By default this
    // just draws the entry text string, but it can be customized to display menu
    // entries in different ways. This also provides an event that will be raised
    // when the menu entry is selected.
    public class InventoryEntry
    {
        private BoundingRectangle bounds;
        private Texture2D _texture;
        private float _scale;
        private float _selectionFade;    // Entries transition out of the selection effect when they are deselected
        private Vector2 _position;    // This is set by the MenuScreen each frame in Update

        /// <summary>
        /// bounding volume of the sprite
        /// </summary>
        public BoundingRectangle Bounds => bounds;

        public float Scale
        {
            private get => _scale;
            set => _scale = value;
        }
        public Texture2D Texture
        {
            private get => _texture;
            set => _texture = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public event EventHandler<PlayerIndexEventArgs> Selected;
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            Selected?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
        }

        public InventoryEntry(Texture2D texture, float size)
        {
            _texture = texture;
            _scale = size / _texture.Width;
        }

        public virtual void Update(InventoryScreen screen, bool isSelected, GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1);
            else
                _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0);
        }


        // This can be overridden to customize the appearance.
        public virtual void Draw(InventoryScreen screen, bool isSelected, GameTime gameTime)
        {
            var color = isSelected ? Color.Yellow : Color.White;
            var scale = isSelected ? 1.1f * _scale : _scale;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            var screenManager = screen.ScreenManager;
            var spriteBatch = screenManager.SpriteBatch;

            var origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            bounds = new BoundingRectangle(_position - new Vector2(GetWidth()/2,GetHeight()/2), GetWidth(), GetHeight());
            spriteBatch.Draw(
                _texture,
                _position,
                null,
                color, 
                0,
                origin,
                scale,
                SpriteEffects.None,
                0
             );
        }

        public virtual int GetHeight()
        {
            return (int)(_texture.Height * _scale);
        }

        public virtual int GetWidth()
        {
            return (int)(_texture.Width * _scale);
        }
    }
}
