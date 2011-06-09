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
            Dialogs.HideProgressDialog();
            Dialogs.ShowNotifyDialog(10, fail.ToString());            
            CollectLocalCheez(CheezManager.CurrentCheezSite);
        }

        public void OnCheezOperationProgress(int progressPercentage, string currentItem) {
            Dialogs.UpdateProgressDialog(currentItem, progressPercentage);         
        }


        public void OnLatestCheezFetched(CheezSite currentSite, List<CheezItem> cheezItems) {
            ProcessAndDisplayNewCheez(currentSite, cheezItems);
        }

        public void OnRandomCheezFetched(CheezSite currentSite, List<CheezItem> cheezItems) {
            ProcessAndDisplayNewCheez(currentSite, cheezItems);
        }

        public void OnLocalCheezFetched(CheezSite currentSite, List<CheezItem> cheezItems) {
            ProcessAndDisplayNewCheez(currentSite, cheezItems);
        }

        #endregion
    }
}
