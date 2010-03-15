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

        public Form1() {
            InitializeComponent();
        }

        void CheezManager_EventCheezFailed(Fail _fail) {
            throw new NotImplementedException();
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
                this.BeginInvoke(new GuiUpdateTextBoxDelegate(GuiUpdateTextbox), new object[] { text });
            } else {
                this.textBox1.AppendText(text);
                this.textBox1.AppendText(Environment.NewLine);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            CheezManager.CancelCheezCollection();
        }

        private void buttonStart_Click(object sender, EventArgs e) {
            CheezManager.InitCheezManager(1, Path.Combine(Application.StartupPath, "EndlessCheez"), true);
            _cheezSites = CheezManager.CheezSites;
            CheezManager.EventCheezFailed += new CheezManager.CheezFailedEventHandler(CheezManager_EventCheezFailed);
            CheezManager.EventLatestCheezArrived += new CheezManager.LatestCheezArrivedEventHandler(CheezManager_EventLatestCheezArrived);
            CheezManager.EventLocalCheezArrived += new CheezManager.LocalCheezArrivedEventHandler(CheezManager_EventLocalCheezArrived);
            CheezManager.EventRandomCheezArrived += new CheezManager.RandomCheezArrivedEventHandler(CheezManager_EventRandomCheezArrived);
            CheezManager.EventCheezProgress += new CheezManager.CheezProgressHandler(CheezManager_CheezProgress);
            GuiUpdateTextbox(_cheezSites.Count.ToString() + " sites");
            CheezManager.CollectCheez(CheezManager.CheezCollectionTypes.Latest, _cheezSites);
            CheezManager.CollectCheez();
        }
    }
}
