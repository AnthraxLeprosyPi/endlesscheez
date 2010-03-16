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
        public Form1() {
            InitializeComponent();
            CheezManager.EventCheezFailed += new CheezManager.CheezFailedEventHandler(CheezManager_EventCheezFailed);
            CheezManager.EventLatestCheezArrived += new CheezManager.CheezArrivedLatestEventHandler(CheezManager_EventLatestCheezArrived);
            CheezManager.EventLocalCheezArrived += new CheezManager.CheezArrivedLocalEventHandler(CheezManager_EventLocalCheezArrived);
            CheezManager.EventRandomCheezArrived += new CheezManager.CheezArrivedRandomEventHandler(CheezManager_EventRandomCheezArrived);
            CheezManager.EventCheezProgress += new CheezManager.CheezProgressHandler(CheezManager_CheezProgress);
            bool success = CheezManager.InitCheezManager(1, Path.Combine(Application.StartupPath, "EndlessCheez"), true);
            GuiUpdateTextbox(CheezManager.CheezSites.Count.ToString() + " sites");              
        }

        void CheezManager_EventCheezFailed(Fail _fail) {
            GuiUpdateTextbox(_fail.ToString());
        }

        void CheezManager_EventLatestCheezArrived(List<CheezItem> cheezItems) {
            GuiUpdateTextbox(cheezItems.Count.ToString() + " latest items collected!");
        }

        void CheezManager_EventRandomCheezArrived(List<CheezItem> cheezItems) {
            throw new NotImplementedException();
        }

        void CheezManager_EventLocalCheezArrived(List<CheezItem> cheezItems) {
            GuiUpdateTextbox(cheezItems.Count.ToString() + " local items collected!");
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
                this.BeginInvoke(new GuiUpdateProgressbarDelegate(GuiUpdateProgressbar), new object[] { value });
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
            Thread worker = new Thread(WorkerWork);
            worker.Start();

        }

        private void WorkerWork() {
            CheezManager.CollectLocalCheez(CheezManager.GetCheezSiteByID(2));
            CheezManager.CollectLocalCheez(null);
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(2));
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(2));            
        }

    }
}
