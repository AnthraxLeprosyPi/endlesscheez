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
using System.Collections;
using MediaPortal.Player;
using System.Linq;

namespace EndlessCheez {
    public partial class EndlessCheezPlugin : ICheezConsumer {
   
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
