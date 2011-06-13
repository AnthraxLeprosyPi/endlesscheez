using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MediaPortal.GUI.Library;

namespace EndlessCheez.Plugin {
    public static class Settings {

        public const string PLUGIN_NAME = "EndlessCheez";
        public const string PLUGIN_AUTHOR = "Anthrax";
        public const string PLUGIN_DESCRIPTION = "A MediaPortal plugin that allows you to view Cheezburger™ content.\nNeither MediaPortal nor the author of this plugin is affiliated with the Cheezburger Network.";
        public const int PLUGIN_WINDOW_ID = 300382;
        public static string PLUGIN_MEDIA_HOVER = @"hover_EndlessCheez.png";

        static Settings() {
            //Set defaults
            FetchCount = 10;
            CheezRootFolder = Path.Combine(MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Thumbs), PLUGIN_NAME);
            DeleteLocalCheezOnExit = false;
        }

        public static int FetchCount { get; set; }
        public static string CheezRootFolder { get; set; }
        public static bool DeleteLocalCheezOnExit { get; set; }

        /// <summary>
        /// Load the settings from the mediaportal config
        /// </summary>
        public static void Load() {
            using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml"))) {
                if (!String.IsNullOrEmpty(reader.GetValue(PLUGIN_NAME, "CheezRootFolder"))) {
                    CheezRootFolder = reader.GetValue(PLUGIN_NAME, "CheezRootFolder");
                }                
                FetchCount = reader.GetValueAsInt(PLUGIN_NAME, "FetchCount", FetchCount);             
                DeleteLocalCheezOnExit = reader.GetValueAsBool(PLUGIN_NAME, "DeleteLocalCheezOnExit", DeleteLocalCheezOnExit);                
            }
        }

        /// <summary>
        /// Save the settings to the MP config
        /// </summary>
        public static void Save() {
            using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml"))) {
                xmlwriter.SetValue(PLUGIN_NAME, "CheezRootFolder", CheezRootFolder);
                xmlwriter.SetValue(PLUGIN_NAME, "FetchCount", (int)FetchCount);
                xmlwriter.SetValueAsBool(PLUGIN_NAME, "DeleteLocalCheezOnExit", DeleteLocalCheezOnExit);
            }
        }
    }
}
