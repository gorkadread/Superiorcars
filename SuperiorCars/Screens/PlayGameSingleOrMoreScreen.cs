namespace SuperiorCars.Screens
{
    class PlayGameSingleOrMoreScreen : MenuScreen
    {
        #region Fields

        private MenuEntry SinglePlayerMenuEntry;
        private MenuEntry MultiPlayerMenuEntry;
        private MenuEntry LoadGameMenuEntry;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public PlayGameSingleOrMoreScreen() : base("Play Game")
        {
            // Create our menu entries.
            SinglePlayerMenuEntry = new MenuEntry("Single Player");
            MultiPlayerMenuEntry = new MenuEntry("Multiplayer");
            LoadGameMenuEntry = new MenuEntry("Load game");

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            SinglePlayerMenuEntry.Selected += SinglePlayerMenuEntrySelected;
            MultiPlayerMenuEntry.Selected += MultiPlayerMenuEntrySelected;
            LoadGameMenuEntry.Selected += LoadGameMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(SinglePlayerMenuEntry);
            MenuEntries.Add(MultiPlayerMenuEntry);
            MenuEntries.Add(LoadGameMenuEntry);
            MenuEntries.Add(back);

        }
        
        #endregion

        #region Handle Input
        /// <summary>
        /// Event handler for when the Single Player menu entry is selected.
        /// </summary>
        void SinglePlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen());
        }
        
        //TODO Fixa multiplayerscreen med inbjudningar och grej, tänk civ4
        /// <summary>
        /// Event handler for when the Multiplayer menu entry is selected.
        /// </summary>
        void MultiPlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new MultiplayerHostClientScreen(), e.PlayerIndex);
        }

        //TODO Fixa laddscreen
        /// <summary>
        /// Event handler for when the Load Game menu entry is selected.
        /// </summary>
        void LoadGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen());
        }

        #endregion
    }
}