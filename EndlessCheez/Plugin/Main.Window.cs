using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CheezburgerAPI;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Pictures;
using WindowPlugins;

namespace EndlessCheez.Plugin {
    public partial class Main : WindowPluginBase {

        #region Private Members

        private BackListItem BackItem { get; set; }

        #endregion

        #region Skin Controls

        [SkinControlAttribute(600)]
        protected GUIImage ctrlBackgroundImage = null;

        [SkinControlAttribute(402)]
        protected GUIProgressControl ctrlProgressBar = null;

        [SkinControlAttribute(403)]
        protected GUILabelControl lblProgressLabel = null;

        private static GUISlideShow SlideShow;

        #endregion

        #region GUIWindow Base Class Overrides

        protected override string SerializeName {
            get {
                return Settings.PLUGIN_NAME;
            }
        }

        public override bool Init() {
            LoadSettings();
            BackItem = new BackListItem();
            CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
            GUIPropertyManager.SetProperty("#EndlessCheez.CurrentItem", " ");
            SlideShow = (GUISlideShow)GUIWindowManager.GetWindow((int)Window.WINDOW_SLIDESHOW);
            return Load(GUIGraphicsContext.Skin + @"\EndlessCheez.xml");
        }

        public override void DeInit() {
            this.CancelCheezCollection();
            base.DeInit();
        }

        protected override void LoadSettings() {
            Settings.Load();
        }

        protected override void OnPageLoad() {
            InitCheezManager(Settings.FetchCount, Settings.CheezRootFolder, true);
            if (ctrlBackgroundImage != null) {
                ctrlBackgroundImage.ImagePath = GUIGraphicsContext.Skin + @"\Media\EndlessCheez\Background.png";
            }
            if (facadeLayout != null) {
                DisplayCheezSitesOverview();
            }
            base.OnPageLoad();
        }

        protected override void OnPageDestroy(int new_windowId) {
            CancelCheezCollection();
            base.OnPageDestroy(new_windowId);
        }

        protected override void OnShowViews() {

        }

        protected override bool AllowLayout(GUIFacadeControl.Layout layout) {
            switch (layout) {
                case GUIFacadeControl.Layout.CoverFlow:
                case GUIFacadeControl.Layout.Filmstrip:
                case GUIFacadeControl.Layout.LargeIcons:
                case GUIFacadeControl.Layout.List:
                case GUIFacadeControl.Layout.SmallIcons:
                    return true;
                default:
                    return false;
            }
        }

        protected override void OnInfo(int iItem) {
            base.OnInfo(iItem);
        }

        protected override void OnClick(int iItem) {
            if (facadeLayout[iItem] is BackListItem) {
                DisplayCheezSitesOverview();
            } else if (facadeLayout[iItem] is CheezListItem) {
                if (facadeLayout[iItem].IsFolder) {
                    DisplayCurrentCheezSite(CheezManager.GetCheezSiteByPath(facadeLayout[iItem].Path));
                } else {
                    if (facadeLayout[iItem].IsRemote) {
                        MediaPortal.Player.g_Player.PlayVideoStream(facadeLayout[iItem].DVDLabel, facadeLayout[iItem].ToString());
                    } else {
                        OnSlideShowCurrent(facadeLayout[iItem].IconImageBig);
                    }
                }
            }
            base.OnClick(iItem);
        }

        protected override void OnShowContextMenu() {
            switch (ContextMenu.GetCurrentContextMenu()) {
                case ContextMenu.ContextMenuButtons.BtnCheezSitesOverview:
                    DisplayCheezSitesOverview();
                    break;
                case ContextMenu.ContextMenuButtons.BtnBrowseLatestCheez:
                    
                    break;
                case ContextMenu.ContextMenuButtons.BtnBrowseLocalCheez:
                    
                    break;
                case ContextMenu.ContextMenuButtons.BtnBrowseRandomCheez:
                    
                    break;
                case ContextMenu.ContextMenuButtons.BtnBrowseMore:
                    
                    break;
                case ContextMenu.ContextMenuButtons.BtnSortAsc:
                    if (facadeLayout != null) {
                        facadeLayout.Sort(new CheezComparerDateAsc());
                        this.Process();
                    }
                    break;
                case ContextMenu.ContextMenuButtons.BtnSortDesc:
                    if (facadeLayout != null) {
                        facadeLayout.Sort(new CheezComparerDateDesc());
                        this.Process();
                    }
                    break;
                case ContextMenu.ContextMenuButtons.BtnShowSlideShowAllLocal:
                    OnSlideShowAllLocal();
                    break;
                case ContextMenu.ContextMenuButtons.BtnShowSlideShowCurrent:
                    OnSlideShowCurrent();
                    break;
                case ContextMenu.ContextMenuButtons.BtnCancelAllDownloads:
                    CancelCheezCollection();
                    break;
                case ContextMenu.ContextMenuButtons.BtnDeleteLocalCheez:
                    DeleteLocalCheez();
                    break;
                case ContextMenu.ContextMenuButtons.NothingSelected:
                default:
                    //throw new ArgumentOutOfRangeException();
                    return;
            }
        }

        #endregion

        #region Plugin Event Handlers

        private void OnItemSelected(GUIListItem item, GUIControl parent) {
            if (parent.GetID == facadeLayout.GetID) {
                if (!CheezManager.IsBusy) {
                    if (facadeLayout.SelectedListItemIndex == facadeLayout.Count - 1) {
                        CollectLatestCheez(null);
                    }
                }
            }
        }

        private void OnDeleteLocalCheez() {
            if (ShowCustomYesNo("Delete all local Cheez?", "Are you sure to delete all of the " + CheezManager.LocalCheezCount + " local files?", "Yes!", "Cancel", false)) {
                if (DeleteLocalCheez()) {
                    ShowNotifyDialog(10, "Local Cheez successfully deleted!");
                } else {
                    ShowNotifyDialog(10, "Unable to delete all local files! (" + CheezManager.LocalCheezCount + " left)");
                }
                DisplayCheezSitesOverview();
            }
        }

        #region SlideShow
        private void OnSlideShowCurrent() {
            OnSlideShowCurrent(String.Empty, false);
        }

        private void OnSlideShowCurrent(bool shuffle) {
            OnSlideShowCurrent(String.Empty, shuffle);
        }

        private void OnSlideShowCurrent(string startImage) {
            OnSlideShowCurrent(startImage, false);
        }

        private void OnSlideShowCurrent(string startImage, bool shuffle) {
            if (facadeLayout == null || facadeLayout.Count <= 0) {
                OnSlideShowAllLocal();
                return;
            }
            SlideShow.Reset();
            if (SlideShow != null) {
                for (int i = 0; i <= facadeLayout.Count; i++) {
                    if (facadeLayout[i] is CheezListItem && facadeLayout[i].HasIconBig && !facadeLayout[i].IsFolder) {
                        SlideShow.Add(facadeLayout[i].IconImageBig);
                    }
                }
                GUIWindowManager.ActivateWindow((int)Window.WINDOW_SLIDESHOW);
                SlideShow.StartSlideShow();
                if (shuffle) {
                    SlideShow.Shuffle();
                }
                if (!String.IsNullOrEmpty(startImage)) {
                    SlideShow.Select(startImage);
                }
            }

        }

        private void OnSlideShowAllLocal() {
            CollectLocalCheez(null);
            while (CheezManager.IsBusy) {
                Thread.Sleep(100);
            }
            if (ShowCustomYesNo("Shuffle Slideshow?", "", "Yes", "No", true)) {
                OnSlideShowCurrent(true);
            } else {
                OnSlideShowCurrent(false);
            }

        }
        #endregion

        #endregion

        #region Private Methods


        private void InitCheezManager(int fetchCount, string cheezRootFolder, bool createRootFolderStructure) {
            if (!CheezManager.InitCheezManager(this, fetchCount, cheezRootFolder, createRootFolderStructure)) {
                ShowNotifyDialog(30, "Unable to initialize CheezManager - check internet connection!");
            }
        }

        private void DisplayCheezSitesOverview() {
            facadeLayout.Clear();
            CheezManager.CheezSites.ForEach(site => facadeLayout.Add(new CheezListItem(site)));
            facadeLayout.DoUpdate();
        }

        private void DisplayCurrentCheezSite(CheezSite selectedCheezSite) {
            facadeLayout.Clear();
            facadeLayout.Add(BackItem);
            CollectLatestCheez(selectedCheezSite);
        }


        private void ProcessAndDisplayNewCheez(List<CheezItem> cheezItems) {
            HideProgressInfo();
            foreach (CheezItem cheezItem in cheezItems) {
                if (SlideShow != null && SlideShow.InSlideShow) {
                    SlideShow.Add(cheezItem.CheezImagePath);
                }
                CheezListItem tmpItem = new CheezListItem(cheezItem);
                tmpItem.OnItemSelected += new GUIListItem.ItemSelectedHandler(OnItemSelected);
                facadeLayout.Add(tmpItem);
            }
            facadeLayout.DoUpdate();
        }

        #endregion

        #region GUI Helper Methods

        private static void ShowNotifyDialog(int timeOut, string notifyMessage) {
            try {
                GUIDialogNotify dialogMailNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                dialogMailNotify.TimeOut = timeOut;
                dialogMailNotify.SetImage(GUIGraphicsContext.Skin + @"\Media\EndlessCheez_logo.png");
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
        public bool ShowCustomYesNo(string heading, string lines, string yesLabel, string noLabel, bool defaultYes) {
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
                dialog.DoModal(GetID);
                return dialog.IsConfirmed;
            } finally {
                // set the standard yes/no dialog back to it's original state (yes/no buttons)
                if (dialog != null) {
                    dialog.ClearAll();
                }
            }
        }

        private void ShowProgressInfo() {
            GUIWaitCursor.Show();
            GUIPropertyManager.SetProperty("#EndlessCheez.CurrentItem", " ");
            ctrlProgressBar.IsVisible = true;
            lblProgressLabel.IsVisible = true;
            ctrlProgressBar.Percentage = 0;
        }

        private void HideProgressInfo() {
            GUIWaitCursor.Hide();
            ctrlProgressBar.IsVisible = false;
            lblProgressLabel.IsVisible = false;
            ctrlProgressBar.Percentage = 100;
        }

        #endregion
    }
}
