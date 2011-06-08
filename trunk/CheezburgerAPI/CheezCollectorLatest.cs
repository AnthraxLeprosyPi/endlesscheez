using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;
using System.Windows.Forms;

namespace CheezburgerAPI {
    internal class CheezCollectorLatest : CheezCollectorBase<CheezCollectorLatest> {

        private int _currentStartIndex = 1;
        private int _fetchCount;
        private object _locker = new object();

        public int CurrentStartIndex {
            get {
                return _currentStartIndex;
            }
            set {
                _currentStartIndex = value;
            }
        }

        public CheezSite CurrentCheezSite {
            get {
                return _currentCheezSite;
            }
        }

        public override void CreateCheezCollection(CheezSite cheezSite, int fetchCount) {
            _fetchCount = fetchCount;
            try{
                if(cheezSite != null && !cheezSite.Equals(_currentCheezSite)) {
                    _currentStartIndex = 1;
                    _currentCheezSite = cheezSite;
                }
                if (CurrentCheezSite == null) {
                    throw new ArgumentNullException();
                }
                _cheezOnlineResponse = CheezApiReader.ReadLatestCheez(CurrentCheezSite, _currentStartIndex, fetchCount);
                if(_cheezOnlineResponse.CheezFail != null) {
                    ReportFail(_cheezOnlineResponse.CheezFail);
                } else {
                    base.CreateCheezCollection(CurrentCheezSite, fetchCount);
                }
            }catch (Exception e){
                ReportFail(new CheezFail("No CheezSite specified!", "CheezCollectorLatest doesn't permit null as category!", e.ToString()));
            }
        }

        protected override void NewCheezCollected(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            _currentStartIndex += _fetchCount;
            base.NewCheezCollected(sender, e);
        }
    }
}