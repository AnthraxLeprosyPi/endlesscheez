using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace CheezburgerAPI {
    internal class CheezCollectorRandom: CheezCollectorBase<CheezCollectorRandom> {
        
        public override void CreateCheezCollection(CheezSite cheezSite, int fetchCount) {
            if(cheezSite != null) {
                _cheezOnlineResponse = CheezApiReader.ReadRandomCheez(cheezSite, fetchCount);
                if(_cheezOnlineResponse.CheezFail != null) {
                    ReportFail(_cheezOnlineResponse.CheezFail);
                } else {
                    base.CreateCheezCollection(cheezSite, fetchCount);
                }
            } else {
                ReportFail(new CheezFail("No CheezSite specified!", "CheezCollectorRandom doesn't permit null as category!","unknown"));
            }
        }        
    }
}
