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
    public partial class Form1 : Form, ICheezConsumer, ICheezCollector {

        Thread worker;
        public Form1() {
            InitializeComponent();
            bool success = InitCheezManager(this, 3, Path.Combine(Application.StartupPath, "EndlessCheez"), true);
            GuiUpdateTextbox(CheezManager.CheezSites.Count.ToString() + " sites");
            FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            CancelCheezCollection();
        }

        public delegate void GuiUpdateProgressbarDelegate(int value);

        public void GuiUpdateProgressbar(int value) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new GuiUpdateProgressbarDelegate(GuiUpdateProgressbar), new object[] { value });
            } else {
                this.progressBar1.Value = value;
            }
        }

        public delegate void GuiUpdateTextBoxDelegate(string text);

        public void GuiUpdateTextbox(string text) {
            if(this.InvokeRequired) {
                this.BeginInvoke(new GuiUpdateTextBoxDelegate(GuiUpdateTextbox), new object[] { text });
            } else {
                this.textBox1.AppendText(text);
                this.textBox1.AppendText(Environment.NewLine);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            CancelCheezCollection();
        }

        private void buttonStart_Click(object sender, EventArgs e) {
            worker = new Thread(WorkerWork);
            worker.Start();
        }

        private void WorkerWork() {
            foreach(CheezSite site in CheezManager.CheezSites) {
                GuiUpdateTextbox(site.ToString());

            }
            CheezManager.DeleteLocalCheez();
            CheezManager.CollectLocalCheez(CheezManager.GetCheezSiteByID(2));
            CheezManager.CollectLocalCheez(null);
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
           CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
        }

        #region ICheezConsumer Member

        public bool InitCheezManager(ICheezConsumer consumer, int fetchCount, string cheezRootFolder, bool createRootFolderStructure) {
            return CheezManager.InitCheezManager(consumer, fetchCount, cheezRootFolder, createRootFolderStructure);
        }

        public void OnCheezOperationFailed(CheezFail fail) {
            GuiUpdateTextbox(fail.ToString());
        }

        public void OnCheezOperationProgress(int progressPercentage, string currentItem) {
            if(progressPercentage >= 0 && progressPercentage <= 100) {
                GuiUpdateProgressbar(progressPercentage);
                if(currentItem != String.Empty) {
                    GuiUpdateTextbox(currentItem);
                }
            }
        }

        public void OnLatestCheezFetched(List<CheezItem> cheezItems) {
            GuiUpdateTextbox(cheezItems.Count.ToString() + " latest items collected!");
        }

        public void OnRandomCheezFetched(List<CheezItem> cheezItems) {
            throw new NotImplementedException();
        }

        public void OnLocalCheezFetched(List<CheezItem> cheezItems) {
            GuiUpdateTextbox(cheezItems.Count.ToString() + " local items collected!");
        }

        #endregion

        #region ICheezCollector Member

        public bool CheckCheezConnection() {
            return CheezManager.CheckCheezConnection();
        }

        public void CollectLatestCheez(CheezSite cheezSite) {
            CheezManager.CollectLatestCheez(cheezSite);
        }

        public void CollectRandomCheez(CheezSite cheezSite) {
            CheezManager.CollectRandomCheez(cheezSite);
        }

        public void CollectLocalCheez(CheezSite cheezSite) {
            CheezManager.CollectLocalCheez(cheezSite);
        }

        public bool DeleteLocalCheez() {
            return CheezManager.DeleteLocalCheez();
        }

        public void CancelCheezCollection() {
            if(worker != null && worker.IsAlive) {
                worker.Abort();
            }
            CheezManager.CancelCheezCollection();
        }

        #endregion

        private void button1_Click_1(object sender, EventArgs e) {

        }



    }
}