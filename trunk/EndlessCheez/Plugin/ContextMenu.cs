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

namespace EndlessCheez.Plugin {

    internal static class ContextMenu {

        private static readonly List<ContextMenuItem> ContextMenuItems = CreateContextmenuItems();

        internal enum ContextMenuButtons {
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
                                            "show Cheezsites Overview"));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseLatestCheez,
                                            "Browse Latest Online Cheez.."));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseRandomCheez,
                                            "Browse Random Online Cheez.."));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseLocalCheez,
                                            "Browse locally available Cheez.."));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseMore,
                                            "Gimme more of this Cheez.."));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnShowSlideShowCurrent,
                                           "Start Slideshow (current items)"));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnShowSlideShowAllLocal,
                                           "Start Slideshow (all local items)"));
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnCancelAllDownloads,
                                           "Cancel Cheez Download(s)!"));
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnDeleteLocalCheez,
                                          "Delete all local Cheez!"));
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnSortAsc,
                                         "Sort by Cheez creation date/time (Asc)"));
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnSortDesc,
                                         "Sort by Cheez creation date/time (Desc)"));
            return tmpList;
        }

        internal static ContextMenuButtons GetCurrentContextMenu() {
            IDialogbox contextMenu = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (contextMenu == null) {
                return ContextMenuButtons.NothingSelected;
            }
            contextMenu.Reset();
            contextMenu.SetHeading("EndlessCheez Menu");
            foreach (ContextMenuItem menuItem in ContextMenuItems) {
                contextMenu.Add(menuItem);
            }
            contextMenu.DoModal(GUIWindowManager.ActiveWindow);
            return (ContextMenuButtons)contextMenu.SelectedId;
        }



        private class ContextMenuItem : GUIListItem {

            public ContextMenuItem(ContextMenuButtons itemId, string itemLabel)
                : base(itemLabel) {
                base.ItemId = (int)itemId;
            }
        }
    }
}
