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

    internal static class Dialogs {

        #region ContextMenu

        private static readonly List<ContextMenuItem> ContextMenuItems = CreateContextmenuItems();

        internal enum ContextMenuButtons {
            BtnSwitchLayout,
            BtnCheezSitesOverview,  
            BtnBrowseMore,
            BtnSortAsc,
            BtnSortDesc,
            BtnShowSlideShowCurrent = 503,
            BtnShowSlideShowAllLocal = 504,
            BtnDeleteLocalCheez = 505,
            BtnBrowseLatestCheez = 506,
            BtnBrowseLocalCheez = 507,
            BtnBrowseRandomCheez,
            BtnCancelAllDownloads,
            NothingSelected
        }

        private static List<ContextMenuItem> CreateContextmenuItems() {
            List<ContextMenuItem> tmpList = new List<ContextMenuItem>();
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnSwitchLayout,
                                                        "Switch Layout"));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnCheezSitesOverview,
                                            "show Cheezsites Overview"));

            //tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseLatestCheez,
            //                                "Browse Latest Online Cheez.."));

            //tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseRandomCheez,
            //                                "Browse Random Online Cheez.."));

            //tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseLocalCheez,
            //                                "Browse locally available Cheez.."));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnBrowseMore,
                                            "Gimme more of this Cheez.."));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnShowSlideShowCurrent,
                                           "Start Slideshow (current items)"));

            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnShowSlideShowAllLocal,
                                           "Start Slideshow (all local items)"));
            //tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnCancelAllDownloads,
            //                               "Cancel Cheez Download(s)!"));
            tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnDeleteLocalCheez,
                                          "Delete all local Cheez!"));
            //tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnSortAsc,
            //                             "Sort by Cheez creation date/time (Asc)"));
            //tmpList.Add(new ContextMenuItem(ContextMenuButtons.BtnSortDesc,
            //                             "Sort by Cheez creation date/time (Desc)"));
            return tmpList;
        }

        internal static ContextMenuButtons ShowContextMenu() {
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

        #endregion

        #region GUI Helper Methods

        private static GUIDialogNotify dialogMailNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
        public static void ShowNotifyDialog(int timeOut, string notifyMessage) {
            try {
                dialogMailNotify.Reset();
                dialogMailNotify.TimeOut = timeOut;
                dialogMailNotify.SetImage(GUIGraphicsContext.Skin + @"\Media\hover_EndlessCheez.png");
                dialogMailNotify.SetHeading("EndlessCheez");
                dialogMailNotify.SetText(notifyMessage);
                dialogMailNotify.DoModal(GUIWindowManager.ActiveWindow);
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// Displays a yes/no dialog with custom labels for the buttons
        /// This method may become obsolete in the future if media portal adds more dialogs
        /// </summary>
        /// <returns>True if yes was clicked, False if no was clicked</returns>
        /// This has been taken (stolen really) from the wonderful MovingPictures Plugin -Anthrax.
        public static bool ShowCustomYesNo(string heading, string lines, string yesLabel, string noLabel, bool defaultYes) {
            GUIDialogYesNo dialog = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            try {
                dialog.Reset();
                dialog.SetHeading(heading);
                string[] linesArray = lines.Split(new string[] { "\\n" }, StringSplitOptions.None);
                if (linesArray.Length > 0)
                    dialog.SetLine(1, linesArray[0]);
                if (linesArray.Length > 1)
                    dialog.SetLine(2, linesArray[1]);
                if (linesArray.Length > 2)
                    dialog.SetLine(3, linesArray[2]);
                if (linesArray.Length > 3)
                    dialog.SetLine(4, linesArray[3]);
                dialog.SetDefaultToYes(defaultYes);

                foreach (var item in dialog.Children) {
                    if (item is GUIButtonControl) {
                        GUIButtonControl btn = (GUIButtonControl)item;
                        if (btn.GetID == 11 && !String.IsNullOrEmpty(yesLabel)) // Yes button
                            btn.Label = yesLabel;
                        else if (btn.GetID == 10 && !String.IsNullOrEmpty(noLabel)) // No button
                            btn.Label = noLabel;
                    }
                }
                dialog.DoModal(GUIWindowManager.ActiveWindow);
                return dialog.IsConfirmed;
            } finally {
                // set the standard yes/no dialog back to it's original state (yes/no buttons)
                if (dialog != null) {
                    dialog.ClearAll();
                }
            }
        }

        private static GUIDialogProgress DialogProgress = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
        public static void ShowProgressDialog(string headerTitle) {
            DialogProgress.Reset();
            DialogProgress.SetHeading(headerTitle);
            DialogProgress.DisableCancel(true);
            DialogProgress.SetLine(1, "Currently Downloading:");
            DialogProgress.Percentage = 0;
            DialogProgress.DisplayProgressBar = true;
            DialogProgress.ShowWaitCursor = true;
            DialogProgress.StartModal(GUIWindowManager.ActiveWindow);
            DialogProgress.Progress();
        }

        public static void UpdateProgressDialog(string currentItem, int progressPercentage) {           
            DialogProgress.SetPercentage(progressPercentage);
            DialogProgress.SetLine(2, currentItem);
            DialogProgress.Progress();
        }

        public static void HideProgressDialog() {
            DialogProgress.ShowWaitCursor = false;
            DialogProgress.Close();
        }

        #endregion

    }
}
