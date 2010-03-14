using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CheezburgerAPI;
using System.IO;
using System.Threading;

namespace EndlessCheezTest {
    public partial class Form1 : Form {
        List<CheezItem> _cheezItemsLocal = new List<CheezItem>();
        List<CheezItem> _cheezItemsLatest = new List<CheezItem>();
        List<CheezItem> _cheezItemsRandom = new List<CheezItem>();
        List<CheezSite> _cheezSites;
        Thread worker;

        public Form1() {
            InitializeComponent();
            CheezManager.InitCheezManager(3, Path.Combine(Application.StartupPath, "EndlessCheez"), true);
            _cheezSites = CheezManager.CheezSites;
            CheezManager.EventCheezFailed += new CheezManager.CheezFailedEventHandler(CheezManager_EventCheezFailed);
            CheezManager.EventLatestCheezArrived += new CheezManager.LatestCheezArrivedEventHandler(CheezManager_EventLatestCheezArrived);
            CheezManager.EventLocalCheezArrived += new CheezManager.LocalCheezArrivedEventHandler(CheezManager_EventLocalCheezArrived);
            CheezManager.EventRandomCheezArrived += new CheezManager.RandomCheezArrivedEventHandler(CheezManager_EventRandomCheezArrived);
            CheezManager.EventCheezProgress += new CheezManager.CheezProgressHandler(CheezManager_CheezProgress);           
            worker = new Thread(ThreadDownloader);
            worker.Start();
            
        }

        void CheezManager_EventCheezFailed(Fail _fail) {
            throw new NotImplementedException();
        }

        private void ThreadDownloader() {
            CheezManager.CheezCollectorLatest.CurrentStartIndex = 1;
            CheezManager.CollectCheez(CheezManager.CheezCollectionTypes.Latest, CheezManager.GetCheezSiteByID(1));
            CheezManager.CollectCheez(CheezManager.CheezCollectionTypes.Latest, CheezManager.GetCheezSiteByID(2));
            CheezManager.CollectCheez(CheezManager.CheezCollectionTypes.Latest, CheezManager.GetCheezSiteByID(3));
            CheezManager.CollectCheez(CheezManager.CheezCollectionTypes.Latest, CheezManager.GetCheezSiteByID(4));
            CheezManager.CollectCheez(CheezManager.CheezCollectionTypes.Latest, CheezManager.GetCheezSiteByID(6));
            GuiUpdateTextbox(_cheezSites.Count.ToString() + " sites");
            foreach (CheezSite site in _cheezSites) {
                GuiUpdateTextbox(site.Url + " (" + site.SiteId + ")");
                CheezManager.CollectCheez(CheezManager.CheezCollectionTypes.Latest, site);
                CheezManager.CollectCheez(CheezManager.CheezCollectionTypes.Local, site);
            }
            CheezManager.CollectCheez(CheezManager.CheezCollectionTypes.Local, null);
        }

        void CheezManager_EventLatestCheezArrived(List<CheezItem> cheezItems) {
            _cheezItemsLatest = cheezItems;
            GuiUpdateTextbox(_cheezItemsLatest.Count.ToString() + " latest items collected!");

        }

        void CheezManager_EventRandomCheezArrived(List<CheezItem> cheezItems) {
            throw new NotImplementedException();
        }

        void CheezManager_EventLocalCheezArrived(List<CheezItem> cheezItems) {
            _cheezItemsLocal = cheezItems;
            GuiUpdateTextbox(_cheezItemsLocal.Count.ToString() + " local items collected!");
        }

        void CheezManager_CheezProgress(int progressPercentage, string currentItem) {
            if (progressPercentage >= 0 && progressPercentage <= 100) {
                GuiUpdateProgressbar(progressPercentage);
                if (currentItem != String.Empty) {
                    GuiUpdateTextbox(currentItem);
                }
            }
        }

        public delegate void GuiUpdateProgressbarDelegate(int value);

        public void GuiUpdateProgressbar(int value) {
            if (this.InvokeRequired) {
                this.Invoke(new GuiUpdateProgressbarDelegate(GuiUpdateProgressbar), new object[] { value });
            } else {
                this.progressBar1.Value = value;
            }
        }

        public delegate void GuiUpdateTextBoxDelegate(string text);

        public void GuiUpdateTextbox(string text) {
            if (this.InvokeRequired) {
                this.Invoke(new GuiUpdateTextBoxDelegate(GuiUpdateTextbox), new object[] { text });
            } else {
                this.textBox1.AppendText(text);
                this.textBox1.AppendText(Environment.NewLine);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            worker.Abort();
            CheezManager.CancelCheezCollection();
        }
    }
}
