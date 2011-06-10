using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CheezburgerAPI;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Pictures;
using WindowPlugins;
using System.Linq;
using MediaPortal.Util;

namespace EndlessCheez.Plugin {
    public partial class Main : WindowPluginBase {

        public Main() {
            GetID = Settings.PLUGIN_WINDOW_ID;
        }

        #region Private Members

        private Dictionary<CheezSite, List<CheezListItem>> CheezHistory { get; set; }
        private List<CheezListItem> CheezSites { get; set; }
        private PluginStates PluginState { get; set; }

        #endregion

        #region Enums

        private enum PluginStates {
            CheezSiteOverview,
            CheezSiteSelected,
            SlideShowRunning
        }
        #endregion


        #region Skin Controls

        [SkinControlAttribute(600)]
        protected GUIImage ctrlBackgroundImage = null;

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
            GUIPropertyManager.SetProperty("#EndlessCheez.CurrentItem", " ");
            SlideShow = (GUISlideShow)GUIWindowManager.GetWindow((int)Window.WINDOW_SLIDESHOW);
            return Load(GUIGraphicsContext.Skin + @"\EndlessCheez.xml");
        }

        public override void DeInit() {
            this.CancelCheezCollection();
            if (Settings.DeleteLocalCheezOnExit) {
                DeleteLocalCheez();
            }
            base.DeInit();
        }

        protected override void LoadSettings() {
            Settings.Load();
        }

        protected override void OnPageLoad() {
            GUIWaitCursor.Init();
            GUIWaitCursor.Show();
            InitCheezManager(Settings.FetchCount, Settings.CheezRootFolder, true);
            CheezHistory = new Dictionary<CheezSite, List<CheezListItem>>();
            CheezSites = new List<CheezListItem>();
            foreach (CheezSite site in CheezManager.CheezSites) {
                CheezListItem tmpItem = new CheezListItem(site);
                if (String.IsNullOrEmpty(tmpItem.IconImageBig)) {
                    Utils.SetDefaultIcons(tmpItem);
                }
                CheezSites.Add(tmpItem);
                CheezHistory.Add(site, new List<CheezListItem>());
            }
            CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
            PluginState = PluginStates.CheezSiteOverview;
            SwitchLayout();
            GUIWaitCursor.Hide();
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

        public override void OnAction(MediaPortal.GUI.Library.Action action) {
            if (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) {
                if (SlideShow.InSlideShow) {
                    GUIWindowManager.ActivateWindow(GetID);
                } else {
                    switch (PluginState) {
                        case PluginStates.CheezSiteSelected:
                        case PluginStates.SlideShowRunning:
                        default:
                            DisplayCheezSitesOverview();
                            return;
                        case PluginStates.CheezSiteOverview:
                            break;
                    }
                }
            }
            base.OnAction(action);
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            switch (controlId) {
                case (int)Dialogs.ContextMenuButtons.BtnShowSlideShowAllLocal:
                    OnSlideShowAllLocal();
                    break;
                case (int)Dialogs.ContextMenuButtons.BtnShowSlideShowCurrent:                  
                    OnSlideShowCurrent();
                    break;
                default:
                    base.OnClicked(controlId, control, actionType);
                    break;
            }           
        }

        protected override void OnClick(int iItem) {
            GUIGraphicsContext.IsFullScreenVideo = true;
            GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO);
            if (facadeLayout[iItem] is CheezListItem) {
                if (facadeLayout[iItem].IsFolder) {
                    DisplayCurrentCheezSite(CheezManager.GetCheezSiteByPath(facadeLayout[iItem].Path));
                } else {
                    if (facadeLayout[iItem].IsRemote) {
                        Dialogs.ShowNotifyDialog(10, "I'd love to have this feature myself!\nBut sadly this requires grabbing the direct video links from \n" + facadeLayout[iItem].DVDLabel + "\nI'll promise to look into this issue asap! ;-)");
                        MediaPortal.Player.g_Player.PlayVideoStream(facadeLayout[iItem].DVDLabel, facadeLayout[iItem].ToString());
                    } else {
                        OnSlideShowCurrent(facadeLayout[iItem].IconImageBig);
                    }
                }
            }

            base.OnClick(iItem);
        }

        protected override void OnShowContextMenu() {
            switch (Dialogs.ShowContextMenu()) {
                case Dialogs.ContextMenuButtons.BtnCheezSitesOverview:
                    DisplayCheezSitesOverview();
                    break;
                case Dialogs.ContextMenuButtons.BtnSwitchLayout:
                    OnShowLayouts();
                    break;
                case Dialogs.ContextMenuButtons.BtnBrowseLatestCheez:

                    break;
                case Dialogs.ContextMenuButtons.BtnBrowseLocalCheez:

                    break;
                case Dialogs.ContextMenuButtons.BtnBrowseRandomCheez:

                    break;
                case Dialogs.ContextMenuButtons.BtnBrowseMore:

                    break;
                case Dialogs.ContextMenuButtons.BtnSortAsc:
                    if (facadeLayout != null) {
                        facadeLayout.Sort(new CheezComparerDateAsc());
                        this.Process();
                    }
                    break;
                case Dialogs.ContextMenuButtons.BtnSortDesc:
                    if (facadeLayout != null) {
                        facadeLayout.Sort(new CheezComparerDateDesc());
                        this.Process();
                    }
                    break;
                case Dialogs.ContextMenuButtons.BtnShowSlideShowAllLocal:
                    OnSlideShowAllLocal();
                    break;
                case Dialogs.ContextMenuButtons.BtnShowSlideShowCurrent:
                    OnSlideShowCurrent();
                    break;
                case Dialogs.ContextMenuButtons.BtnCancelAllDownloads:
                    CancelCheezCollection();
                    break;
                case Dialogs.ContextMenuButtons.BtnDeleteLocalCheez:
                    DeleteLocalCheez();
                    break;
                case Dialogs.ContextMenuButtons.NothingSelected:
                default:
                    //throw new ArgumentOutOfRangeException();
                    return;
            }
        }

        #endregion

        #region Private Methods

        private void InitCheezManager(int fetchCount, string cheezRootFolder, bool createRootFolderStructure) {
            if (!CheezManager.InitCheezManager(this, fetchCount, cheezRootFolder, createRootFolderStructure)) {
                Dialogs.ShowNotifyDialog(30, "Unable to initialize CheezManager - check internet connection!");
            }
        }

        private void DisplayCheezSitesOverview() {
            PluginState = PluginStates.CheezSiteOverview;
            facadeLayout.Clear();
            CheezSites.ForEach(siteItem => facadeLayout.Add(siteItem));
            facadeLayout.DoUpdate();
        }

        private void DisplayCurrentCheezSite(CheezSite selectedCheezSite) {
            PluginState = PluginStates.CheezSiteSelected;
            facadeLayout.Clear();
            CheezHistory[selectedCheezSite].ForEach(item => facadeLayout.Add(item));
            if (facadeLayout.Count < 1) {
                CollectLatestCheez(selectedCheezSite);
            } else {
                facadeLayout.SelectedItem = CheezSites.First(x => x.Path == selectedCheezSite.SiteId).LastSelectedIndex;
            }
            facadeLayout.DoUpdate();
        }

        private void ProcessAndDisplayNewCheez(CheezSite sourceSite, List<CheezItem> cheezItems) {
            Dialogs.HideProgressDialog();
            foreach (CheezItem cheezItem in cheezItems) {
                CheezListItem tmpItem = new CheezListItem(cheezItem);
                Utils.SetDefaultIcons(tmpItem);
                tmpItem.OnItemSelected += new GUIListItem.ItemSelectedHandler(OnItemSelected);
                if (SlideShow != null && SlideShow.InSlideShow) {
                    SlideShow.Add(cheezItem.CheezImagePath);
                }
                if (PluginState == PluginStates.CheezSiteSelected) {
                    facadeLayout.Add(tmpItem);
                }
                CheezHistory[sourceSite].Add(tmpItem);
            }
            facadeLayout.DoUpdate();
        }

        #endregion

        #region Plugin Event Handlers

        private void OnItemSelected(GUIListItem item, GUIControl parent) {
            if (PluginState == PluginStates.CheezSiteSelected) {
                if (parent.GetID == facadeLayout.GetID) {
                    CheezSites.First(x => x.Path == CheezManager.CurrentCheezSite.SiteId).LastSelectedIndex = facadeLayout.SelectedListItemIndex;
                    if (!CheezManager.IsBusy) {
                        if (facadeLayout.SelectedListItemIndex == facadeLayout.Count - 1) {
                            CollectLatestCheez(CheezManager.CurrentCheezSite);
                        }
                    }
                }
            }
        }

        private void OnDeleteLocalCheez() {
            if (Dialogs.ShowCustomYesNo("Delete all local Cheez?", "Are you sure to delete all of the " + CheezManager.LocalCheezCount + " local files?", "Yes!", "Cancel", false)) {
                if (DeleteLocalCheez()) {
                    Dialogs.ShowNotifyDialog(10, "Local Cheez successfully deleted!");
                } else {
                    Dialogs.ShowNotifyDialog(10, "Unable to delete all local files! (" + CheezManager.LocalCheezCount + " left)");
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
            if (Dialogs.ShowCustomYesNo("Shuffle Slideshow?", "", "Yes", "No", true)) {
                OnSlideShowCurrent(true);
            } else {
                OnSlideShowCurrent(false);
            }

        }
        #endregion

        #endregion



        /// <summary>Implements ascending sort algorithm</summary>
        class CheezComparerDateAsc : IComparer<GUIListItem> {
            #region IComparer<GUIListItem> Member

            public int Compare(GUIListItem x, GUIListItem y) {
                return DateTime.Compare(x.FileInfo.CreationTime, y.FileInfo.CreationTime);
            }

            #endregion
        }

        class CheezComparerDateDesc : IComparer<GUIListItem> {
            #region IComparer<GUIListItem> Member

            public int Compare(GUIListItem x, GUIListItem y) {
                return DateTime.Compare(y.FileInfo.CreationTime, x.FileInfo.CreationTime);
            }

            #endregion
        }
    }
}
