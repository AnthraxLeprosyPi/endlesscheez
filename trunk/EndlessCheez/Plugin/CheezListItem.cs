using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Util;
using MediaPortal.Threading;
using CheezburgerAPI;


namespace EndlessCheez.Plugin {
    internal class CheezListItem : GUIListItem{

        internal CheezListItem(CheezSite cheezSite)
            : base(cheezSite.Name, cheezSite.Description, cheezSite.SiteId, true, null) {                
            base.RetrieveArt = false;
            base.Label3 = cheezSite.ShortDescription;
            LastSelectedIndex = 1;
            SetIcons(cheezSite.SquareLogoPath);
        }

        public int LastSelectedIndex { get; set; }

        internal CheezListItem(CheezItem cheezItem): base(cheezItem.CheezTitle) {            
            base.Label2 = String.Format("[{0}]", cheezItem.CheezCreationDateTime.ToShortDateString());
            if (cheezItem.CheezAsset != null) {
                base.Label3 = cheezItem.CheezAsset.FullText;
                base.Path = cheezItem.CheezAsset.AssetId;
                base.DVDLabel = cheezItem.CheezAsset.ContentUrl;
                base.IsRemote = cheezItem.CheezAsset.AssetType.Contains("Video");
            }
            LastSelectedIndex = 1;
            base.FileInfo = new FileInformation();
            base.FileInfo.CreationTime = cheezItem.CheezCreationDateTime;
            base.RetrieveArt = false;
                base.IsFolder = false;
            SetIcons(cheezItem.CheezImagePath);
        }

        private void SetIcons(string localImagePath) {
            IconImage = localImagePath;
            IconImageBig = localImagePath;
            ThumbnailImage = localImagePath;
            RetrieveArt = true;
            RefreshCoverArt();
        }
    }
}
