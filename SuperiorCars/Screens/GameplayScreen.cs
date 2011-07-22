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

        Vector2 mCarPosition = new Vector2(300, 150);
        Texture2D mCar;
        int mCarHeight;
        int mCarWidth;
        float mCarRotation = 0;
        double mCarScale = 1.0;
        RenderTarget2D mTrackRender;
        RenderTarget2D mTrackRenderRotated;
        Texture2D currentTrackOverlay;
        Texture2D currentTrack;
        

        // The time we were last moving properly
        double lastProperMove = 0;

        // The last known speed we moved with (forwards or backward)
        int lastProperDirection = 0;
        
        float pauseAlpha;
        InputAction pauseAction;

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
                mCar = content.Load<Texture2D>("Textures/Cars/Car");

                mCarWidth = (int)(mCar.Width * mCarScale);
                mCarHeight = (int)(mCar.Height * mCarScale);

                //Setup the render targets to be used in determining if the car is on the track
                mTrackRender = new RenderTarget2D(ScreenManager.GraphicsDevice, mCarWidth + 100,
                       mCarHeight + 100, false, SurfaceFormat.Color, DepthFormat.None);
                mTrackRenderRotated = new RenderTarget2D(ScreenManager.GraphicsDevice, mCarWidth + 100,
                       mCarHeight + 100, false, SurfaceFormat.Color, DepthFormat.None);

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
                /* Apply some random jitter to make the enemy move around. 
                const float randomization = 10;

                mCarPosition.X += (float)(random.NextDouble() - 0.5) * randomization;
                mCarPosition.Y += (float)(random.NextDouble() - 0.5) * randomization;

                 Apply a stabilizing force to stop the enemy moving off the screen.
                Vector2 targetPosition = new Vector2(
                    ScreenManager.GraphicsDevice.Viewport.Width / 2 - mCarWidth / 2,
                    200);

                mCarPosition = Vector2.Lerp(mCarPosition, targetPosition, 0.05f);*/

                
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
                mCarRotation += (float)(gamePadState.ThumbSticks.Left.X * 3.0f * gameTime.ElapsedGameTime.TotalSeconds);

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
                    mCarRotation -= (aMove * 0.15f) * (float)(1 * 3.0f * gameTime.ElapsedGameTime.TotalSeconds);
                }
                else if ( keyboardState.IsKeyDown(Keys.Right))
                {
                    mCarRotation += (aMove * 0.15f) * (float)(1 * 3.0f * gameTime.ElapsedGameTime.TotalSeconds);
                }

                //Check to see if a collision occured. If a collision didn't occur, then move the sprite
                if (CollisionOccurred(aMove) == false)
                {
                    //Move the sprite
                    mCarPosition.X += (float) (aMove*Math.Cos(mCarRotation));
                    mCarPosition.Y += (float) (aMove*Math.Sin(mCarRotation));
                    lastProperMove = gameTime.ElapsedGameTime.TotalMilliseconds;
                    lastProperDirection = aMove;

                }
                else // We have crashed
                {
                    double timeOfCrash = gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timeOfCrash > lastProperMove + 2000 == false)
                    {
                        if(lastProperDirection > 0)
                            aMove += (int) (200 * gameTime.ElapsedGameTime.TotalSeconds)*(int) Math.Floor((float) 0.9);
                        else
                            aMove -= (int)(200 * gameTime.ElapsedGameTime.TotalSeconds) * (int)Math.Floor((float)0.9);
                        mCarPosition.X -= (float) (aMove*Math.Cos(mCarRotation));
                        mCarPosition.Y -= (float) (aMove*Math.Sin(mCarRotation));
                    }
                }
                // TODO Fixa till nedre delen av kameran så den inte visar utanför kartan
                cam.Pos = new Vector2(MathHelper.Clamp(mCarPosition.X / 2, currentTrackOverlay.Width / 8, 960), MathHelper.Clamp(mCarPosition.Y / 2, currentTrackOverlay.Height / 8, currentTrackOverlay.Height - 180)); 
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

            // Car
            spriteBatch.Draw(mCar, new Rectangle((int)mCarPosition.X, (int)mCarPosition.Y, mCarWidth, mCarHeight),
                new Rectangle(0, 0, mCar.Width, mCar.Height), Color.White, mCarRotation,
                new Vector2(mCar.Width / 2, mCar.Height / 2), SpriteEffects.None, 0);

            spriteBatch.End();

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
            float aXPosition = (float)(-mCarWidth / 2 + mCarPosition.X + aMove * Math.Cos(mCarRotation));
            float aYPosition = (float)(-mCarHeight / 2 + mCarPosition.Y + aMove * Math.Sin(mCarRotation));
            Texture2D aCollisionCheck = CreateCollisionTexture(aXPosition, aYPosition);

            //Use GetData to fill in an array with all of the Colors of the Pixels in the area of the Collision Texture
            int aPixels = mCarWidth * mCarHeight;
            Color[] myColors = new Color[aPixels];
            aCollisionCheck.GetData<Color>(0, new Rectangle((int)(aCollisionCheck.Width / 2 - mCarWidth / 2),
                (int)(aCollisionCheck.Height / 2 - mCarHeight / 2), mCarWidth, mCarHeight), myColors, 0, aPixels);

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
            mSpriteBatch.Draw(currentTrack, new Rectangle(0, 0, mCarWidth + 100, mCarHeight + 100),
                new Rectangle((int)(theXPosition - 50),
                (int)(theYPosition - 50), mCarWidth + 100, mCarHeight + 100), Color.White);
            mSpriteBatch.End();

            g.SetRenderTarget(null);

            Texture2D aPicture = mTrackRender;

            //Rotate the snapshot of the area Around the car sprite and return that 
            g.SetRenderTarget(mTrackRenderRotated);
            g.Clear(ClearOptions.Target, Color.Red, 0, 0);

            mSpriteBatch.Begin();
            mSpriteBatch.Draw(aPicture, new Rectangle((int)(aPicture.Width / 2), (int)(aPicture.Height / 2),
                aPicture.Width, aPicture.Height), new Rectangle(0, 0, aPicture.Width, aPicture.Width),
                Color.White, -mCarRotation, new Vector2((int)(aPicture.Width / 2), (int)(aPicture.Height / 2)),
                SpriteEffects.None, 0);
            mSpriteBatch.End();

            g.SetRenderTarget(null);

            return mTrackRenderRotated;
        }

        #endregion
    }
}
