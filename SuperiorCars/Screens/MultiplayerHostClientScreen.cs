using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperiorCars.Screens
{
    class MultiplayerHostClientScreen : MenuScreen
    {
        #region Fields

        private MenuEntry HostMenuEntry;
        private MenuEntry ClientMenuEntry;
        private MenuEntry LoadGameMenuEntry;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public MultiplayerHostClientScreen() : base("Multiplayer")
        {
            // Create our menu entries.
            HostMenuEntry = new MenuEntry("Host a game");
            ClientMenuEntry = new MenuEntry("Connect to a game");

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            HostMenuEntry.Selected += HostMenuEntrySelected;
            ClientMenuEntry.Selected += ClientMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(HostMenuEntry);
            MenuEntries.Add(ClientMenuEntry);
            MenuEntries.Add(back);

        }
        
        #endregion

        #region Handle Input
        /// <summary>
        /// Event handler for when the Host menu entry is selected.
        /// </summary>
        void HostMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen());
        }

        //TODO Fixa multiplayerscreen med inbjudningar och grej, tänk civ4
        /// <summary>
        /// Event handler for when the Client menu entry is selected.
        /// </summary>
        void ClientMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen());
        }

        #endregion
    }
}
