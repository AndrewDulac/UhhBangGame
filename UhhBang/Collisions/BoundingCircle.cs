using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace UhhBang.Collisions
{

    /// <summary>
    /// a struct representing circular bounds
    /// </summary>
    public struct BoundingCircle
    {
        /// <summary>
        /// The center of the BoundingCircle
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// The radius of the BoundingCircle
        /// </summary>
        public float Radius;


        /// <summary>
        /// Constructs a new BoundingCircle
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public BoundingCircle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;

        }

        /// <summary>
        /// Checks if this circle is colliding with another
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True for collision, false otherwise</returns>
        public bool CollidesWith(BoundingCircle other)
        {
            return CollisionHelper.Collides(this, other);
        }

        /// <summary>
        /// Checks if this circle is colliding with a rectangle
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True for collision, false otherwise</returns>
        public bool CollidesWith(BoundingRectangle other)
        {
            return CollisionHelper.Collides(this, other);
        }
    }
}
