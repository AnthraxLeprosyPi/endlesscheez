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
            BtnSortAsc,
            BtnSortDesc,
            BtnShowSlideShowCurrent,
            BtnShowSlideShowAllLocal,
            BtnCancelAllDownloads,
            BtnDeleteLocalCheez,
            NothingSelected
        }

        private static List<ContextMenuItem> CreateContextmenuItems() {
            List<ContextMenuItem> tmpList = new List<ContextMenuItem>();
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnCheezSitesOverview,
                                            "Cheezsites Overview",
                                            new List<PluginStates>(new PluginStates[] { 
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
                                           }), CheezItemsAvailable()));

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
                                           null, CheezManager.IsBusy));
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnDeleteLocalCheez,
                                          "Delete all local Cheez!",
                                          null));
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnSortAsc,
                                         "Sort by Cheez creation date/time (Asc)",
                                         null, CheezItemsAvailable()));
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnSortDesc,
                                         "Sort by Cheez creation date/time (Desc)",
                                         null, CheezItemsAvailable()));
            return tmpList;
        }

        private static ContextMenuButtons GetCurrentContextMenu(PluginStates pluginState) {
            IDialogbox contextMenu = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if(contextMenu == null) {
                return ContextMenuButtons.NothingSelected;
            }
            contextMenu.Reset();
            contextMenu.SetHeading("EndlessCheez Menu");
            foreach(GUIListItem menuItem in (List<GUIListItem>)ContextMenuItems.Where(item => item.GetVisibility(pluginState))) {
                contextMenu.Add(menuItem);
            }
            contextMenu.DoModal(GUIWindowManager.ActiveWindow);
            return (ContextMenuButtons)contextMenu.SelectedId;
        }



        private class ContextMenuItem : GUIListItem{
            private List<PluginStates> _pluginStateVisibility;
            private bool _advancedVisibility = true;
            
            public ContextMenuItem(ContextMenuButtons itemId, string itemLabel, List<PluginStates> visibleStates)
                : base(itemLabel) {
                base.ItemId = (int)itemId;
                this._pluginStateVisibility = visibleStates;
            }

            public ContextMenuItem(ContextMenuButtons itemId, string itemLabel, List<PluginStates> visibleStates, bool advancedVisibility) : this(itemId,itemLabel,visibleStates){
                this._advancedVisibility = advancedVisibility;
            }

            public bool GetVisibility(PluginStates pluginState) {
                return (this._pluginStateVisibility != null) ? this._pluginStateVisibility.Contains(pluginState) || this._advancedVisibility : this._advancedVisibility;
            }
        }
       
    }
}
