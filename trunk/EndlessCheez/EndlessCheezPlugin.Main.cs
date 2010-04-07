using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MediaPortal.UserInterface.Controls;
using MediaPortal.GUI;
using MediaPortal.GUI.Library;
using MediaPortal.Util;
using MediaPortal.Dialogs;
using MediaPortal.Configuration;
using CheezburgerAPI;
using System.Threading;

namespace EndlessCheez {

    [PluginIcons("EndlessCheez.img.EndlessCheez_enabled.png", "EndlessCheez.img.EndlessCheez_disabled.png")]
    public partial class EndlessCheezPlugin : ISetupForm, IShowPlugin, ICheezCollector {

        #region Enumerations

        internal enum PluginStates {
            DisplayCheezSites,
            DisplayLocalOnly,
            DisplayCurrentCheezSite,
            BrowseLatest,
            BrowseRandom,
            BrowseLocal,           
        }

        #endregion

        #region Private Members

        private static string _cheezRootFolder;
        private static int _fetchCount;
        private static PluginStates _defaultStartupState;

        #endregion

        #region Plugin Constructor / Initialization

        public EndlessCheezPlugin() {
            using (MediaPortal.Profile.Settings xmlReader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))) {
                _cheezRootFolder = xmlReader.GetValueAsString("EndlessCheez", "#EndlessCheez.CheezRootFolder", Config.GetFolder(Config.Dir.Thumbs) + @"\EndlessCheez\");
                _fetchCount = xmlReader.GetValueAsInt("EndlessCheez", "#EndlessCheez.FetchCount", 10);
                _defaultStartupState = (PluginStates)Enum.Parse(typeof(PluginStates), xmlReader.GetValueAsString("EndlessCheez", "#EndlessCheez.DefaultStartup", "DisplayCheezSites"));
            }            
        }

        private void InitCheezManager(int fetchCount, string cheezRootFolder, bool createRootFolderStructure) {
            if (!CheezManager.InitCheezManager(this, fetchCount, cheezRootFolder, createRootFolderStructure)) {
                ShowNotifyDialog(30, "Unable to initialize CheezManager - check internet connection!");
                _defaultStartupState = PluginStates.DisplayLocalOnly;
            }
        }

        ~EndlessCheezPlugin() {
            this.CancelCheezCollection();
        }

        #endregion

        #region ISetupForm Members

        // Returns the name of the plugin which is shown in the plugin menu
        public string PluginName() {
            return "EndlessCheez";
        }

        // Returns the description of the plugin is shown in the plugin menu
        public string Description() {
            return "TODO";
        }

        // Returns the author of the plugin which is shown in the plugin menu
        public string Author() {
            return "Anthrax";
        }

        // show the setup dialog
        public void ShowPlugin() {
            new EndlessCheezConfig().ShowDialog();
        }

        // Indicates whether plugin can be enabled/disabled
        public bool CanEnable() {
            return true;
        }

        // Get Windows-ID
        public int GetWindowId() {
            // WindowID of windowplugin belonging to this setup
            // enter your own unique code
            return GetWID;
        }

        // Indicates if plugin is enabled by default;
        public bool DefaultEnabled() {
            return true;
        }

        // indicates if a plugin has it's own setup screen
        public bool HasSetup() {
            return true;
        }

        /// <summary>
        /// If the plugin should have it's own button on the main menu of Mediaportal then it
        /// should return true to this method, otherwise if it should not be on home
        /// it should return false
        /// </summary>
        /// <param name="strButtonText">text the button should have</param>
        /// <param name="strButtonImage">image for the button, or empty for default</param>
        /// <param name="strButtonImageFocus">image for the button, or empty for default</param>
        /// <param name="strPictureImage">subpicture for the button or empty for none</param>
        /// <returns>true : plugin needs it's own button on home
        /// false : plugin does not need it's own button on home</returns>
        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage) {
            strButtonText = PluginName();
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = String.Empty;
            return true;
        }


        #endregion

        #region IShowPlugin Member

        public bool ShowDefaultHome() {
            return false;
        }

        public static int GetWID {
            get {
                return 300382;
            }
        }

        // With GetID it will be an window-plugin / otherwise a process-plugin
        // Enter the id number here again
        public override int GetID {
            get {
                return EndlessCheezPlugin.GetWID;
            }
            set {
            }
        }

        #endregion

        #region ICheezCollector Member

        public bool DeleteLocalCheez() {
            return CheezManager.DeleteLocalCheez();
        }

        public bool CheckCheezConnection() {
            return CheezManager.CheckCheezConnection();
        }

        public void CollectLatestCheez(CheezSite cheezSite) {
            ShowProgressInfo();
            Thread collectLatestCheez = new Thread(delegate() {
                CheezManager.CollectLatestCheez(cheezSite);
            });
            collectLatestCheez.Start();
        }

        public void CollectRandomCheez(CheezSite cheezSite) {
            ShowProgressInfo();
            Thread collectRandomCheez = new Thread(delegate() {
                CheezManager.CollectRandomCheez(cheezSite);
            });
            collectRandomCheez.Start();
        }

        public void CollectLocalCheez(CheezSite cheezSite) {
            ShowProgressInfo();
            Thread collectLocalCheez = new Thread(delegate() {
                CheezManager.CollectLocalCheez(cheezSite);
            });
            collectLocalCheez.Start();
        }

        public void CancelCheezCollection() {
            HideProgressInfo();
            CheezManager.CancelCheezCollection();
        }

        #endregion
    }
}

