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

namespace EndlessCheez.Plugin {

    [PluginIcons("EndlessCheez.Resources.img.EndlessCheez_enabled.png", "EndlessCheez.Resources.img.EndlessCheez_disabled.png")]
    public class Configuration : ISetupForm {
                         
        #region Plugin Constructor / Initialization

        public Configuration() {
            Log.Debug(Settings.PLUGIN_NAME + " started!");        
        }

        ~Configuration() {

        }
        #endregion

        #region ISetupForm Members

        // Returns the name of the plugin which is shown in the plugin menu
        public string PluginName() {
            return Settings.PLUGIN_NAME;
        }

        // Returns the description of the plugin is shown in the plugin menu
        public string Description() {
            return Settings.PLUGIN_DESCRIPTION;
        }

        // Returns the author of the plugin which is shown in the plugin menu
        public string Author() {
            return Settings.PLUGIN_AUTHOR;
        }

        // show the setup dialog
        public void ShowPlugin() {
            new ConfigurationForm().ShowDialog();
        }

        // Indicates whether plugin can be enabled/disabled
        public bool CanEnable() {
            return true;
        }

        // Get Windows-ID
        public int GetWindowId() {
            // WindowID of windowplugin belonging to this setup
            // enter your own unique code
            return Settings.PLUGIN_WINDOW_ID;
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

    
    }
}

