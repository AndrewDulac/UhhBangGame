using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UhhBang.StateManagement;


namespace UhhBang.GameObjects.Particles
{
    public class FireworkParticleSystem : ParticleSystem
    {

        private const int WIDTH = 34;
        private const int HEIGHT = 34;

        private float _angle;
        private Color _color;

        public FireworkParticleSystem(Game game, int maxExplosions) : base(game, maxExplosions * 25, new Vector2(WIDTH/2, HEIGHT/2))
        {

        }
        protected override void InitializeConstants()
        {
            textureFilename = "Sprites/M484ExplosionSet2";

            minNumParticles = 22;
            maxNumParticles = 22;

            blendState = BlendState.AlphaBlend;
            DrawOrder = AdditiveBlendDrawOrder;

        }
        protected override void InitializeParticle(ref Particle p, Vector2 where, Color color)
        {
            var velocity = NextDirection() * 150;

            var lifetime = 1f;

            var acceleration = -velocity / lifetime;

            var rotation = RandomHelper.NextFloat(0, MathHelper.TwoPi);

            var angularVelocity = RandomHelper.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);

            var scale = .25f;

            var alpha = 50;
            color.A = 50;

            var sourceRect = new Rectangle(373, 296, 34, 34);

            var animationFrames = new Rectangle[7];
            for(int i = 0; i < animationFrames.Length; i++)
            {
                animationFrames[i] = new Rectangle(sourceRect.X + i * sourceRect.Width, sourceRect.Y, sourceRect.Width, sourceRect.Height);
            }

            p.Initialize(where, velocity, acceleration, color, sourceRect, animationFrames, lifetime: lifetime, rotation: rotation, angularVelocity: angularVelocity, scale: scale);
        }

        protected override void UpdateParticle(ref Particle particle, float dt)
        {
            base.UpdateParticle(ref particle, dt);

            float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;

            particle.Scale = .05f + .25f * normalizedLifetime;

            if (particle.AnimationSequence != null)
            {
                //particle.SourceIndex = (int)(normalizedLifetime * particle.AnimationSequence.Length);
                particle.SourceIndex = (int)(Math.Pow(normalizedLifetime, 2) * (particle.AnimationSequence.Length - 1));
                //particle.SourceIndex = (int)Math.Pow(particle.AnimationSequence.Length + 1, normalizedLifetime) - 1;
            }
        }

        public void PlaceFireWork(Vector2 where, Color color)
        {
            AddParticles(where, color);
        }

        public Vector2 NextDirection()
        {
            if(_angle > 360) { _angle = 0; }
            else { _angle += 2; }
            return new Vector2(MathF.Cos(_angle), MathF.Sin(_angle));
        }
    }
}
