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


namespace EndlessCheez {
    public partial class EndlessCheezConfig : MPConfigForm {

        public EndlessCheezConfig() {
            try {
                InitializeComponent();
                Load += new EventHandler(EndlessCheezConfig_Load);
            }
            catch (Exception ex) {
                //log errors here
            }
        }

        void EndlessCheezConfig_Load(object sender, EventArgs e) {
            //try {
            //    using (MediaPortal.Profile.Settings xmlReader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"))) {
            //        textBoxHost.Text = xmlReader.GetValueAsString("EndlessCheez", "#EndlessCheez.Host", "imap.gmail.com");
            //        textBoxCacheFolder.Text = xmlReader.GetValueAsString("EndlessCheez", "#EndlessCheez.CacheFile", "");
            //        textBoxUsername.Text = xmlReader.GetValueAsString("EndlessCheez", "#EndlessCheez.Username", "");
            //        textBoxPassword.Text = xmlReader.GetValueAsString("EndlessCheez", "#EndlessCheez.Password", "");
            //        checkBoxNotifyOnNewMail.Checked = xmlReader.GetValueAsBool("EndlessCheez", "#EndlessCheez.NotifyOnNewMail", true);
            //        checkBoxSsl.Checked = xmlReader.GetValueAsBool("EndlessCheez", "#EndlessCheez.UseSSL", true);
            //        numericUpDownCheckInterval.Value = xmlReader.GetValueAsInt("EndlessCheez", "#EndlessCheez.CheckInterval", 15);
            //        numericUpDownNotifyTimeOut.Value = xmlReader.GetValueAsInt("EndlessCheez", "#EndlessCheez.NotifyTimeOut", 3);
            //    }
            //}
            //catch (Exception ex) {
            //    Log.Error(ex);
            //}
        }

     
    }
}