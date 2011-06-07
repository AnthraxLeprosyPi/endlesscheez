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
    public partial class EndlessCheezPlugin : ICheezCollector {

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

}
