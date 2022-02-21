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
    public class RedFireWorkEffect : FireWorkEffect
    {


        public RedFireWorkEffect(string name, string path, TimeSpan lifeSpan, Vector2 position, EffectState state) 
            : base(name, path, lifeSpan, position, state)
        {
            
        }

        private Rectangle GetNextSourceRectangle()
        {

        }
    }
}
