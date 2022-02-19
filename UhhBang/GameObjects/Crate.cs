using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using UhhGame.Collisions;
using UhhGame.StateManagement;

namespace UhhBang.GameObjects
{
    public class Crate
    {
        private Vector2 _position;
        private int _tier;
        private List<FireWorkEffect> _items;

        private Texture2D _texture;
        private BoundingRectangle _bounds;

        public Crate(Vector2 position)
        {
            _position = position;
        }
    }
}
