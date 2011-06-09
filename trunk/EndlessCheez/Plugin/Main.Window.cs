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
        
        public Main() {
            GetID = Settings.PLUGIN_WINDOW_ID;
        }

        #region Private Members

        private BackListItem BackItem { get; set; }
        private Dictionary<CheezSite, List<CheezListItem>> CheezHistory { get; set; }
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
            InitCheezManager(Settings.FetchCount, Settings.CheezRootFolder, true);
            BackItem = new BackListItem();
            CheezHistory = new Dictionary<CheezSite, List<CheezListItem>>();
            CheezManager.CheezSites.ForEach(site => CheezHistory.Add(site, new List<CheezListItem>()));
            CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
            SwitchLayout();
            GUIPropertyManager.SetProperty("#EndlessCheez.CurrentItem", " ");
            SlideShow = (GUISlideShow)GUIWindowManager.GetWindow((int)Window.WINDOW_SLIDESHOW);
            PluginState = PluginStates.CheezSiteOverview;
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
                        case PluginStates.CheezSiteOverview:
                            base.OnAction(action);
                            break;
                        case PluginStates.CheezSiteSelected:
                        case PluginStates.SlideShowRunning:                            
                        default:
                            DisplayCheezSitesOverview();
                            break;
                    }                
                }
            }           
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

        #region Private Methods


        private void InitCheezManager(int fetchCount, string cheezRootFolder, bool createRootFolderStructure) {
            if (!CheezManager.InitCheezManager(this, fetchCount, cheezRootFolder, createRootFolderStructure)) {
                Dialogs.ShowNotifyDialog(30, "Unable to initialize CheezManager - check internet connection!");
            }
        }

        private void DisplayCheezSitesOverview() {
            PluginState = PluginStates.CheezSiteOverview;
            facadeLayout.Clear();
            CheezManager.CheezSites.ForEach(site => facadeLayout.Add(new CheezListItem(site)));
            facadeLayout.DoUpdate();
        }

        private void DisplayCurrentCheezSite(CheezSite selectedCheezSite) {
            PluginState = PluginStates.CheezSiteSelected;
            facadeLayout.Clear();
            facadeLayout.Add(BackItem);
            CheezHistory[selectedCheezSite].ForEach(item => facadeLayout.Add(item));
            CollectLatestCheez(selectedCheezSite);
        }


        private void ProcessAndDisplayNewCheez(CheezSite sourceSite, List<CheezItem> cheezItems) {
            Dialogs.HideProgressDialog();            
            foreach (CheezItem cheezItem in cheezItems) {
                CheezListItem tmpItem = new CheezListItem(cheezItem);
                tmpItem.OnItemSelected += new GUIListItem.ItemSelectedHandler(OnItemSelected);
                if (PluginState == PluginStates.CheezSiteSelected) {
                    if (SlideShow != null && SlideShow.InSlideShow) {
                        SlideShow.Add(cheezItem.CheezImagePath);
                    }
                    facadeLayout.Add(tmpItem);
                }
                CheezHistory[sourceSite].Add(tmpItem);
            }                   
            facadeLayout.DoUpdate();
        }

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
