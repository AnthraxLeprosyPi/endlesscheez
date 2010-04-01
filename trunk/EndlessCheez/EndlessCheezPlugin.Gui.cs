using System;
using System.Windows.Forms;
using MediaPortal.GUI.Library;
using MediaPortal.GUI;
using MediaPortal.Dialogs;
using System.Collections.Generic;
using System.IO;
using CheezburgerAPI;
using System.Threading;
using MediaPortal.GUI.Pictures;

namespace EndlessCheez {
    public partial class EndlessCheezPlugin : GUIWindow, ICheezConsumer {

        #region Private Members

        private static PluginStates _currentState;
        private static CheezSite _currentCheezSite;
        private static List<CheezItem> _currentCheezItems;

        #endregion

        #region Skin Controls

        [SkinControlAttribute(50)]
        protected GUIFacadeControl ctrlFacade = null;

        [SkinControlAttribute(401)]
        protected GUIImage ctrlFullSizeImage = null;

        [SkinControlAttribute(600)]
        protected GUIImage ctrlBackgroundImage = null;

        [SkinControlAttribute(402)]
        protected GUIProgressControl ctrlProgressBar = null;

        [SkinControlAttribute(403)]
        protected GUILabelControl lblProgressLabel = null;

        #region TODO: delete
        [SkinControlAttribute(499)]
        protected GUIButtonControl btnCheezSitesOverview = null;

        [SkinControlAttribute(500)]
        protected GUIButtonControl btnBrowseLatest = null;

        [SkinControlAttribute(501)]
        protected GUIButtonControl btnBrowseRandom = null;

        [SkinControlAttribute(502)]
        protected GUIButtonControl btnBrowseLocal = null;

        [SkinControlAttribute(503)]
        protected GUIButtonControl btnSlideShowCurrent = null;

        [SkinControlAttribute(504)]
        protected GUIButtonControl btnSlideShowAllLocal = null;

        [SkinControlAttribute(505)]
        protected GUIButtonControl btnCancelAllDownloads = null;

        [SkinControlAttribute(506)]
        protected GUIButtonControl btnDeleteLocalCheez = null;
        #endregion
        #endregion

        #region GUIWindow Base Class Overrides

        public override bool Init() {
            GUIPropertyManager.SetProperty("#EndlessCheez.CurrentItem", " ");
            _currentState = _defaultStartupState;
            _currentCheezSite = CheezManager.GetCheezSiteByID(1);
            _currentCheezItems = new List<CheezItem>();
            return Load(GUIGraphicsContext.Skin + @"\EndlessCheez.xml");
        }

        protected override void OnPageLoad() {
            InitCheezManager(_fetchCount, _cheezRootFolder, true);
            btnSlideShowAllLocal.IsVisible = btnDeleteLocalCheez.IsVisible = btnBrowseLocal.IsVisible = (CheezManager.LocalCheezCount > 0);
            btnBrowseLatest.IsVisible = btnBrowseRandom.IsVisible = btnCheezSitesOverview.IsVisible = false;
            if(ctrlBackgroundImage != null) {
                ctrlBackgroundImage.ImagePath = GUIGraphicsContext.Skin + @"\Media\EndlessCheez\Background.png";
            }
            if(ctrlFacade != null) {
                switch(_defaultStartupState) {
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
            if(control == ctrlFacade && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
                if(_currentState == PluginStates.DisplayCheezSites) {
                    DisplayCurrentCheezSite(CheezManager.GetCheezSiteByID(ctrlFacade.SelectedListItem.ItemId));
                } else if(_currentState == PluginStates.BrowseLatest || _currentState == PluginStates.BrowseLocal || _currentState == PluginStates.BrowseRandom) {
                    OnSlideShowCurrent(ctrlFacade.SelectedListItem.IconImageBig);
                }
            }
            #region TODO: delete
            //if(control == btnCheezSitesOverview) {
            //    DisplayCheezSitesOverview();
            //}
            //if(control == btnCancelAllDownloads) {
            //    CancelCheezCollection();
            //}
            //if(control == btnBrowseLatest) {
            //    BrowseLatestCheez();
            //}
            //if(control == btnBrowseRandom) {
            //    BrowseRandomCheez();
            //}
            //if(control == btnBrowseLocal) {
            //    BrowseLocalCheez(_currentCheezSite);
            //}
            //if(control == btnSlideShowCurrent) {
            //    OnSlideShowCurrent();
            //}
            //if(control == btnSlideShowAllLocal) {
            //    OnSlideShowAllLocal();
            //}
            //if(control == btnDeleteLocalCheez) {
            //    OnDeleteLocalCheez();
            //}
            #endregion
            base.OnClicked(controlId, control, actionType);
        }

        protected override void OnShowContextMenu() {
            IDialogbox contextMenu = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if(contextMenu == null) {
                return;
            }
            contextMenu.Reset();
            contextMenu.SetHeading("EndlessCheez Menu");
            foreach(GUIListItem menuItem in GetCurrentContextMenu(_currentState)) {
                contextMenu.Add(menuItem);
            }
            contextMenu.DoModal(GUIWindowManager.ActiveWindow);            
            switch((ContextMenuButtons)contextMenu.SelectedId) {
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
                    throw new NotImplementedException();
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
                default:
                    throw new ArgumentOutOfRangeException();
                    return;                   
            }
        }

        public override void OnAction(MediaPortal.GUI.Library.Action action) {
            if(action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) {
                if(_currentState == PluginStates.DisplayCurrentCheezSite) {
                    DisplayCheezSitesOverview();
                    return;
                }
                if(_currentState == PluginStates.BrowseLatest || _currentState == PluginStates.BrowseLocal || _currentState == PluginStates.BrowseRandom) {
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
            if(parent.GetID == ctrlFacade.GetID) {
                if(!CheezManager.IsBusy) {
                    if(ctrlFacade.SelectedListItemIndex == ctrlFacade.Count - 1) {
                        if(_currentState == PluginStates.BrowseLatest) {
                            CollectLatestCheez(_currentCheezSite);
                        } else if(_currentState == PluginStates.BrowseRandom) {
                            CollectRandomCheez(_currentCheezSite);
                        } else if(_currentState == PluginStates.BrowseLocal) {
                            //CollectLocalCheez(_currentCheezSite);
                        }
                    }
                }
            }
        }

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
            if(_currentCheezItems == null || _currentCheezItems.Count <= 0) {
                OnSlideShowAllLocal();
                return;
            }
            GUISlideShow SlideShow = (GUISlideShow)GUIWindowManager.GetWindow((int)Window.WINDOW_SLIDESHOW);
            SlideShow.Reset();
            if(SlideShow != null) {
                foreach(CheezItem currentItem in _currentCheezItems) {
                    SlideShow.Add(currentItem.CheezImagePath);
                }
                GUIWindowManager.ActivateWindow((int)Window.WINDOW_SLIDESHOW);
                SlideShow.StartSlideShow();
                if(shuffle) {
                    SlideShow.Shuffle();
                }
                if(!String.IsNullOrEmpty(startImage)) {
                    SlideShow.Select(startImage);
                }
            }

        }

        private void OnSlideShowAllLocal() {
            BrowseLocalCheez();
            while(CheezManager.IsBusy) {
                Thread.Sleep(100);
            }
            if(ShowCustomYesNo("Shuffle Slideshow?", "", "Yes", "No", true)) {
                OnSlideShowCurrent(true);
            } else {
                OnSlideShowCurrent(false);
            }

        }

        private void OnDeleteLocalCheez() {
            if(ShowCustomYesNo("Delete all local Cheez?", "Are you sure to delete all of the " + CheezManager.LocalCheezCount + " local files?", "Yes!", "Cancel", false)) {
                if(DeleteLocalCheez()) {
                    ShowNotifyDialog(10, "Local Cheez successfully deleted!");
                } else {
                    ShowNotifyDialog(10, "Unable to delete all local files! (" + CheezManager.LocalCheezCount + " left)");
                }
                DisplayCheezSitesOverview();
            }
        }

        #endregion

        #region Private Methods
        private void DisplayCheezSitesOverview() {
            _currentState = PluginStates.DisplayCheezSites;
            _currentCheezSite = null;
            _currentCheezItems.Clear();
            btnBrowseLatest.IsVisible = btnBrowseRandom.IsVisible = btnCheezSitesOverview.IsVisible = false;
            btnBrowseLocal.IsVisible = (CheezManager.LocalCheezCount > 0);
            GUIControl.ClearControl(GetID, ctrlFacade.GetID);
            ctrlFacade.View = GUIFacadeControl.ViewMode.List;
            foreach(CheezSite cheezSite in CheezManager.CheezSites) {
                ctrlFacade.Add(CreateGuiListFolder(cheezSite.ToString(), GUIGraphicsContext.Skin + @"\Media\EndlessCheez\" + cheezSite.CheezSiteID + ".png", cheezSite.CheezSiteIntID, cheezSite.Url));
            }
        }

        private void DisplayCurrentCheezSite(CheezSite selectedCheezSite) {
            btnBrowseLatest.IsVisible = btnBrowseRandom.IsVisible = btnCheezSitesOverview.IsVisible = true;
            btnBrowseLocal.IsVisible = (CheezManager.LocalCheezCount > 0);
            _currentState = PluginStates.DisplayCurrentCheezSite;
            _currentCheezSite = selectedCheezSite;
            _currentCheezItems.Clear();
            GUIControl.ClearControl(GetID, ctrlFacade.GetID);
            ctrlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
            BrowseLatestCheez();
        }

        private void BrowseRandomCheez() {
            if(_currentState == PluginStates.DisplayCurrentCheezSite) {
                _currentCheezItems.Clear();
            }
            _currentState = PluginStates.BrowseRandom;
            GUIControl.ClearControl(GetID, ctrlFacade.GetID);
            ctrlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
            CollectRandomCheez(_currentCheezSite);
        }

        private void BrowseLatestCheez() {
            if(_currentState == PluginStates.DisplayCurrentCheezSite) {
                _currentCheezItems.Clear();
            }
            _currentState = PluginStates.BrowseLatest;
            GUIControl.ClearControl(GetID, ctrlFacade.GetID);
            ctrlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
            CollectLatestCheez(_currentCheezSite);
        }

        private void BrowseLocalCheez() {
            BrowseLocalCheez(null);
        }

        private void BrowseLocalCheez(CheezSite selectedCheezSite) {
            if(_currentState == PluginStates.DisplayCurrentCheezSite) {
                _currentCheezItems.Clear();
            }
            _currentState = PluginStates.BrowseLocal;
            GUIControl.ClearControl(GetID, ctrlFacade.GetID);
            ctrlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
            CollectLocalCheez(selectedCheezSite);
        }

        private void ProcessAndDisplayNewCheez(List<CheezItem> cheezItems) {
            lock(_currentCheezItems) {
                HideProgressInfo();
                _currentCheezItems.AddRange(cheezItems);
                btnDeleteLocalCheez.IsVisible = (CheezManager.LocalCheezCount > 0);
                if(_currentState == PluginStates.BrowseLatest || _currentState == PluginStates.BrowseLocal || _currentState == PluginStates.BrowseRandom || _currentState == PluginStates.DisplayCurrentCheezSite) {
                    foreach(CheezItem cheezItem in cheezItems) {
                        ctrlFacade.Add(CreateGuiListItem(cheezItem));
                    }
                }
                ctrlFacade.DoUpdate();
            }
        }

        private void SwitchPluginState(PluginStates newState) {
            switch(newState) {
                case PluginStates.BrowseLatest:
                case PluginStates.BrowseLocal:
                case PluginStates.BrowseRandom:
                    break;
                case PluginStates.DisplayCheezSites:
                    break;
                case PluginStates.DisplayCurrentCheezSite:
                    break;
                case PluginStates.DisplayLocalOnly:
                    break;
                default:
                    break;
            }
            _currentState = newState;
        }

        #endregion

        #region GUI Helper Methods

        private GUIListItem CreateGuiListFolder(string label, string imagePath, int itemId, string itemPath) {
            GUIListItem guiListFolder = new GUIListItem(label);
            guiListFolder.Path = itemPath;
            guiListFolder.IsFolder = true;
            guiListFolder.ItemId = itemId;
            if(File.Exists(imagePath)) {
                guiListFolder.IconImage = imagePath;
                guiListFolder.IconImageBig = imagePath;
                guiListFolder.ThumbnailImage = imagePath;
            }
            return guiListFolder;
        }

        private GUIListItem CreateGuiListItem(CheezItem item) {
            GUIListItem tmp = new GUIListItem(item.CheezTitle);
            tmp.Label2 = "[" + item.CheezCreationDate + "]";
            tmp.Path = item.CheezImagePath;
            tmp.IsFolder = false;
            if(File.Exists(item.CheezImagePath)) {
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
            } catch(Exception ex) {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// Displays a yes/no dialog with custom labels for the buttons
        /// This method may become obsolete in the future if media portal adds more dialogs
        /// </summary>
        /// <returns>True if yes was clicked, False if no was clicked</returns>
        public bool ShowCustomYesNo(string heading, string lines, string yesLabel, string noLabel, bool defaultYes) {
            GUIDialogYesNo dialog = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            try {
                dialog.Reset();
                dialog.SetHeading(heading);
                string[] linesArray = lines.Split(new string[] { "\\n" }, StringSplitOptions.None);
                if(linesArray.Length > 0)
                    dialog.SetLine(1, linesArray[0]);
                if(linesArray.Length > 1)
                    dialog.SetLine(2, linesArray[1]);
                if(linesArray.Length > 2)
                    dialog.SetLine(3, linesArray[2]);
                if(linesArray.Length > 3)
                    dialog.SetLine(4, linesArray[3]);
                dialog.SetDefaultToYes(defaultYes);

                foreach(System.Windows.UIElement item in dialog.Children) {
                    if(item is GUIButtonControl) {
                        GUIButtonControl btn = (GUIButtonControl)item;
                        if(btn.GetID == 11 && !String.IsNullOrEmpty(yesLabel)) // Yes button
                            btn.Label = yesLabel;
                        else if(btn.GetID == 10 && !String.IsNullOrEmpty(noLabel)) // No button
                            btn.Label = noLabel;
                    }
                }
                dialog.DoModal(GetID);
                return dialog.IsConfirmed;
            } finally {
                // set the standard yes/no dialog back to it's original state (yes/no buttons)
                if(dialog != null) {
                    dialog.ClearAll();
                }
            }
        }

        private void ShowItemFullSize(GUIListItem item) {
            if(ctrlFullSizeImage != null) {
                ctrlFullSizeImage.ImagePath = item.IconImageBig;
                ctrlFullSizeImage.DoUpdate();
            }
        }

        private void ShowFiltersMenu() {
            //IDialogbox dlg = (IDialogbox)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            //if(dlg == null)
            //    return;

            //dlg.Reset();
            //dlg.SetHeading(Translation.FanArtFilter);

            //GUIListItem pItem = new GUIListItem(Translation.FanArtFilterAll);
            //dlg.Add(pItem);
            //pItem.ItemId = (int)menuFilterAction.all;

            //pItem = new GUIListItem("1280x720");
            //dlg.Add(pItem);
            //pItem.ItemId = (int)menuFilterAction.hd;

            //pItem = new GUIListItem("1920x1080");
            //dlg.Add(pItem);
            //pItem.ItemId = (int)menuFilterAction.fullhd;

            //dlg.DoModal(GUIWindowManager.ActiveWindow);
            //if(dlg.SelectedId >= 0) {
            //    switch(dlg.SelectedId) {
            //        case (int)menuFilterAction.all:
            //            DBOption.SetOptions(DBOption.cFanartThumbnailResolutionFilter, "0");
            //            break;
            //        case (int)menuFilterAction.hd:
            //            DBOption.SetOptions(DBOption.cFanartThumbnailResolutionFilter, "1");
            //            break;
            //        case (int)menuFilterAction.fullhd:
            //            DBOption.SetOptions(DBOption.cFanartThumbnailResolutionFilter, "2");
            //            break;
            //    }
            //    m_Facade.Clear();
            //    DBFanart.ClearAll();
            //    ClearProperties();

            //    UpdateFilterProperty(false);
            //    loadingWorker.RunWorkerAsync(SeriesID);
            //}
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

        #region ICheezConsumer Member

        public void OnCheezOperationFailed(CheezFail fail) {
            HideProgressInfo();
            ShowNotifyDialog(10, fail.ToString());
            DisplayCheezSitesOverview();
        }

        public void OnCheezOperationProgress(int progressPercentage, string currentItem) {
            if(ctrlProgressBar != null) {
                if(progressPercentage >= 0 && progressPercentage <= 100) {
                    ctrlProgressBar.Percentage = progressPercentage;
                    if(String.IsNullOrEmpty(currentItem)) {
                        currentItem = " ";
                    }
                    GUIPropertyManager.SetProperty("#EndlessCheez.CurrentItem", String.Format("{0} ({1}%)", currentItem, progressPercentage.ToString()));
                }
            }
            this.Process();
        }


        public void OnLatestCheezArrived(List<CheezItem> cheezItems) {
            HideProgressInfo();
            ProcessAndDisplayNewCheez(cheezItems);
        }

        public void OnRandomCheezArrived(List<CheezItem> cheezItems) {
            HideProgressInfo();
            ProcessAndDisplayNewCheez(cheezItems);
        }

        public void OnLocalCheezArrived(List<CheezItem> cheezItems) {
            HideProgressInfo();
            ProcessAndDisplayNewCheez(cheezItems);
        }

        #endregion

    }
}
