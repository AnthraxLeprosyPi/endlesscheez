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
    public partial class Main : ICheezCollector {       

        #region ICheezCollector Member

        public bool DeleteLocalCheez() {
            return CheezManager.DeleteLocalCheez();
        }

        public bool CheckCheezConnection() {
            return CheezManager.CheckCheezConnection();
        }

        public void CollectLatestCheez(CheezSite cheezSite) {
            ShowProgressInfo();
            Thread collectLatestCheez = new Thread(delegate() {
                CheezManager.CollectLatestCheez(cheezSite);
            });
            collectLatestCheez.Start();
        }

        public void CollectRandomCheez(CheezSite cheezSite) {
            ShowProgressInfo();
            Thread collectRandomCheez = new Thread(delegate() {
                CheezManager.CollectRandomCheez(cheezSite);
            });
            collectRandomCheez.Start();
        }

        public void CollectLocalCheez(CheezSite cheezSite) {
            ShowProgressInfo();
            Thread collectLocalCheez = new Thread(delegate() {
                CheezManager.CollectLocalCheez(cheezSite);
            });
            collectLocalCheez.Start();
        }

        public void CancelCheezCollection() {
            HideProgressInfo();
            CheezManager.CancelCheezCollection();
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
