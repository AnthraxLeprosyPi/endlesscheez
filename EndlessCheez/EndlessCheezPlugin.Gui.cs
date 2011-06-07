using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CheezburgerAPI;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Pictures;

namespace EndlessCheez {
    public partial class EndlessCheezPlugin : , ICheezConsumer {

        #region Private Members

        private static PluginStates _currentState;
        private static CheezSite _currentCheezSite;
        private static List<CheezItem> _currentCheezItems;

        #endregion

        #region Skin Controls

        [SkinControlAttribute(50)]
        protected GUIFacadeControl ctrlFacade = null;

        [SkinControlAttribute(600)]
        protected GUIImage ctrlBackgroundImage = null;

        [SkinControlAttribute(402)]
        protected GUIProgressControl ctrlProgressBar = null;

        [SkinControlAttribute(403)]
        protected GUILabelControl lblProgressLabel = null;

        private static GUISlideShow SlideShow;

        #endregion

        #region GUIWindow Base Class Overrides

        public override bool Init() {
            GUIPropertyManager.SetProperty("#EndlessCheez.CurrentItem", " ");
            _currentState = _defaultStartupState;
            _currentCheezSite = CheezManager.GetCheezSiteByID(1);
            _currentCheezItems = new List<CheezItem>();
            SlideShow = (GUISlideShow)GUIWindowManager.GetWindow((int)Window.WINDOW_SLIDESHOW);
            return Load(GUIGraphicsContext.Skin + @"\EndlessCheez.xml");
        }

        protected override void OnPageLoad() {
            InitCheezManager(_fetchCount, _cheezRootFolder, true);
            if (ctrlBackgroundImage != null) {
                ctrlBackgroundImage.ImagePath = GUIGraphicsContext.Skin + @"\Media\EndlessCheez\Background.png";
            }
            if (ctrlFacade != null) {
                switch (_defaultStartupState) {
                    case PluginStates.DisplayCheezSites:
                        DisplayCheezSitesOverview();
                        break;
                    case PluginStates.DisplayLocalOnly:
                    default:
                        BrowseLocalCheez();
                        break;
                }
            }
            base.OnPageLoad();
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if (control == ctrlFacade && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
                if (_currentState == PluginStates.DisplayCheezSites) {
                    DisplayCurrentCheezSite(CheezManager.GetCheezSiteByID(ctrlFacade.SelectedListItem.ItemId));
                } else if (_currentState == PluginStates.BrowseLatest || _currentState == PluginStates.BrowseLocal || _currentState == PluginStates.BrowseRandom) {
                    if (ctrlFacade.SelectedListItem.IsRemote) {
                        MediaPortal.Player.g_Player.PlayVideoStream(ctrlFacade.SelectedListItem.DVDLabel, ctrlFacade.SelectedListItem.ToString());
                    } else {
                        OnSlideShowCurrent(ctrlFacade.SelectedListItem.IconImageBig);
                    }

                }
            }
            base.OnClicked(controlId, control, actionType);
        }

        protected override void OnShowContextMenu() {
            switch (GetCurrentContextMenu(_currentState)) {
                case ContextMenuButtons.BtnCheezSitesOverview:
                    DisplayCheezSitesOverview();
                    break;
                case ContextMenuButtons.BtnBrowseLatestCheez:
                    BrowseLatestCheez();
                    break;
                case ContextMenuButtons.BtnBrowseLocalCheez:
                    BrowseLocalCheez();
                    break;
                case ContextMenuButtons.BtnBrowseRandomCheez:
                    BrowseRandomCheez();
                    break;
                case ContextMenuButtons.BtnBrowseMore:
                    switch (_currentState) {
                        case PluginStates.BrowseLatest:
                            BrowseLatestCheez();
                            break;
                        case PluginStates.BrowseRandom:
                            BrowseRandomCheez();
                            break;
                    }
                    break;
                case ContextMenuButtons.BtnSortAsc:
                    if (ctrlFacade != null) {
                        ctrlFacade.Sort(new CheezComparerDateAsc());
                        this.Process();
                    }
                    break;
                case ContextMenuButtons.BtnSortDesc:
                    if (ctrlFacade != null) {
                        ctrlFacade.Sort(new CheezComparerDateDesc());
                        this.Process();
                    }
                    break;
                case ContextMenuButtons.BtnShowSlideShowAllLocal:
                    OnSlideShowAllLocal();
                    break;
                case ContextMenuButtons.BtnShowSlideShowCurrent:
                    OnSlideShowCurrent();
                    break;
                case ContextMenuButtons.BtnCancelAllDownloads:
                    CancelCheezCollection();
                    break;
                case ContextMenuButtons.BtnDeleteLocalCheez:
                    DeleteLocalCheez();
                    break;
                case ContextMenuButtons.NothingSelected:
                default:
                    //throw new ArgumentOutOfRangeException();
                    return;
            }
        }

        public override void OnAction(MediaPortal.GUI.Library.Action action) {
            if (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) {
                if (_currentState == PluginStates.DisplayCurrentCheezSite) {
                    DisplayCheezSitesOverview();
                    return;
                }
                if (_currentState == PluginStates.BrowseLatest || _currentState == PluginStates.BrowseLocal || _currentState == PluginStates.BrowseRandom) {
                    DisplayCheezSitesOverview();
                    return;
                }
            }
            base.OnAction(action);
        }

        public override bool OnMessage(GUIMessage message) {
            return base.OnMessage(message);
        }
        #endregion

        #region Plugin Event Handlers

        private void OnItemSelected(GUIListItem item, GUIControl parent) {
            if (parent.GetID == ctrlFacade.GetID) {
                if (!CheezManager.IsBusy) {
                    if (ctrlFacade.SelectedListItemIndex == ctrlFacade.Count - 1) {
                        if (_currentState == PluginStates.BrowseLatest) {
                            CollectLatestCheez(_currentCheezSite);
                        } else if (_currentState == PluginStates.BrowseRandom) {
                            CollectRandomCheez(_currentCheezSite);
                        } else if (_currentState == PluginStates.BrowseLocal) {
                            //CollectLocalCheez(_currentCheezSite);
                        }
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
            if (_currentCheezItems == null || _currentCheezItems.Count <= 0) {
                OnSlideShowAllLocal();
                return;
            }
            SlideShow.Reset();
            if (SlideShow != null) {
                foreach (CheezItem currentItem in _currentCheezItems) {
                    SlideShow.Add(currentItem.CheezImagePath);
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
            BrowseLocalCheez();
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
        private void DisplayCheezSitesOverview() {
            SwitchPluginState(PluginStates.DisplayCheezSites);
            foreach (CheezSite cheezSite in CheezManager.CheezSites) {
                ctrlFacade.Add(CreateGuiListFolder(cheezSite.ToString(), GUIGraphicsContext.Skin + @"\Media\EndlessCheez\" + cheezSite.CheezSiteID + ".png", cheezSite.CheezSiteIntID, cheezSite.Url));
            }
        }

        private void DisplayCurrentCheezSite(CheezSite selectedCheezSite) {
            _currentCheezSite = selectedCheezSite;
            SwitchPluginState(PluginStates.DisplayCurrentCheezSite);
            BrowseLatestCheez();
        }

        private void BrowseRandomCheez() {
            SwitchPluginState(PluginStates.BrowseRandom);
            CollectRandomCheez(_currentCheezSite);
        }

        private void BrowseLatestCheez() {
            SwitchPluginState(PluginStates.BrowseLatest);
            CollectLatestCheez(_currentCheezSite);
        }

        private void BrowseLocalCheez() {
            BrowseLocalCheez(null);
        }

        private void BrowseLocalCheez(CheezSite selectedCheezSite) {
            SwitchPluginState(PluginStates.BrowseLocal);
            CollectLocalCheez(selectedCheezSite);
        }

        private void ProcessAndDisplayNewCheez(List<CheezItem> cheezItems) {
            HideProgressInfo();
            lock (_currentCheezItems) {
                _currentCheezItems.AddRange(cheezItems);
                if (_currentState == PluginStates.BrowseLatest || _currentState == PluginStates.BrowseLocal || _currentState == PluginStates.BrowseRandom) {
                    foreach (CheezItem cheezItem in cheezItems) {
                        if (SlideShow != null && SlideShow.InSlideShow) {
                            SlideShow.Add(cheezItem.CheezImagePath);
                        }
                        ctrlFacade.Add(CreateGuiListItem(cheezItem));
                    }
                }
                ctrlFacade.DoUpdate();
            }

        }

        private static bool CheezItemsAvailable() {
            return (_currentCheezItems != null && _currentCheezItems.Count > 0);
        }

        private void SwitchPluginState(PluginStates newState) {
            if (_currentState == newState) {
                return;
            }
            _currentState = newState;
            _currentCheezItems.Clear();
            GUIControl.ClearControl(GetID, ctrlFacade.GetID);
            switch (newState) {
                case PluginStates.BrowseLatest:
                case PluginStates.BrowseLocal:
                case PluginStates.BrowseRandom:
                    HideProgressInfo();
                    ctrlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
                    break;
                case PluginStates.DisplayCheezSites:
                    _currentCheezSite = null;
                    ctrlFacade.View = GUIFacadeControl.ViewMode.List;
                    break;
                case PluginStates.DisplayCurrentCheezSite:
                    ctrlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
                    break;
                case PluginStates.DisplayLocalOnly:
                    break;
                default:
                    break;
            }
        }

        //private CheezItem GetCheezItemFromFacade(string itemPermanentLink) {
        //    try {
        //        return _currentCheezItems.First(item => item.CheezAsset.Permalink == itemPermanentLink); 
        //    } catch {
        //        throw new IndexOutOfRangeException();
        //    }
        //}

        #endregion

        #region GUI Helper Methods

        private GUIListItem CreateGuiListFolder(string label, string imagePath, int itemId, string itemPath) {
            GUIListItem guiListFolder = new GUIListItem(label);
            guiListFolder.Path = itemPath;
            guiListFolder.IsFolder = true;
            guiListFolder.ItemId = itemId;
            if (File.Exists(imagePath)) {
                guiListFolder.IconImage = imagePath;
                guiListFolder.IconImageBig = imagePath;
                guiListFolder.ThumbnailImage = imagePath;
            }
            return guiListFolder;
        }

        private GUIListItem CreateGuiListItem(CheezItem item) {
            GUIListItem tmp = new GUIListItem(item.CheezTitle);
            tmp.Label2 = "[" + item.CheezCreationDateTime.ToShortDateString() + "]";
            tmp.Path = item.CheezImagePath;
            tmp.DVDLabel = item.CheezAsset.ContentUrl;
            tmp.FileInfo.CreationTime = item.CheezCreationDateTime;
            tmp.IsFolder = false;
            tmp.IsRemote = item.CheezAsset.AssetType.Contains("video");
            if (File.Exists(item.CheezImagePath)) {
                tmp.IconImage = item.CheezImagePath;
                tmp.IconImageBig = item.CheezImagePath;
                tmp.ThumbnailImage = item.CheezImagePath;
            }
            tmp.OnItemSelected += new GUIListItem.ItemSelectedHandler(OnItemSelected);
            return tmp;
        }

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
