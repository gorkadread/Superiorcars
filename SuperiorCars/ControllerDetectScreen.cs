using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SuperiorCars
{
    class ControllerDetectScreen : Screen
    {
        // Text
        private SpriteFont Miramonte;
#if WINDOWS
        private string detectControllerText = "Press Enter to begin";
#elif XBOX
        private string detectControllerText = "Press A to begin";
#else //WINDOWS_PHONE
        private string detectControllerText = "Touch screen to begin";
#endif
        
        int mAlphaValue = 150;
        int mFadeIncrement = 3;

        // Background texture for the screen
        Texture2D mControllerDetectScreenBackground;
        public ControllerDetectScreen(ContentManager theContent, EventHandler theScreenEvent) : base(theScreenEvent)
        {
            // Load the background texture for the screen
            mControllerDetectScreenBackground = theContent.Load<Texture2D>("Peripherals/startbg");

            // Load the font for the text
            Miramonte = theContent.Load<SpriteFont>("Fonts/Miramonte");
        }

        public override void Update(GameTime theTime)
        {
            //Poll all the gamepads (and the keyboard) to check to see
            //which controller will be the player one controller. When the controlling
            //controller is detected, call the screen event associated with this screen
            for (int aPlayer = 0; aPlayer < 4; aPlayer++ )
            {
                if (GamePad.GetState((PlayerIndex)aPlayer).Buttons.A == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    PlayerOne = (PlayerIndex) aPlayer;
                    ScreenEvent.Invoke(this, new EventArgs());
                    return;
                }
            }

            //Increment/Decrement the fade value for the image
            mAlphaValue += mFadeIncrement;

            //If the AlphaValue is equal or above the max Alpha value or
            //has dropped below or equal to the min Alpha value, then 
            //reverse the fade
            if (mAlphaValue >= 255 || mAlphaValue <= 150)
            {
                mFadeIncrement *= -1;
            }


            base.Update(theTime);
        }

        public override void Draw(SpriteBatch theBatch)
        {
            theBatch.Draw(mControllerDetectScreenBackground, Vector2.Zero, Color.White);
            theBatch.DrawString(Miramonte, detectControllerText, new Vector2((1280 / 2) - (Miramonte.MeasureString(detectControllerText).X / 2), (760 / 2) - (Miramonte.MeasureString(detectControllerText).Y / 2)), new Color((byte)MathHelper.Clamp(mAlphaValue, 150, 255), (byte)MathHelper.Clamp(mAlphaValue, 150, 255), (byte)MathHelper.Clamp(mAlphaValue, 150, 255), (byte)MathHelper.Clamp(mAlphaValue, 150, 255)));
            base.Draw(theBatch);
        }

    }
}
