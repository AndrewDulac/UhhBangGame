using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UhhBang.StateManagement;
using System.Collections.Generic;
using UhhBang.GameObjects;
using UhhBang.GameObjects.Particles;
using Microsoft.Xna.Framework.Audio;
using System.Linq;

namespace UhhBang.Screens
{
    public enum GameState { Tutorial, Start, Playing, End };
    public enum TutorialState { Movement, Inventory, Ignite, ClearInventory, Finished };
    public enum IgniteState { Wait, Light, Rise, Explode, Finished};
    public enum TurnState { Auto, PlayerTurn };
    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen
    {

        GameState _gameState = GameState.Tutorial;
        TutorialState _tutorialState = TutorialState.Movement;
        IgniteState _igniteState = IgniteState.Wait;
        TurnState _turn = TurnState.Auto;

        private const float TIME_SPLIT = 0.5f;

        private string _dispText = "";

        private int _currentLevel = 1;
        private int _maxLevel = 30;

        private List<Color> generatedSequence;

        private ContentManager _content;
        private PersonSprite player;
        private float _timeSinceLit;
        private bool _lit;
        private bool _continue;
        private List<(Color, Texture2D, Rectangle)> _inventoryTextures = new List<(Color, Texture2D, Rectangle)>();
        private int _lastIndex = -1;
        static Random random = new Random();

        private List<Color> colors = new List<Color>()
        {
            Color.Yellow,
            Color.Green,
            Color.Blue,
            Color.Red,
            Color.Purple,
            Color.Cyan
        };

        public FireworkParticleSystem FireworkParticleSystem { get; private set; }

        private Vector2 _playerMovement;
        private DirectionEnum _playerDirection;
        private SoundEffect _explosion;

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;
        private readonly InputAction _inventoryAction;
        private readonly InputAction _lightAction;
        private readonly InputAction _continueAction;

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
            _lightAction = new InputAction(
                new[] { Buttons.A },
                new[] { Keys.E }, true);
            _continueAction = new InputAction(
                new[] { Buttons.Y },
                new[] { Keys.Space }, true);
        }

        // Load graphics content for the game
        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            Texture2D explosionAtlas = _content.Load<Texture2D>("Sprites/M484ExplosionSet2");

            _explosion = _content.Load<SoundEffect>("Sounds/cannon_fire");

            var sourceRect = new Rectangle(373, 296, 34, 34);
            foreach(Color c in colors)
            {
                _inventoryTextures.Add((c, explosionAtlas, sourceRect));
            }
            _inventoryTextures.Add((Color.Black, explosionAtlas, sourceRect));

            player = new PersonSprite(new Vector2(30, ScreenManager.GraphicsDevice.Viewport.Height - 50), 2f);
            var backgroundScreen = new BackgroundScreen();
            ScreenManager.AddScreen(backgroundScreen, null);

            var Game = ScreenManager.Game;

            generatedSequence = new List<Color>();
            FireworkParticleSystem = new FireworkParticleSystem(Game, 44);
            ScreenManager.Game.Components.Add(FireworkParticleSystem);

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
                player.Update(_playerMovement, _playerDirection, 200);
                switch (_gameState)
                {
                    case GameState.Tutorial:
                        switch (_tutorialState)
                        {
                            case TutorialState.Movement:
                                _dispText = "Use [WD] to move Left and Right";
                                if(Math.Abs(_playerMovement.X) > 0)
                                {
                                    _tutorialState = TutorialState.Inventory;
                                } 
                                break;
                            case TutorialState.Inventory:
                                _dispText = "Press [Tab] or (start) to open Inventory to add effects";
                                if(this.player.Inventory.Count > 0)
                                {
                                    _tutorialState = TutorialState.Ignite;
                                }
                                break;
                            case TutorialState.Ignite:
                                _dispText = "Press [E] to start effects";
                                if (_lit)
                                {
                                    _tutorialState = TutorialState.ClearInventory;
                                }
                                break;
                            case TutorialState.ClearInventory:
                                _dispText = "In your Inventory, press the black effect to clear your inventory";
                                if(this.player.Inventory.Count == 0)
                                {
                                    _gameState = GameState.Start;
                                }
                                break;
                        }
                        break;
                    case GameState.Start:
                        _dispText = "Match the effect patterns to advance levels! (Press Space to start)";
                        if (_continue)
                        {
                            _gameState = GameState.Playing;
                            _continue = false;
                        }
                        break;
                    case GameState.Playing:
                        switch (_turn)
                        {
                            case TurnState.Auto:
                                _dispText = "Level " + _currentLevel.ToString();
                                if (_lit) _lit = false;
                                _timeSinceLit += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                int i = (int)(_currentLevel * _timeSinceLit * TIME_SPLIT);
                                if (_lastIndex != i)
                                {
                                    _lastIndex = i;
                                    generatedSequence.Add(colors[random.Next(colors.Count)]);
                                    Vector2 randomPos = RandomHelper.RandomPosition(new Rectangle(500, 100, 50, 50));
                                    _explosion.Play();
                                    FireworkParticleSystem.PlaceFireWork(randomPos, generatedSequence[i]);

                                    if (i == _currentLevel + 1)
                                    {
                                        _timeSinceLit = 0;
                                        _turn = TurnState.PlayerTurn;
                                    }
                                }
                                break;
                            case TurnState.PlayerTurn:
                                _dispText = "Level " + _currentLevel.ToString() + "\n Copy the pattern!";
                                if (_igniteState == IgniteState.Finished)
                                {
                                    if(Enumerable.SequenceEqual(this.player.Inventory, generatedSequence))
                                    {
                                        _currentLevel++;
                                    }
                                    else
                                    {
                                        _gameState = GameState.End;
                                    }
                                    _turn = TurnState.Auto;
                                    player.Inventory.Clear();
                                    generatedSequence.Clear();
                                }
                                break;

                            default:
                                break;
                        }
                        break;
                    case GameState.End:
                        _dispText = "Better luck next time!";

                        break;
                }

                if (_lit)
                {
                    _igniteState = IgniteState.Light;
                    _lit = false;
                }
                switch (_igniteState)
                {
                    case IgniteState.Wait:
                        break;
                    case IgniteState.Light:
                        _igniteState = IgniteState.Rise;
                        break;
                    case IgniteState.Rise:
                        _igniteState = IgniteState.Explode;
                        break;
                    case IgniteState.Explode:
                        _timeSinceLit += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        int i = (int)(this.player.Inventory.Count * _timeSinceLit * TIME_SPLIT);
                        if (_lastIndex != i)
                        {
                            _lastIndex = i;
                            Vector2 randomPos = RandomHelper.RandomPosition(new Rectangle(100, 100, 50, 50));
                            _explosion.Play();
                            FireworkParticleSystem.PlaceFireWork(randomPos, this.player.Inventory[i]);

                            if (i == this.player.Inventory.Count - 1)
                            {
                                _timeSinceLit = 0;
                                _igniteState = IgniteState.Finished;
                            }
                        }
                        break;
                    case IgniteState.Finished:
                        _igniteState = IgniteState.Wait;
                        break;
                    default:
                        break;
                }
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
            if (_lightAction.Occurred(input, ControllingPlayer, out player) && this.player.Inventory.Count > 0)
            {
                _lit = true;
            }
            if (_continueAction.Occurred(input, ControllingPlayer, out player))
            {
                _continue = true;
            }
            if (_inventoryAction.Occurred(input, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(new MainInventoryScreen(_inventoryTextures, this.player.Inventory), ControllingPlayer);
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
                    _playerMovement = new Vector2(_playerMovement.X, 0);
                }


            }

        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            //ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            var spriteBatch = ScreenManager.SpriteBatch;
            var font = ScreenManager.Fonts["Britannic_Bold_12"];
            Vector2 textSize = font.MeasureString(_dispText);
            Vector2 textLoc = new Vector2((this.ScreenManager.GraphicsDevice.Viewport.Width / 2 - textSize.X / 2), 20);


            spriteBatch.Begin();
            spriteBatch.DrawString(font, _dispText, textLoc, Color.LightSlateGray);
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
