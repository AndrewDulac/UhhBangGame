﻿using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UhhGame.StateManagement;

namespace UhhGame.Screens
{
    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteFont _gameFont;
        private PersonSprite player;

        private Vector2 _playerMovement;
        private DirectionEnum _playerDirection;

        private Vector2 _enemyPosition = new Vector2(100, 100);

        private Texture2D _backgroundTexture;

        private readonly Random _random = new Random();

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;
        private readonly InputAction _inventoryAction;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);
            _inventoryAction = new InputAction(
                new[] { Buttons.DPadUp },
                new[] { Keys.Tab }, true);
        }

        // Load graphics content for the game
        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            player = new PersonSprite(new Vector2(30, ScreenManager.GraphicsDevice.Viewport.Height - 50), 2f);
            var backgroundScreen = new BackgroundScreen();
            ScreenManager.AddScreen(backgroundScreen, null);
            player.LoadContent(_content);
            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            
            ScreenManager.Game.ResetElapsedTime();
        }


        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // Apply some random jitter to make the enemy move around.
                const float randomization = 10;
                player.Update(_playerMovement, _playerDirection, 200);
                // This game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            if (_inventoryAction.Occurred(input, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(new MainInventoryScreen(), ControllingPlayer);
            }
            else
            {
                #region direction
                // TODO: Assign actions based on input
                // Right thumbstick
                _playerMovement = Vector2.Zero;
                _playerMovement += gamePadState.ThumbSticks.Left * 1;

                // WASD keys:
                if (keyboardState.IsKeyDown(Keys.Left) ||
                    keyboardState.IsKeyDown(Keys.A))
                    _playerMovement += new Vector2(-1, 0);
                if (keyboardState.IsKeyDown(Keys.Right) ||
                    keyboardState.IsKeyDown(Keys.D))
                    _playerMovement += new Vector2(1, 0);
                if (keyboardState.IsKeyDown(Keys.Up) ||
                    keyboardState.IsKeyDown(Keys.W))
                    _playerMovement += new Vector2(0, -1);
                if (keyboardState.IsKeyDown(Keys.Down) ||
                    keyboardState.IsKeyDown(Keys.S))
                    _playerMovement += new Vector2(0, 1);
                #endregion

                if (Math.Abs(_playerMovement.X) > 0 || Math.Abs(_playerMovement.Y) > 0)
                {
                    // should probably change this to if statements to capture gamepadinput.

                    if (_playerMovement.X == 0 && _playerMovement.Y < -.01f)
                        _playerDirection = DirectionEnum.Up;

                    else if (_playerMovement.X == 0 && _playerMovement.Y > .01f)
                        _playerDirection = DirectionEnum.Down;

                    else if (_playerMovement.X < -.01f && _playerMovement.Y == 0)
                        _playerDirection = DirectionEnum.Left;

                    else if (_playerMovement.X > .01f && _playerMovement.Y == 0)
                        _playerDirection = DirectionEnum.Right;

                    else if (_playerMovement.X > .01f && _playerMovement.Y < -.01f)
                        _playerDirection = DirectionEnum.UpRight;

                    else if (_playerMovement.X > .01f && _playerMovement.Y > .01f)
                        _playerDirection = DirectionEnum.DownRight;

                    else if (_playerMovement.X < -.01f && _playerMovement.Y > .01f)
                        _playerDirection = DirectionEnum.DownLeft;

                    else if (_playerMovement.X < -.01f && _playerMovement.Y < -.01f)
                        _playerDirection = DirectionEnum.UpLeft;

                    else _playerDirection = DirectionEnum.Down;

                    _playerMovement = _playerMovement * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Math.Abs(_playerMovement.X) + Math.Abs(_playerMovement.Y) > 1) _playerMovement.Normalize();
                }


            }
        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            //ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            player.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
