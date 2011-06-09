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
            Dialogs.ShowProgressDialog(cheezSite.Name);
            Thread collectLatestCheez = new Thread(delegate() {
                CheezManager.CollectLatestCheez(cheezSite);
            });
            collectLatestCheez.Start();
        }

        public void CollectRandomCheez(CheezSite cheezSite) {
            Dialogs.ShowProgressDialog(cheezSite.Name);
            Thread collectRandomCheez = new Thread(delegate() {
                CheezManager.CollectRandomCheez(cheezSite);
            });
            collectRandomCheez.Start();
        }

        public void CollectLocalCheez(CheezSite cheezSite) {
            Dialogs.ShowProgressDialog(cheezSite.Name);
            Thread collectLocalCheez = new Thread(delegate() {
                CheezManager.CollectLocalCheez(cheezSite);
            });
            collectLocalCheez.Start();
        }

        public void CancelCheezCollection() {
            Dialogs.HideProgressDialog();
            CheezManager.CancelCheezCollection();
        }

        #endregion       
    }
}
