using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Util;
using MediaPortal.Threading;


namespace EndlessCheez.Plugin {
    class CheezListItem : GUIListItem{

        CheezListItem(CheezSite cheezSite) : base(cheezSite.Name, cheezSite.Description, cheezSite.SiteId, true, null)  {
            base.RetrieveArt = false;
            base.OnRetrieveArt += new RetrieveCoverArtHandler(CheezListItem_OnRetrieveArt);           
        }

        //event which gets fired if the list,thumbnail, filmstrip or coverflow view needs the
        //coverart for the specified item
        void CheezListItem_OnRetrieveArt(GUIListItem item) {
            
           
        } 
    }
}
