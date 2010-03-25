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

        void OnLatestCheezArrived(List<CheezItem> cheezItems);

        void OnRandomCheezArrived(List<CheezItem> cheezItems);

        void OnLocalCheezArrived(List<CheezItem> cheezItems);       
    }
}
