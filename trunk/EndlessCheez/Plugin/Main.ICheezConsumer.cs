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
    public partial class Main : ICheezConsumer {

        #region ICheezConsumer Member

        public void OnCheezOperationFailed(CheezFail fail) {
            HideProgressInfo();
            ShowNotifyDialog(10, fail.ToString());
            DisplayCheezSitesOverview();
        }

        public void OnCheezOperationProgress(int progressPercentage, string currentItem) {
            if (ctrlProgressBar != null) {
                if (progressPercentage >= 0 && progressPercentage <= 100) {
                    ctrlProgressBar.Percentage = progressPercentage;
                    if (String.IsNullOrEmpty(currentItem)) {
                        currentItem = " ";
                    }
                    GUIPropertyManager.SetProperty("#EndlessCheez.CurrentItem", String.Format("{0} ({1}%)", currentItem, progressPercentage.ToString()));
                }
            }
            this.Process();
        }


        public void OnLatestCheezFetched(List<CheezItem> cheezItems) {
            ProcessAndDisplayNewCheez(cheezItems);
        }

        public void OnRandomCheezFetched(List<CheezItem> cheezItems) {
            ProcessAndDisplayNewCheez(cheezItems);
        }

        public void OnLocalCheezFetched(List<CheezItem> cheezItems) {
            ProcessAndDisplayNewCheez(cheezItems);
        }

        #endregion
    }
}
