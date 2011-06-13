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
using OnlineVideos.Sites;
using OnlineVideos.Hoster;
using System.Text.RegularExpressions;
using System.Net;
using OnlineVideos;

namespace EndlessCheez.Plugin {
    public partial class Main : WindowPluginBase {

        public Main() {
            GetID = Settings.PLUGIN_WINDOW_ID;
        }

        #region Private Members

        private Dictionary<CheezSite, List<CheezListItem>> CheezHistory { get; set; }
        private List<CheezListItem> CheezSites { get; set; }
        private PluginStates PluginState { get; set; }
        private bool IsOnlineMode { get; set; }
        private bool IsOnlineVideosInstalled { get; set; }

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
            IsOnlineVideosInstalled = File.Exists(Path.Combine(MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Plugins), @"Windows\OnlineVideos\OnlineVideos.dll"));
            InitCheezManager(Settings.FetchCount, Settings.CheezRootFolder, true);
            InitSiteHistory();
            PluginState = PluginStates.CheezSiteOverview;
            SlideShow = (GUISlideShow)GUIWindowManager.GetWindow((int)Window.WINDOW_SLIDESHOW);
            return Load(GUIGraphicsContext.Skin + @"\EndlessCheez.xml");
        }

        private void InitSiteHistory() {
            CheezHistory = new Dictionary<CheezSite, List<CheezListItem>>();
            CheezSites = new List<CheezListItem>();
            foreach (CheezSite site in CheezManager.CheezSites) {
                CheezListItem tmpItem = new CheezListItem(site);
                if (String.IsNullOrEmpty(tmpItem.IconImageBig)) {
                    MediaPortal.Util.Utils.SetDefaultIcons(tmpItem);
                }
                CheezSites.Add(tmpItem);
                CheezHistory.Add(site, new List<CheezListItem>());
            }
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
            base.OnPageLoad();
            if (ctrlBackgroundImage != null) {
                ctrlBackgroundImage.ImagePath = GUIGraphicsContext.Skin + @"\Media\EndlessCheez\Background.png";
            }
            switch (PluginState) {
                case PluginStates.CheezSiteOverview:
                default:
                    DisplayCheezSitesOverview();
                    break;
                case PluginStates.CheezSiteSelected:
                case PluginStates.SlideShowRunning:
                    DisplayCurrentCheezSite(CheezManager.CurrentCheezSite);
                    break;
            }

            if (CheezManager.CheckCheezConnection()) {
                GUIPropertyManager.SetProperty("#currentmodule", Settings.PLUGIN_NAME + " (online mode)");
                IsOnlineMode = true;
            } else {
                GUIPropertyManager.SetProperty("#currentmodule", Settings.PLUGIN_NAME + " (local mode)");
                IsOnlineMode = false;
            }
            CurrentLayout = GUIFacadeControl.Layout.CoverFlow;
            SwitchLayout();           
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
                switch (PluginState) {
                    case PluginStates.CheezSiteSelected:
                    case PluginStates.SlideShowRunning:
                    default:
                        DisplayCheezSitesOverview();
                        return;
                    case PluginStates.CheezSiteOverview:
                        base.OnAction(action);
                        break;
                }
            } else {
                base.OnAction(action);
            }
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            switch (controlId) {
                case (int)Dialogs.ContextMenuButtons.BtnShowSlideShowAllLocal:
                    OnSlideShowAllLocal();
                    break;
                case (int)Dialogs.ContextMenuButtons.BtnShowSlideShowCurrent:
                    OnSlideShowCurrent();
                    break;
                case (int)Dialogs.ContextMenuButtons.BtnDeleteLocalCheez:
                    OnDeleteLocalCheez();
                    break;
                case (int)Dialogs.ContextMenuButtons.BtnBrowseLatestCheez:
                    GUIPropertyManager.SetProperty("#currentmodule", Settings.PLUGIN_NAME + " (online mode)");
                    IsOnlineMode = true;
                    break;
                case (int)Dialogs.ContextMenuButtons.BtnBrowseLocalCheez:
                    GUIPropertyManager.SetProperty("#currentmodule", Settings.PLUGIN_NAME + " (local mode)");
                    IsOnlineMode = false;
                    break;
                default:
                    break;
            }
            base.OnClicked(controlId, control, actionType);
        }

        protected override void OnClick(int iItem) {
            if (facadeLayout[iItem] is CheezListItem) {
                if (facadeLayout[iItem].IsFolder) {
                    DisplayCurrentCheezSite(CheezManager.GetCheezSiteByPath(facadeLayout[iItem].Path));
                } else {
                    if (facadeLayout[iItem].IsRemote) {                      
                        if (!TryPlayVideo((CheezListItem)facadeLayout[iItem])) {
                            if (IsOnlineVideosInstalled) {
                                PlayOnlineVideo((CheezListItem)facadeLayout[iItem]);
                            } else {
                                Dialogs.ShowNotifyDialog(10, "To view this video please\n install OnlineVideos plugin...");
                            }
                        }
                    } else {
                        OnSlideShowCurrent(facadeLayout[iItem].IconImageBig);
                    }
                }
            }

            base.OnClick(iItem);
        }

        private void PlayOnlineVideo(CheezListItem item) {
            if (IsOnlineVideosInstalled) {
                if (string.IsNullOrEmpty(OnlineVideoSettings.Instance.DllsDir)) {
                    OnlineVideoSettings.Instance.DllsDir = Path.Combine(MediaPortal.Configuration.Config.GetFolder(MediaPortal.Configuration.Config.Dir.Plugins), @"Windows\OnlineVideos");
                }
                SiteUtilFactory.GetAllNames();
                string url = OnlineVideos.Sites.GenericSiteUtil.GetVideoUrl(item.DVDLabel);               
                MediaPortal.Player.g_Player.PlayVideoStream(url, item.Label);
                MediaPortal.Player.g_Player.ShowFullScreenWindow();
            }
        }

        private bool CheckViddlerVideo(CheezListItem item) {
            return item.DVDLabel.Contains("viddler");
        }

        private bool TryPlayVideo(CheezListItem item) {
            try {
               if(CheckViddlerVideo(item)){
                    string page = new WebClient().DownloadString(item.DVDLabel);
                    if (!string.IsNullOrEmpty(page)) {
                        string regex = @"href=\""(.|\n)*?(.flv)(.|\n)*?";
                        MatchCollection matches = Regex.Matches(page, regex, RegexOptions.IgnoreCase);
                        for (int i = 1; i < matches.Count; i++) {
                            if (matches[i].ToString().Contains(".flv")) {                                
                                MediaPortal.Player.g_Player.PlayVideoStream("http://www.viddler.com" + matches[i].ToString().Replace("href=\"", ""), item.Label);
                                MediaPortal.Player.g_Player.ShowFullScreenWindow();
                            }
                        }
                    }
                    return true;
                }  
             
            } catch {
                Dialogs.ShowNotifyDialog(10, "Unable to play video:\n" + item.DVDLabel);                
            }
            return false;
        }

        private bool CheckOnlineVideosPLugin() {
            System.Reflection.Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
            return AppDomain.CurrentDomain.GetAssemblies().Count(a => a.FullName.Contains("OnlineVideos.MediaPortal1")) > 0;
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
                    SlideShow.Reset();
                    OnSlideShowCurrent();
                    break;
                case Dialogs.ContextMenuButtons.BtnCancelAllDownloads:
                    CancelCheezCollection();
                    break;
                case Dialogs.ContextMenuButtons.BtnDeleteLocalCheez:
                    OnDeleteLocalCheez();
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
            CheezManager.CurrentCheezSite = selectedCheezSite;

            facadeLayout.Clear();
            CheezHistory[selectedCheezSite].ForEach(item => facadeLayout.Add(item));
            if (IsOnlineMode) {
                if (facadeLayout.Count < 1) {
                    CollectLatestCheez(selectedCheezSite);
                } else {
                    facadeLayout.SelectedListItemIndex = CheezSites.First(x => x.Path == selectedCheezSite.SiteId).LastSelectedIndex;
                }
            } else {
                CollectLocalCheez(selectedCheezSite);
            }
            facadeLayout.DoUpdate();
        }

        private void ProcessAndDisplayNewCheez(CheezSite sourceSite, List<CheezItem> cheezItems) {
            Dialogs.HideProgressDialog();
            foreach (CheezItem cheezItem in cheezItems) {
                CheezListItem tmpItem = new CheezListItem(cheezItem);
                MediaPortal.Util.Utils.SetDefaultIcons(tmpItem);
                tmpItem.OnItemSelected += new GUIListItem.ItemSelectedHandler(OnItemSelected);
                if (SlideShow != null) {
                    SlideShow.Add(cheezItem.CheezImagePath);
                }
                if (PluginState == PluginStates.CheezSiteSelected) {
                    facadeLayout.Add(tmpItem);
                }
                if (sourceSite != null) {
                    CheezHistory[sourceSite].Add(tmpItem);
                }
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
                            if (IsOnlineMode) {
                                CollectLatestCheez(CheezManager.CurrentCheezSite);
                            }
                        }
                    }
                }
            }
        }

        private void OnDeleteLocalCheez() {
            if (Dialogs.ShowCustomYesNo("Delete all local Cheez?", "Are you sure to delete all of the \n" + CheezManager.LocalCheezCount + " local files?", "Yes!", "Cancel", false)) {
                if (DeleteLocalCheez()) {
                    Dialogs.ShowNotifyDialog(10, "Local Cheez successfully deleted!");
                } else {
                    Dialogs.ShowNotifyDialog(10, "Unable to delete all local files! (" + CheezManager.LocalCheezCount + " left)");
                }
                InitSiteHistory();
                CheezManager.ResetCurrentStartIndex();
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
            if (SlideShow != null) {
                for (int i = 0; i <= facadeLayout.Count; i++) {
                    if (facadeLayout[i] is CheezListItem && facadeLayout[i].HasIconBig && !facadeLayout[i].IsFolder) {
                        SlideShow.Add(facadeLayout[i].IconImageBig);
                    }
                }
                if (shuffle) {
                    SlideShow.Shuffle();
                }
                GUIWindowManager.ActivateWindow((int)Window.WINDOW_SLIDESHOW);
                SlideShow.StartSlideShow();
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
