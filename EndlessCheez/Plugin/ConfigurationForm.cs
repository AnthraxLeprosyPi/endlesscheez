#region Copyright (C) 2005-2008 Team MediaPortal

/* 
 *      Copyright (C) 2005-2008 Team MediaPortal
 *      http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */
#endregion

using System;
using MediaPortal.UserInterface.Controls;
using MediaPortal.GUI.Library;
using System.Windows.Forms;


namespace EndlessCheez.Plugin {
    public partial class ConfigurationForm : MPConfigForm {

        public ConfigurationForm() {            
            try {
                InitializeComponent();
                Load += new EventHandler(ConfigurationForm_Load);
                FormClosing += new System.Windows.Forms.FormClosingEventHandler(ConfigurationForm_FormClosing);
            }
            catch (Exception ex) {
                Log.Error(ex);
            }
        }

        void ConfigurationForm_Load(object sender, EventArgs e) {
            Settings.Load();
            textBoxCheezRootFolder.Text = Settings.CheezRootFolder;
            numericUpDownFetchCount.Value = Settings.FetchCount;
            checkBoxDeleteOnExit.Checked = Settings.DeleteLocalCheezOnExit;
        }

        void ConfigurationForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {
            Settings.CheezRootFolder = textBoxCheezRootFolder.Text;
            Settings.FetchCount = (int)numericUpDownFetchCount.Value;
            Settings.DeleteLocalCheezOnExit = checkBoxDeleteOnExit.Checked;
            Settings.Save();
        }

        private void button1_Click(object sender, EventArgs e) {
            FolderBrowserDialog dlgFolderBrowser = new FolderBrowserDialog();
            dlgFolderBrowser.Description = "Local download path for cheezy pictures...";
            dlgFolderBrowser.SelectedPath = Settings.CheezRootFolder;
            if (dlgFolderBrowser.ShowDialog(this) == DialogResult.OK) {
                Settings.CheezRootFolder = dlgFolderBrowser.SelectedPath;
            } else {
                Log.Debug("No folder selected");
            }
        }

     
    }
}