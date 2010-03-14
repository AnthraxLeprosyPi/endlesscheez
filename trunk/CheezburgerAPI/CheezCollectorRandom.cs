using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace CheezburgerAPI {
    public class CheezCollectorRandom: CheezCollectorBase<CheezCollectorRandom> {
        
        public override void CreateCheezCollection(CheezSite cheezSite, int fetchCount) {
            if(cheezSite != null) {
                _cheezOnlineResponse = CheezApiReader.ReadRandomCheez(cheezSite, fetchCount);                         
                if(_cheezOnlineResponse.Fail != null) {
                    ReportFail(_cheezOnlineResponse.Fail);
                }
                base.CreateCheezCollection(cheezSite, fetchCount);
            } else {
                throw new ArgumentNullException("category","CheezCollectorRandom doesn't permit null as category!");
            }
        }        
    }
}
