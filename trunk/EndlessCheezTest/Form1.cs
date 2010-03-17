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
    public partial class Form1 : Form, ICheezConsumer {
        public Form1() {
            InitializeComponent();
            bool success = InitCheezManager(this, 1, Path.Combine(Application.StartupPath, "EndlessCheez"), true);
            GuiUpdateTextbox(CheezManager.CheezSites.Count.ToString() + " sites");
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
            CheezManager.CancelCheezCollection();
        }

        private void buttonStart_Click(object sender, EventArgs e) {
            Thread worker = new Thread(WorkerWork);
            worker.Start();

        }

        private void WorkerWork() {
            CheezManager.DeleteLocalCheez();
            CheezManager.CollectLocalCheez(CheezManager.GetCheezSiteByID(2));
            CheezManager.CollectLocalCheez(null);
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));
            CheezManager.CollectLatestCheez(CheezManager.GetCheezSiteByID(1));

        }

        #region ICheezConsumer Member

        public bool InitCheezManager(ICheezConsumer consumer, int fetchCount, string cheezRootFolder, bool createRootFolderStructure) {
            return CheezManager.InitCheezManager(consumer, fetchCount, cheezRootFolder, createRootFolderStructure);
        }
        
        public void CheezOperationFailed(CheezFail fail) {
            GuiUpdateTextbox(fail.ToString());
        }

        public void CheezOperationProgress(int progressPercentage, string currentItem) {
            if(progressPercentage >= 0 && progressPercentage <= 100) {
                GuiUpdateProgressbar(progressPercentage);
                if(currentItem != String.Empty) {
                    GuiUpdateTextbox(currentItem);
                }
            }
        }

        public void LatestCheezArrived(List<CheezItem> cheezItems) {
            GuiUpdateTextbox(cheezItems.Count.ToString() + " latest items collected!");
        }

        public void RandomCheezArrived(List<CheezItem> cheezItems) {
            throw new NotImplementedException();
        }

        public void LocalCheezArrived(List<CheezItem> cheezItems) {
            GuiUpdateTextbox(cheezItems.Count.ToString() + " local items collected!");
        }
        
        #endregion
    }
}