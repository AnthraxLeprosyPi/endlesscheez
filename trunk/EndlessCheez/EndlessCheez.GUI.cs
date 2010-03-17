//using System;
//using System.Windows.Forms;
//using MediaPortal.GUI.Library;
//using MediaPortal.GUI;
//using MediaPortal.Dialogs;
//using System.Collections.Generic;
//using System.IO;
//using CheezburgerAPI;

//namespace EndlessCheez {
//    public partial class EndlessCheezPlugin : GUIWindow {
//        private static List<CheezItem> allCheezItems = new List<CheezItem>();
//        private CurrentState currentState = CurrentState.CategoryList;
//        private CheezSite currentSite;


//        [SkinControlAttribute(400)]
//        protected GUIFacadeControl controlFacade = null;

//        private enum CurrentState {
//            CategoryList,
//            SelectedCategory,
//            SelectedLatest,
//            SelectedRandom,
//            SelectedSingleItem
//        }

//        public override bool Init() {
//            return Load(GUIGraphicsContext.Skin + @"\EndlessCheez.xml");
//        }

//        protected override void OnPageLoad() {
//            if(controlFacade != null) {
//                BuildCategoryList();
//            }
//            base.OnPageLoad();
//        }

//        private void BuildCategoryList() {
//            currentState = CurrentState.CategoryList;
//            GUIControl.ClearControl(GetID, controlFacade.GetID);
//            controlFacade.View = GUIFacadeControl.ViewMode.List;
//            foreach(CheezSite cheezSite in CheezManager.CheezSites) {
//                controlFacade.Add(CreateGuiListFolder(cheezSite.Name, GUIGraphicsContext.Skin + @"\Media\EndlessCheez\" + cheezSite.Name + ".png", CheezManager.GetCheezSiteID(cheezSite), cheezSite.Name));
//            }
//        }

//        private void BuildSelectedCategory(string categoryName) {
//            currentState = CurrentState.SelectedCategory;
//            currentCategory = (CheezManager.CheezCategories)Enum.Parse(typeof(CheezManager.CheezCategories), categoryName);
//            GUIControl.ClearControl(GetID, controlFacade.GetID);
//            controlFacade.View = GUIFacadeControl.ViewMode.LargeIcons;
//            controlFacade.Add(CreateGuiListFolder("Show Latest Online Cheez", "", 0, "CollectLatestCheez"));
//            controlFacade.Add(CreateGuiListFolder("Show Random Online Cheez", "", 1, "CollectRandomCheez"));
//            CheezManager.CollectLocalCheez(currentCategory);
//        }

//        public override void OnAction(MediaPortal.GUI.Library.Action action) {
//            if(action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU) {
//                if(currentState == CurrentState.SelectedCategory) {
//                    BuildCategoryList();
//                    return;
//                }
//                if(currentState == CurrentState.SelectedLatest || currentState == CurrentState.SelectedRandom) {
//                    BuildSelectedCategory(currentCategory.GetShortDescription<CheezManager.CheezCategories>());
//                    return;
//                }
//            }
//            base.OnAction(action);
//        }

//        public override bool OnMessage(GUIMessage message) {
//            return base.OnMessage(message);
//        }

//        protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType) {
//            if(control == controlFacade && actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM) {
//                if(currentState == CurrentState.CategoryList) {
//                    BuildSelectedCategory(controlFacade.SelectedListItem.Path);
//                }
//                if(currentState == CurrentState.SelectedCategory) {
//                    if(controlFacade.SelectedListItem.Path == "CollectLatestCheez") {
//                        ShowLatestCheez();
//                    } else if(controlFacade.SelectedListItem.Path == "CollectRandomCheez") {
//                        ShowRandomCheez();
//                    }
//                }
//            }
//            base.OnClicked(controlId, control, actionType);
//        }

//        private void ShowRandomCheez() {
//            currentState = CurrentState.SelectedRandom;
//            GUIControl.ClearControl(GetID, controlFacade.GetID);
//            controlFacade.View = GUIFacadeControl.ViewMode.Filmstrip;
//            CheezManager.CollectRandomCheez(currentCategory);
//        }

//        private void ShowLatestCheez() {
//            currentState = CurrentState.SelectedLatest;
//            GUIControl.ClearControl(GetID, controlFacade.GetID);
//            controlFacade.View = GUIFacadeControl.ViewMode.Filmstrip;
//            CheezManager.CollectLatestCheez(currentCategory);
//        }

//        private void ShowNotifyDialog(int timeOut, string notifyMessage) {
//            try {
//                GUIDialogNotify dialogMailNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
//                dialogMailNotify.TimeOut = timeOut;
//                dialogMailNotify.SetImage(GUIGraphicsContext.Skin + @"\Media\EndlessCheez_logo.png");
//                dialogMailNotify.SetHeading("EndlessCheez");
//                dialogMailNotify.SetText(notifyMessage);
//                dialogMailNotify.DoModal(GUIWindowManager.ActiveWindow);
//            } catch(Exception ex) {
//                Log.Error(ex);
//            }
//        }

//        private GUIListItem CreateGuiListItem(CheezItem item, bool folder) {
//            GUIListItem tmp = new GUIListItem(item.CheezTitle);
//            tmp.Label2 = "[" + item.CheezCreationDate + "]";
//            tmp.Path = item.CheezImagePath;
//            tmp.IsFolder = folder;
//            if(File.Exists(item.CheezImagePath)) {
//                tmp.IconImage = item.CheezImagePath;
//                tmp.IconImageBig = item.CheezImagePath;
//                tmp.ThumbnailImage = item.CheezImagePath;
//            }
//            tmp.OnItemSelected += new GUIListItem.ItemSelectedHandler(tmp_OnItemSelected);
//            return tmp;
//        }

//        void tmp_OnItemSelected(GUIListItem item, GUIControl parent) {
//            if(parent.GetID == controlFacade.GetID) {
//                if(controlFacade.SelectedListItemIndex == controlFacade.Count - 1) {
//                    if(currentState == CurrentState.SelectedLatest) {
//                        CheezManager.CollectLatestCheez(currentCategory);
//                    } else if(currentState == CurrentState.SelectedRandom) {
//                        CheezManager.CollectRandomCheez(currentCategory);
//                    }
//                }
//            }
//        }

//        private GUIListItem CreateGuiListFolder(string label, string imagePath, int itemId, string itemPath) {
//            GUIListItem guiListFolder = new GUIListItem(label);
//            guiListFolder.Path = itemPath;
//            guiListFolder.IsFolder = true;
//            guiListFolder.ItemId = itemId;
//            guiListFolder.IconImage = imagePath;
//            guiListFolder.IconImageBig = imagePath;
//            guiListFolder.ThumbnailImage = imagePath;
//            return guiListFolder;
//        }

//        private void FacadeAddItems(List<CheezItem> cheezItems) {
//            allCheezItems.AddRange(cheezItems);
//            foreach(CheezItem item in cheezItems) {
//                controlFacade.Add(CreateGuiListItem(item, false));
//            }
//        }
//    }
//}
