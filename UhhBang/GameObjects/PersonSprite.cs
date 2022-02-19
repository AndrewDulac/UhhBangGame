using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using UhhGame.Collisions;

namespace UhhGame
{
    /// <summary>
    /// A class representing a slime ghost
    /// </summary>
    public class PersonSprite
    { 
        private float scale;

        private const int height = 32;

        private const int width = 32;

        private Texture2D idleTexture;

        private bool running = false;

        private Texture2D runTexture;

        private Vector2 position;

        private BoundingRectangle bounds;

        private double animationTimer;

        private short animationFrame = 1;
        /// <summary>
        /// direction of the bat
        /// </summary>
        public DirectionEnum DirectionState;

        /// <summary>
        /// The color blend with the ghost;
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// bounding volume of the sprite
        /// </summary>
        public BoundingRectangle Bounds => bounds;

        public PersonSprite(Vector2 position, float scale)
        {
            this.position = position;
            this.scale = scale;
            this.DirectionState = DirectionEnum.Down;
            this.bounds = new BoundingRectangle(
                position - new Vector2(scale * width/4, scale * height/2),
                (scale * width / 2),
                scale * height );
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            idleTexture = content.Load<Texture2D>("Player/IdleSheet");
            runTexture = content.Load<Texture2D>("Player/RunSheet");
        }

        /// <summary>
        /// Updates the sprite's position based on user input
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public void Update(Vector2 dir, DirectionEnum direction, float velocity)
        {
            running = false;
            if (Math.Abs(dir.X) > 0 || Math.Abs(dir.Y) > 0)
            {
                running = true;
                DirectionState = direction;
                position += dir * velocity;
            }
            /*
            var viewport = game.GraphicsDevice.Viewport;
            if (position.Y < 0) position.Y = viewport.Height;
            if (position.Y > viewport.Height) position.Y = 0;
            if (position.X < 0) position.X = viewport.Width;
            if (position.X > viewport.Width) position.X = 0;
            */
            // Update the bounds
            bounds.X = position.X - ((width * scale) / 4);
            bounds.Y = position.Y - ((height * scale) / 2);

        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;

            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > 0.08)
            {
                animationFrame++;
                if (animationFrame > 7) animationFrame = 1;
                animationTimer -= 0.08;
            }
            var source = new Rectangle(animationFrame * 32, (int)DirectionState * 32 + 1, 32, 32);
            if (running) 
            {
                spriteBatch.Draw(
                    runTexture,
                    position,
                    source,
                    Color.White,
                    0,
                    new Vector2(width/2, height/2),
                    scale,
                    spriteEffects,
                    0
                );
            }
            else
            {
                spriteBatch.Draw(
                    idleTexture,
                    position,
                    source,
                    Color.White,
                    0,
                    new Vector2(width/2, height/2),
                    scale,
                    spriteEffects,
                    0
                );
            }
        }
    }
}
