using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using UhhBang.Collisions;
using UhhBang.StateManagement;

namespace UhhBang.GameObjects.Effects
{
    public enum EffectState
    {
        Static,
        Preview,
        Simulation
    }
    public abstract class FireWorkEffect
    {
        protected const float ANIMATION_SPEED = 0.08f;
        public string Name { get => _name; }
        public string Path { get => _path; }

        protected EffectState _state { get; set; }
        protected string _name { get; set; }
        protected string _path { get; set; }
        protected TimeSpan _lifeSpan { get; set; }
        protected string _isAlive { get; set; }
        protected double _animationTimer { get; set; }
        protected float _maxScale, _minScale, _scale;
        protected int _animationIndex { get; set; }

        protected Vector2 _position { get; set; }
        protected Texture2D _texture { get; set; }

        protected Vector2 _direction { get; set; }
        protected FireWorkEffect(string name, string path, TimeSpan lifeSpan, Vector2 position, EffectState state)
        {
            _name = name;
            _path = path;
            _lifeSpan = lifeSpan;
            _position = position;
            _state = state;
        }
    }
}
