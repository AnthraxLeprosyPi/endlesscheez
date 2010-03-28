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

        [SkinControlAttribute(404)]
        protected GUIButtonControl btnCancel = null;

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

        #endregion

        #region GUIWindow Base Class Overrides

        public override bool Init() {
            _currentState = _defaultStartupState;
            _currentCheezSite = CheezManager.GetCheezSiteByID(1);
            _currentCheezItems = new List<CheezItem>();
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
                }
            }
            if (control == btnCancel) {
                CancelCheezCollection();
            }
            if (control == btnBrowseLatest) {
                BrowseLatestCheez();
            }
            if (control == btnBrowseRandom) {
                BrowseRandomCheez();
            }
            if (control == btnBrowseLocal) {
                BrowseLocalCheez(_currentCheezSite);
            }
            if (control == btnSlideShowCurrent) {
                OnSlideShowCurrent();
            }
            if (control == btnSlideShowAllLocal) {
                OnSlideShowAllLocal();
            }
            base.OnClicked(controlId, control, actionType);
        }

        public override void OnAction(MediaPortal.GUI.Library.Action action) {
            if (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) {
                if (_currentState == PluginStates.DisplayCurrentCheezSite) {
                    DisplayCheezSitesOverview();
                    return;
                }
                if (_currentState == PluginStates.BrowseLatest || _currentState == PluginStates.BrowseLocal || _currentState == PluginStates.BrowseRandom) {
                    DisplayCurrentCheezSite(_currentCheezSite);
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

        private void OnSlideShowCurrent() {
            if (_currentCheezItems == null || _currentCheezItems.Count <= 0) {
                OnSlideShowAllLocal();
                return;
            }
            GUISlideShow SlideShow = (GUISlideShow)GUIWindowManager.GetWindow((int)Window.WINDOW_SLIDESHOW);
            if (SlideShow != null) {
                foreach (CheezItem currentItem in _currentCheezItems) {
                    SlideShow.Add(currentItem.CheezImagePath);
                }
                GUIWindowManager.ActivateWindow((int)Window.WINDOW_SLIDESHOW);
                SlideShow.StartSlideShow();
            }
        }


        private void OnSlideShowAllLocal() {
            BrowseLocalCheez();
            //while (CheezManager.IsBusy) {
            //    Thread.Sleep(100);
            //}
            OnSlideShowCurrent();
        }

        #endregion

        #region Private Methods
        private void DisplayCheezSitesOverview() {
            _currentState = PluginStates.DisplayCheezSites;
            _currentCheezSite = null;
            _currentCheezItems.Clear();
            GUIControl.ClearControl(GetID, ctrlFacade.GetID);
            ctrlFacade.View = GUIFacadeControl.ViewMode.List;
            foreach (CheezSite cheezSite in CheezManager.CheezSites) {
                ctrlFacade.Add(CreateGuiListFolder(cheezSite.ToString(), GUIGraphicsContext.Skin + @"\Media\EndlessCheez\" + cheezSite.CheezSiteID + ".png", cheezSite.CheezSiteIntID, cheezSite.Url));
            }
        }

        private void DisplayCurrentCheezSite(CheezSite selectedCheezSite) {
            _currentState = PluginStates.DisplayCurrentCheezSite;
            _currentCheezSite = selectedCheezSite;
            _currentCheezItems.Clear();
            GUIControl.ClearControl(GetID, ctrlFacade.GetID);
            ctrlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
            BrowseLocalCheez(selectedCheezSite);
        }


        private void BrowseRandomCheez() {
            if (_currentState == PluginStates.DisplayCurrentCheezSite) {
                _currentCheezItems.Clear();
            }
            _currentState = PluginStates.BrowseRandom;
            GUIControl.ClearControl(GetID, ctrlFacade.GetID);
            ctrlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
            CollectRandomCheez(_currentCheezSite);
        }

        private void BrowseLatestCheez() {
            if (_currentState == PluginStates.DisplayCurrentCheezSite) {
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
            if (_currentState == PluginStates.DisplayCurrentCheezSite) {
                _currentCheezItems.Clear();
            }
            //_currentState = PluginStates.BrowseLocal;
            GUIControl.ClearControl(GetID, ctrlFacade.GetID);
            ctrlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
            CollectLocalCheez(selectedCheezSite);
        }

        private void ProcessAndDisplayNewCheez(List<CheezItem> cheezItems) {
            _currentCheezItems.AddRange(cheezItems);
            if (_currentState == PluginStates.BrowseLatest || _currentState == PluginStates.BrowseLocal || _currentState == PluginStates.BrowseRandom || _currentState == PluginStates.DisplayCurrentCheezSite) {
                foreach (CheezItem cheezItem in cheezItems) {
                    ctrlFacade.Add(CreateGuiListItem(cheezItem));
                }
            }
        }

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
            tmp.Label2 = "[" + item.CheezCreationDate + "]";
            tmp.Path = item.CheezImagePath;
            tmp.IsFolder = false;
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

        private void ShowItemFullSize(GUIListItem item) {
            if (ctrlFullSizeImage != null) {
                ctrlFullSizeImage.ImagePath = item.IconImageBig;
                ctrlFullSizeImage.DoUpdate();
            }
        }


        private void ShowProgressInfo() {
            ctrlProgressBar.IsVisible = true;
            lblProgressLabel.IsVisible = true;
        }

        private void HideProgressInfo() {
            ctrlProgressBar.IsVisible = false;
            lblProgressLabel.IsVisible = false;
        }

        #endregion

        #region ICheezConsumer Member

        public void OnCheezOperationFailed(CheezFail fail) {
            HideProgressInfo();
            ShowNotifyDialog(30, fail.ToString());
        }

        public void OnCheezOperationProgress(int progressPercentage, string currentItem) {
            if (ctrlProgressBar != null) {
                if (progressPercentage >= 0 && progressPercentage <= 100) {
                    ctrlProgressBar.Percentage = progressPercentage;
                    lblProgressLabel.Label = String.Format("Current item: {0} ({1}%)", currentItem, progressPercentage.ToString());
                    GUIPropertyManager.SetProperty("#EndlessCheez.CurrentItem",  String.Format("Current item: {0} ({1}%)", currentItem, progressPercentage.ToString()));                   
                }
            }            
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
