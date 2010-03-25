using System;
using System.Windows.Forms;
using MediaPortal.GUI.Library;
using MediaPortal.GUI;
using MediaPortal.Dialogs;
using System.Collections.Generic;
using System.IO;
using CheezburgerAPI;

namespace EndlessCheez {
    public partial class EndlessCheezPlugin : GUIWindow, ICheezConsumer {

        internal enum PluginStates {
            DisplayCheezSites,
            DisplayCurrentCheezSite,
            BrowseLatest,
            BrowseRandom,
            BrowseLocal,
            DisplaySingleItem
        }

        private static PluginStates _currentState;
        private static CheezSite _currentCheezSite;
        private static List<CheezSite> _currentCheezItems;

        private static PluginStates CurrentState {
            get {
                return _currentState;
            }
        }

        private static CheezSite CurrentCheezSite {
            get {
                return _currentCheezSite;
            }
        }

        private static int CurrentCheezItemID {
            get {
                return _currentCheezSite.SelectedCheezItemID;
            }
            set {
                _currentCheezSite.SelectedCheezItemID = value;
            }
        }


        [SkinControlAttribute(400)]
        protected GUIFacadeControl controlFacade = null;

        public override bool Init() {
            _currentState = PluginStates.DisplayCheezSites;
            _currentCheezSite = CheezManager.GetCheezSiteByID(1);
            _currentCheezItems = new List<CheezSite>();
            return Load(GUIGraphicsContext.Skin + @"\EndlessCheez.xml");
        }

        protected override void OnPageLoad() {
            if(controlFacade != null) {
                DisplayCheezSitesOverview();
            }
            base.OnPageLoad();
        }

        private void DisplayCheezSitesOverview() {
            _currentState = PluginStates.DisplayCheezSites;
            GUIControl.ClearControl(GetID, controlFacade.GetID);
            controlFacade.View = GUIFacadeControl.ViewMode.List;
            foreach(CheezSite cheezSite in CheezManager.CheezSites) {
                controlFacade.Add(CreateGuiListFolder(cheezSite.ToString(), GUIGraphicsContext.Skin + @"\Media\EndlessCheez\" + cheezSite.CheezSiteID + ".png", cheezSite.CheezSiteIntID, cheezSite.Url));
            }
        }

        private void DisplayCurrentCheezSite(CheezSite selectedCheezSite) {
            _currentState = PluginStates.DisplayCurrentCheezSite;
            _currentCheezSite = selectedCheezSite;
            GUIControl.ClearControl(GetID, controlFacade.GetID);
            controlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
            controlFacade.Add(CreateGuiListFolder("Show Latest Online Cheez", "", 0, "CollectLatestCheez"));
            controlFacade.Add(CreateGuiListFolder("Show Random Online Cheez", "", 1, "CollectRandomCheez"));
            //CheezManager.CollectLocalCheez(currentSite);
        }

        public override void OnAction(MediaPortal.GUI.Library.Action action) {
            if(action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) {
                if(_currentState == PluginStates.DisplayCurrentCheezSite) {
                    DisplayCheezSitesOverview();
                    return;
                }
                if(_currentState == PluginStates.BrowseLatest || _currentState == PluginStates.BrowseLocal || _currentState == PluginStates.BrowseRandom) {
                    DisplayCurrentCheezSite(_currentCheezSite);
                    return;
                }
            }
            base.OnAction(action);
        }

        public override bool OnMessage(GUIMessage message) {
            return base.OnMessage(message);
        }

        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
            if(control == controlFacade && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
                if(_currentState == PluginStates.DisplayCheezSites) {
                    DisplayCurrentCheezSite(CheezManager.GetCheezSiteByID(controlFacade.SelectedListItem.ItemId));
                }
                if(_currentState == PluginStates.DisplayCurrentCheezSite) {
                    if(controlFacade.SelectedListItem.Path == "CollectLatestCheez") {
                        ShowLatestCheez();
                    } else if(controlFacade.SelectedListItem.Path == "CollectRandomCheez") {
                        ShowRandomCheez();
                    }
                }
            }
            base.OnClicked(controlId, control, actionType);
        }

        private void ShowRandomCheez() {
            _currentState = PluginStates.BrowseRandom;
            GUIControl.ClearControl(GetID, controlFacade.GetID);
            controlFacade.View = GUIFacadeControl.ViewMode.Filmstrip;
            CheezManager.CollectRandomCheez(_currentCheezSite);
        }

        private void ShowLatestCheez() {
            _currentState = PluginStates.DisplayCurrentCheezSite;
            GUIControl.ClearControl(GetID, controlFacade.GetID);
            controlFacade.View = GUIFacadeControl.ViewMode.Filmstrip;
            CheezManager.CollectLatestCheez(_currentCheezSite);
        }


        private GUIListItem CreateGuiListItem(CheezItem item, bool folder) {
            GUIListItem tmp = new GUIListItem(item.CheezTitle);
            tmp.Label2 = "[" + item.CheezCreationDate + "]";
            tmp.Path = item.CheezImagePath;
            tmp.IsFolder = folder;
            if(File.Exists(item.CheezImagePath)) {
                tmp.IconImage = item.CheezImagePath;
                tmp.IconImageBig = item.CheezImagePath;
                tmp.ThumbnailImage = item.CheezImagePath;
            }
            tmp.OnItemSelected += new GUIListItem.ItemSelectedHandler(tmp_OnItemSelected);
            return tmp;
        }

        void tmp_OnItemSelected(GUIListItem item, GUIControl parent) {
            if(parent.GetID == controlFacade.GetID) {
                if(controlFacade.SelectedListItemIndex == controlFacade.Count - 1) {
                    if(_currentState == PluginStates.DisplayCurrentCheezSite) {
                        CheezManager.CollectLatestCheez(_currentCheezSite);
                    } else if(_currentState == PluginStates.BrowseRandom) {
                        CheezManager.CollectRandomCheez(_currentCheezSite);
                    }
                }
            }
        }

        private GUIListItem CreateGuiListFolder(string label, string imagePath, int itemId, string itemPath) {
            GUIListItem guiListFolder = new GUIListItem(label);
            guiListFolder.Path = itemPath;
            guiListFolder.IsFolder = true;
            guiListFolder.ItemId = itemId;
            guiListFolder.IconImage = imagePath;
            guiListFolder.IconImageBig = imagePath;
            guiListFolder.ThumbnailImage = imagePath;
            return guiListFolder;
        }

        private void FacadeAddItems(List<CheezItem> cheezItems) {
            _currentCheezItems.AddRange(cheezItems);
            foreach(CheezItem item in cheezItems) {
                controlFacade.Add(CreateGuiListItem(item, false));
            }
        }

        // With GetID it will be an window-plugin / otherwise a process-plugin
        // Enter the id number here again
        public override int GetID {
            get {
                return EndlessCheezPlugin.GetWID;
            }
            set {
            }
        }

        #region ICheezConsumer Member

        public void OnCheezOperationFailed(CheezFail fail) {
            throw new NotImplementedException();
        }

        public void OnCheezOperationProgress(int progressPercentage, string currentItem) {
            throw new NotImplementedException();
        }

        public void OnLatestCheezArrived(List<CheezItem> cheezItems) {
            throw new NotImplementedException();
        }

        public void OnRandomCheezArrived(List<CheezItem> cheezItems) {
            throw new NotImplementedException();
        }

        public void OnLocalCheezArrived(List<CheezItem> cheezItems) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
