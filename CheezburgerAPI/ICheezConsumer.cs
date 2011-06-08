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
    public interface ICheezConsumer {

        void OnCheezOperationFailed(CheezFail fail);

        void OnCheezOperationProgress(int progressPercentage, string currentItem);

        void OnLatestCheezFetched(CheezSite currentSite, List<CheezItem> cheezItems);

        void OnRandomCheezFetched(CheezSite currentSite, List<CheezItem> cheezItems);

        void OnLocalCheezFetched(CheezSite currentSite, List<CheezItem> cheezItems);
    }
}
