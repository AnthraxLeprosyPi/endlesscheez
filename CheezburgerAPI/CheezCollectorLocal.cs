﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CheezburgerAPI {
    internal class CheezCollectorLocal : CheezCollectorBase<CheezCollectorLocal> {
        protected override void CollectCheez(object sender, System.ComponentModel.DoWorkEventArgs e) {
            _listCheezItems = new List<CheezItem>();
            string searchPatch = CheezManager.CheezRootFolder + @"\Cache";
            if (_currentCheezSite != null) {
                searchPatch = Path.Combine(searchPatch, _currentCheezSite.CheezSiteID);
            }
            try {
                List<string> folderFiles = Directory.GetFiles(searchPatch, "*.jpg", SearchOption.AllDirectories).ToList<string>();
                foreach (string filePath in folderFiles) {
                    string tmpTitle = String.Empty;
                    if (File.Exists(Path.ChangeExtension(filePath, ".txt"))) {
                        tmpTitle = System.IO.File.ReadAllText(Path.ChangeExtension(filePath, ".txt"));
                    } else {
                        tmpTitle = Path.GetFileNameWithoutExtension(filePath);
                    }
                    _listCheezItems.Add(new CheezItem(tmpTitle, filePath, File.GetCreationTime(filePath), _currentCheezSite));
                    base.backgroundCheezCollector.ReportProgress((int)((float)folderFiles.IndexOf(filePath) / (float)folderFiles.Count * 100), tmpTitle);
                }
            } catch (Exception ee) {
                ReportFail(new CheezFail(ee));
            }
        }

        internal int GetLocalCheezCount() {
            try {
                return Directory.GetFiles(CheezManager.CheezRootFolder, "*.jpg", SearchOption.AllDirectories).Length;
            } catch {
                return -1;
            }
        }
    }
}
