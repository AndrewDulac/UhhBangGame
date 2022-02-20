using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using UhhBang.Collisions;
using UhhBang.StateManagement;


namespace UhhBang
{
    public class MouseSprite
    {

        private Point center;

        /// <summary>
        /// bounding volume of the sprite
        /// </summary>
        public Point Bounds => center;

        /// <summary>
        /// Creates a new coin sprite
        /// </summary>
        /// <param name="position">The position of the sprite in the game</param>
        public MouseSprite()
        {
            center = new Point(0,0);
        }


        /// <summary>
        /// Updates the sprite's position based on user input
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public void Update(InputState input)
        {
            center = input.CurrentMouseStates[0].Position;
        }

        /// <summary>
        /// Draws the animated sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;

            spriteBatch.Draw(
                texture,
                center.ToVector2(),
                null,
                Color.White,
                0,
                new Vector2(32,32),
                0.35f,
                spriteEffects,
                0
            );
        }
    }
}
