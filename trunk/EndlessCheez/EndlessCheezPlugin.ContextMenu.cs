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
using System.Collections;

namespace EndlessCheez {

    public partial class EndlessCheezPlugin {

        private static readonly List<ContextMenuItem> ContextMenuItems = CreateContextmenuItems();

        private enum ContextMenuButtons {
            BtnCheezSitesOverview,
            BtnBrowseLatestCheez,
            BtnBrowseRandomCheez,
            BtnBrowseLocalCheez,
            BtnBrowseMore,
            BtnShowSlideShowCurrent,
            BtnShowSlideShowAllLocal,
            BtnCancelAllDownloads,
            BtnDeleteLocalCheez
        }

        private static List<ContextMenuItem> CreateContextmenuItems() {
            List<ContextMenuItem> tmpList = new List<ContextMenuItem>();
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnCheezSitesOverview,
                                            "Cheezsites Overview",
                                            new List<PluginStates>(new PluginStates[] { 
                                                    PluginStates.DisplayCheezSites,
                                                    PluginStates.DisplayCurrentCheezSite,
                                                    PluginStates.BrowseLatest, 
                                                    PluginStates.BrowseLocal, 
                                                    PluginStates.BrowseRandom, 
                                                    PluginStates.DisplayLocalOnly
                                            })));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseLatestCheez,
                                            "Browse Latest Online Cheez..",
                                            new List<PluginStates>(new PluginStates[] { 
                                                    PluginStates.DisplayCurrentCheezSite,                                                                                 
                                                    PluginStates.BrowseLocal, 
                                                    PluginStates.BrowseRandom
                                            })));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseRandomCheez,
                                            "Browse Random Online Cheez..",
                                            new List<PluginStates>(new PluginStates[] { 
                                                    PluginStates.DisplayCurrentCheezSite,
                                                    PluginStates.BrowseLatest, 
                                                    PluginStates.BrowseLocal
                                            })));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseLocalCheez,
                                            "Browse locally available Cheez..",
                                            new List<PluginStates>(new PluginStates[] { 
                                                    PluginStates.DisplayCurrentCheezSite,
                                                    PluginStates.BrowseLatest,                                                                                 
                                                    PluginStates.BrowseRandom
                                            })));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseMore,
                                            "Gimme more of this Cheez..",
                                            new List<PluginStates>(new PluginStates[] { 
                                                    PluginStates.BrowseLatest, 
                                                    PluginStates.BrowseRandom
                                            })));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnShowSlideShowCurrent,
                                           "Start Slideshow (current items)",
                                           new List<PluginStates>(new PluginStates[] { 
                                                    PluginStates.DisplayCurrentCheezSite,
                                                    PluginStates.BrowseLatest, 
                                                    PluginStates.BrowseLocal, 
                                                    PluginStates.BrowseRandom, 
                                                    PluginStates.DisplayLocalOnly
                                           })));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnShowSlideShowAllLocal,
                                           "Start Slideshow (all local items)",
                                           new List<PluginStates>(new PluginStates[] { 
                                                    PluginStates.DisplayCurrentCheezSite,
                                                    PluginStates.BrowseLatest, 
                                                    PluginStates.BrowseLocal, 
                                                    PluginStates.BrowseRandom, 
                                                    PluginStates.DisplayLocalOnly
                                           })));
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnCancelAllDownloads,
                                           "Cancel Cheez Download(s)!",
                                           null));
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnDeleteLocalCheez,
                                          "Delete all local Cheez!",
                                          null));
            return tmpList;
        }

        private static List<GUIListItem> GetCurrentContextMenu(PluginStates pluginState) {
            return (List<GUIListItem>)ContextMenuItems.Where(item => item.GetVisibility(pluginState));
        }



        private class ContextMenuItem : GUIListItem {
            private List<PluginStates> _pluginStateVisibility;

            public ContextMenuItem(ContextMenuButtons itemId, string itemLabel, List<PluginStates> visibleStates)
                : base(itemLabel) {
                base.ItemId = (int)itemId;
                this._pluginStateVisibility = visibleStates;
            }

            public bool GetVisibility(PluginStates pluginState) {
                return (this._pluginStateVisibility != null) ? this._pluginStateVisibility.Contains(pluginState) : true;
            }
        }
    }
}
