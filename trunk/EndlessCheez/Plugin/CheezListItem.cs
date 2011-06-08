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
            Utils.SetDefaultIcons(this);            
            base.RetrieveArt = false;
            base.Label3 = cheezSite.ShortDescription;
            SetIcons(cheezSite.SquareLogoPath);
        }

        internal CheezListItem(CheezItem cheezItem): base(cheezItem.CheezTitle) {
            Utils.SetDefaultIcons(this); 
            base.Label2 = String.Format("[{0}]", cheezItem.CheezCreationDateTime.ToShortDateString());
            base.Label3 = cheezItem.CheezAsset.FullText; 
            base.Path = cheezItem.CheezAsset.AssetId;
            base.DVDLabel = cheezItem.CheezAsset.ContentUrl;
            base.FileInfo = new FileInformation();
            base.FileInfo.CreationTime = cheezItem.CheezCreationDateTime;
            base.IsFolder = false;
            base.IsRemote = cheezItem.CheezAsset.AssetType.Contains("video");            
            base.RetrieveArt = false;
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

    internal class BackListItem : GUIListItem {
        public BackListItem() : base("..") { }
    }

}
