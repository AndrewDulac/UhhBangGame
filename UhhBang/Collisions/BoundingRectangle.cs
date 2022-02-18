using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace UhhGame.Collisions
{

    /// <summary>
    /// a bounding rectangle for collision detection
    /// </summary>
    public struct BoundingRectangle
    {
        public float X;

        public float Y;

        public float Width;

        public float Height;

        public float Left => X;

        public float Right => X + Width;

        public float Top => Y;

        public float Bottom => Y + Height;

        public BoundingRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

        }

        public BoundingRectangle(Vector2 position, float width, float height)
        {
            X = position.X;
            Y = position.Y;
            Height = height;
            Width = width;
        }

        /// <summary>
        /// Detects collision between two BoundingRectangles
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True for collision, false otherwise</returns>
        public bool CollidesWith(BoundingRectangle other)
        {
            return CollisionHelper.Collides(this, other);
        }

        /// <summary>
        /// Checks if this rectagnle is colliding with a BoundingCircle
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True for collision, false otherwise</returns>
        public bool CollidesWith(BoundingCircle other)
        {
            return CollisionHelper.Collides(other, this);
        }
    }
}
