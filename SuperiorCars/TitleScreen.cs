using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SuperiorCars
{
    class TitleScreen : Screen
    {
        // Background texture for the title screen
        private Texture2D mTitleScreenBackground;

        public TitleScreen(ContentManager theContent, EventHandler theScreenEvent): base(theScreenEvent)
        {
            // Load the background texture for the screen
            mTitleScreenBackground = theContent.Load<Texture2D>("Peripherals/startbgTest");
        }

        // Update all of the elements that need updating in the title screen
        public override void Update(GameTime theTime)
        {
            // Check to see if the player one controller has pressed the "B"/Escape button. If so,
            // then call the screen event associated with this screen
            if (GamePad.GetState(PlayerOne).Buttons.B == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                ScreenEvent.Invoke(this, new EventArgs());
            }

            base.Update(theTime);
        }

        public override void Draw(SpriteBatch theBatch)
        {
            theBatch.Draw(mTitleScreenBackground, Vector2.Zero, Color.White);
            base.Draw(theBatch);
        }
    }
}