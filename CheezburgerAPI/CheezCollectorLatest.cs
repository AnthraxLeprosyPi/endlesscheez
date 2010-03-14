using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;

namespace CheezburgerAPI {
    public class CheezCollectorLatest: CheezCollectorBase<CheezCollectorLatest> {

        private int currentStartIndex = 1;
        
        public int CurrentStartIndex
        {
            get { return currentStartIndex; }
            set { currentStartIndex = value; }
        }
       
        public override void CreateCheezCollection(CheezSite cheezSite, int fetchCount) {
            if(cheezSite != null) {
                _cheezOnlineResponse = CheezApiReader.ReadLatestCheez(cheezSite, currentStartIndex, fetchCount);
                if(_cheezOnlineResponse.Fail != null) {
                    ReportFail(_cheezOnlineResponse.Fail);
                } 
                base.CreateCheezCollection(cheezSite, fetchCount);
            } else {
                throw new ArgumentNullException("category", "CheezCollectorRandom doesn't permit null as category!");
            }
        }


        protected override void NewCheezCollected(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            currentStartIndex += _fetchCount;
            base.NewCheezCollected(sender, e);
        }
    }
}