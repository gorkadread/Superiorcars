#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

#endregion

namespace SuperiorCars.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        Random random = new Random();
        Camera2d cam = new Camera2d();
        float pauseAlpha;
        InputAction pauseAction;

        // Starting positions
        Car playerCar = new Car(300, 150);
        Car enemyCar1 = new Car(300, 200);

        // Jingles and jazz to determine collision
        RenderTarget2D mTrackRender;
        RenderTarget2D mTrackRenderRotated;
        Texture2D currentTrackOverlay;
        Texture2D currentTrack;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("Fonts/gamefont");
                currentTrack = content.Load<Texture2D>("Textures/Tracks/Track1");
                currentTrackOverlay = content.Load<Texture2D>("Textures/Tracks/Track1overlay");
                playerCar.Texture = content.Load<Texture2D>("Textures/Cars/Car");
                enemyCar1.Texture = content.Load<Texture2D>("Textures/Cars/Car");

                playerCar.Width = (int)(playerCar.Texture.Width * playerCar.Scale);
                playerCar.Height = (int)(playerCar.Texture.Height * playerCar.Scale);

                enemyCar1.Width = (int)(enemyCar1.Texture.Width * enemyCar1.Scale);
                enemyCar1.Height = (int)(enemyCar1.Texture.Height * enemyCar1.Scale);

                //Setup the render targets to be used in determining if the car is on the track
                mTrackRender = new RenderTarget2D(ScreenManager.GraphicsDevice, playerCar.Width + 100,
                       playerCar.Height + 100, false, SurfaceFormat.Color, DepthFormat.None);
                mTrackRenderRotated = new RenderTarget2D(ScreenManager.GraphicsDevice, playerCar.Width + 100,
                       playerCar.Height + 100, false, SurfaceFormat.Color, DepthFormat.None);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
#endif
        }


        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // Apply some random jitter to make the enemy move around. 
                const float randomization = 10;

                enemyCar1.X += (float)(random.NextDouble() - 0.5) * randomization;
                enemyCar1.Y += (float)(random.NextDouble() - 0.5) * randomization;

                /* Apply a stabilizing force to stop the enemy moving off the screen.
                Vector2 targetPosition = new Vector2(
                    ScreenManager.GraphicsDevice.Viewport.Width / 2 - mCarWidth / 2,
                    200);

                mCarPosition = Vector2.Lerp(mCarPosition, targetPosition, 0.05f); */

                
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
#endif
            }
            else
            {

                //Rotate the Car sprite with the Left Thumbstick or the up and down arrows
                playerCar.Rotation += (float)(gamePadState.ThumbSticks.Left.X * 3.0f * gameTime.ElapsedGameTime.TotalSeconds);

                // Camera zoom
                if (keyboardState.IsKeyDown(Keys.N)) cam.IncrementZoom(0.1f);
                if (keyboardState.IsKeyDown(Keys.M)) cam.IncrementZoom(-0.1f);

                int aMove = 0;

                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    aMove += (int)(200 * gameTime.ElapsedGameTime.TotalSeconds);
                }
                else if (keyboardState.IsKeyDown(Keys.Down))
                {
                    aMove -= (int)(100 * gameTime.ElapsedGameTime.TotalSeconds);
                }

                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    playerCar.Rotation -= (aMove * 0.15f) * (float)(1 * 3.0f * gameTime.ElapsedGameTime.TotalSeconds);
                }
                else if ( keyboardState.IsKeyDown(Keys.Right))
                {
                    playerCar.Rotation += (aMove * 0.15f) * (float)(1 * 3.0f * gameTime.ElapsedGameTime.TotalSeconds);
                }

                //Check to see if a collision occured. If a collision didn't occur, then move the sprite
                if (CollisionOccurred(aMove) == false)
                {
                    //Move the sprite
                    playerCar.X += (float)(aMove * Math.Cos(playerCar.Rotation));
                    playerCar.Y += (float)(aMove * Math.Sin(playerCar.Rotation));
                    playerCar.LastProperMove = gameTime.ElapsedGameTime.TotalMilliseconds;
                    playerCar.LastProperDirection = aMove;

                }
                else // We have crashed
                {
                    double timeOfCrash = gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timeOfCrash > playerCar.LastProperMove + 2000 == false)
                    {
                        if (playerCar.LastProperDirection > 0)
                            aMove += (int) (200 * gameTime.ElapsedGameTime.TotalSeconds)*(int) Math.Floor((float) 0.9);
                        else
                            aMove -= (int)(200 * gameTime.ElapsedGameTime.TotalSeconds) * (int)Math.Floor((float)0.9);
                        playerCar.X -= (float)(aMove * Math.Cos(playerCar.Rotation));
                        playerCar.Y -= (float)(aMove * Math.Sin(playerCar.Rotation));
                    }
                }
                // TODO Fixa till nedre delen av kameran så den inte visar utanför kartan
                cam.Pos = new Vector2(MathHelper.Clamp(playerCar.X / 2, currentTrackOverlay.Width / 8, 960), MathHelper.Clamp(playerCar.Y / 2, currentTrackOverlay.Height / 8, currentTrackOverlay.Height - 180)); 
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Camera
            spriteBatch.Begin(SpriteSortMode.Immediate,
                                    BlendState.AlphaBlend,
                                    null,
                                    null,
                                    null,
                                    null,
                                    cam.get_transformation(ScreenManager.GraphicsDevice));

            // Map
            spriteBatch.Draw(currentTrackOverlay, new Rectangle(0, 0, currentTrackOverlay.Width, currentTrackOverlay.Height), Color.White);

            // playerCar
            spriteBatch.Draw(playerCar.Texture, new Rectangle((int)playerCar.X, (int)playerCar.Y, playerCar.Width, playerCar.Height),
                new Rectangle(0, 0, playerCar.Width, playerCar.Height), Color.White, playerCar.Rotation,
                new Vector2(playerCar.Width / 2, playerCar.Height / 2), SpriteEffects.None, 0);

            // enemyCar
            spriteBatch.Draw(enemyCar1.Texture, new Rectangle((int)enemyCar1.X, (int)enemyCar1.Y, enemyCar1.Width, enemyCar1.Height),
                new Rectangle(0, 0, enemyCar1.Width, enemyCar1.Height), Color.White, enemyCar1.Rotation,
                new Vector2(enemyCar1.Width / 2, enemyCar1.Height / 2), SpriteEffects.None, 0);

            spriteBatch.End();
            //pewpew
            // Since the explosions wants additive, we'll do this last
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            //DrawExplosion();
            //spriteBatch.End();


            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion

        #region Collision Detection methods

        //This method checks to see if the Sprite is going to move into an area that does
        //not contain all Gray pixels. If the move amount would cause a movement into a non-gray
        //pixel, then a collision has occurred.
        private bool CollisionOccurred(int aMove)
        {
            //Calculate the Position of the Car and create the collision Texture. This texture will contain
            //all of the pixels that are directly underneath the sprite currently on the Track image.
            float aXPosition = (float)(-playerCar.Width / 2 + playerCar.X + aMove * Math.Cos(playerCar.Rotation));
            float aYPosition = (float)(-playerCar.Height / 2 + playerCar.Y + aMove * Math.Sin(playerCar.Rotation));
            Texture2D aCollisionCheck = CreateCollisionTexture(aXPosition, aYPosition);

            //Use GetData to fill in an array with all of the Colors of the Pixels in the area of the Collision Texture
            int aPixels = playerCar.Width * playerCar.Height;
            Color[] myColors = new Color[aPixels];
            aCollisionCheck.GetData<Color>(0, new Rectangle((int)(aCollisionCheck.Width / 2 - playerCar.Width / 2),
                (int)(aCollisionCheck.Height / 2 - playerCar.Height / 2), playerCar.Width, playerCar.Height), myColors, 0, aPixels);

            //Cycle through all of the colors in the Array and see if any of them
            //are not Gray. If one of them isn't Gray, then the Car is heading off the road
            //and a Collision has occurred
            bool aCollision = false;
            foreach (Color aColor in myColors)
            {
                //If one of the pixels in that area is not Gray, then the sprite is moving
                //off the allowed movement area
                if (aColor != Color.Gray)
                {
                    aCollision = true;
                    break;
                }
            }

            return aCollision;
        }


        //Create the Collision Texture that contains the rotated Track image for determine
        //the pixels beneath the Car srite.
        private Texture2D CreateCollisionTexture(float theXPosition, float theYPosition)
        {
            SpriteBatch mSpriteBatch = ScreenManager.SpriteBatch;
            var g = ScreenManager.GraphicsDevice;

            //Grab a square of the Track image that is around the Car
            g.SetRenderTarget(mTrackRender);
            g.Clear(ClearOptions.Target, Color.Red, 0, 0);

            
            mSpriteBatch.Begin();
            mSpriteBatch.Draw(currentTrack, new Rectangle(0, 0, playerCar.Width + 100, playerCar.Height + 100),
                new Rectangle((int)(theXPosition - 50),
                (int)(theYPosition - 50), playerCar.Width + 100, playerCar.Height + 100), Color.White);
            mSpriteBatch.End();

            g.SetRenderTarget(null);

            Texture2D aPicture = mTrackRender;

            //Rotate the snapshot of the area Around the car sprite and return that 
            g.SetRenderTarget(mTrackRenderRotated);
            g.Clear(ClearOptions.Target, Color.Red, 0, 0);

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(aPicture, new Rectangle((int)(aPicture.Width / 2), (int)(aPicture.Height / 2),
                aPicture.Width, aPicture.Height), new Rectangle(0, 0, aPicture.Width, aPicture.Width),
                Color.White, -playerCar.Rotation, new Vector2((int)(aPicture.Width / 2), (int)(aPicture.Height / 2)),
                SpriteEffects.None, 0);
            mSpriteBatch.End();

            g.SetRenderTarget(null);

            return mTrackRenderRotated;
        }

        #endregion
    }
}
