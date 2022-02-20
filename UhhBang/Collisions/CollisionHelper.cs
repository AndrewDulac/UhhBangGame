using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;


namespace UhhBang.Collisions
{
    public static class CollisionHelper
    {

        /// <summary>
        /// Detects collision between two BoundingCircles
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True for collision, false otherwise</returns>
        public static bool Collides(BoundingCircle a, BoundingCircle b)
        {
            return Math.Pow(a.Radius + b.Radius, 2) >=
                (Math.Pow(a.Center.X - b.Center.X, 2) +
                 Math.Pow(a.Center.Y - b.Center.Y, 2));
        }

        /// <summary>
        /// Detects collision between two BoundingRectangles
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True for collision, false otherwise</returns>
        public static bool Collides(BoundingRectangle a, BoundingRectangle b)
        {
            return !(a.Right < b.Left || a.Left > b.Right ||
                     a.Top > b.Bottom || a.Bottom < b.Top);
        }

        /// <summary>
        /// Detects collision between a BoundingCircle and a BoundingRectangle
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Collides(BoundingCircle a, BoundingRectangle b)
        {
            float nearestX = MathHelper.Clamp(a.Center.X, b.Left, b.Right);
            float nearestY = MathHelper.Clamp(a.Center.Y, b.Top, b.Bottom);
            return Math.Pow(a.Radius, 2) >=
                  (Math.Pow(a.Center.X - nearestX, 2) +
                   Math.Pow(a.Center.Y - nearestY, 2));
        }

        /// <summary>
        /// Detects collision between a Point and a BoundingRectangle
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Collides(Point a, BoundingRectangle b)
        {
            return !(a.X < b.Left || a.X > b.Right ||
                     a.Y > b.Bottom || a.Y < b.Top);
        }

        /// <summary>
        /// Detects collision between a Point and a Boundingcircle
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Collides(Point a, BoundingCircle b)
        {
            return Math.Pow(b.Radius, 2) >=
                (Math.Pow(a.X - b.Center.X, 2) +
                 Math.Pow(a.Y - b.Center.Y, 2));
        }
    }
}
