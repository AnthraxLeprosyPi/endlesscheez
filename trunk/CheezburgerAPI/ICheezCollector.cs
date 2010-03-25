using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;

namespace CheezburgerAPI {  
    public interface ICheezCollector {

        bool DeleteLocalCheez();

        bool CheckCheezConnection();

        void CollectLatestCheez(CheezSite cheezSite);

        void CollectRandomCheez(CheezSite cheezSite);

        void CollectLocalCheez(CheezSite cheezSite);

        void CancelCheezCollection();
    }
}
