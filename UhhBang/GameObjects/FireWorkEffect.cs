using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using UhhBang.Collisions;
using UhhBang.StateManagement;

namespace UhhBang.GameObjects
{
    class FireWorkEffect
    {
        private const float ANIMATION_SPEED = 0.08f;
        public string Name { get => _name; }
        private string _name { get; set; }
        private string _path { get; set; }
        private TimeSpan _lifeSpan { get; set; }
        private string _isActive { get; set; }

        
        private double _animationTimer;
        private bool _grow = true;
        private float _maxScale, _minScale, _scale;
        private Vector2 _position { get; set; }
        private Texture2D _texture { get; set; }

        private Vector2 _direction { get; set; }

        public FireWorkEffect(string name, string path, TimeSpan lifeSpan, Vector2 position)
        {
            _name = name;
            _path = path;
            _lifeSpan = lifeSpan;
            _position = position;
        }
    }
}
