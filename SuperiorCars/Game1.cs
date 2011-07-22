using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperiorCars.Screens;

namespace SuperiorCars
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice graphicsDev;
        ScreenManager screenManager;
        ScreenFactory screenFactory;

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public GraphicsDevice GraphicsDev
        {
            get { return graphicsDev; }
        }

        /// <summary>
        /// Main game constructor
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            TargetElapsedTime = TimeSpan.FromTicks(333333);

#if WINDOWS_PHONE
            graphics.IsFullScreen = true;

            // Choose whether you want a landscape or portait game by using one of the two helper functions.
            InitializeLandscapeGraphics();
            // InitializePortraitGraphics();
#else 
            //Initialize screen size to an ideal resolution for the XBox 360
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
#endif

            // Create the screen factory and add it to the services
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

#if WINDOWS_PHONE
            // Hook events on the PhoneApplicationService so we're notified of the application's life cycle
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Launching += 
                new EventHandler<Microsoft.Phone.Shell.LaunchingEventArgs>(GameLaunching);
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Activated += 
                new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>(GameActivated);
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Deactivated += 
                new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(GameDeactivated);
#else
            // On Windows and Xbox we just add the initial screens
            AddInitialScreens();
#endif
        }

        private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);

            // We have different menus for Windows Phone to take advantage of the touch interface
#if WINDOWS_PHONE
            screenManager.AddScreen(new PhoneMainMenuScreen(), null);
#else
            screenManager.AddScreen(new MainMenuScreen(), null);
#endif
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
/*
#if WINDOWS
#elif XBOX / WINDOWS_PHONE
#endif
*/