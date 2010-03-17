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

        bool InitCheezManager(ICheezConsumer consumer, int fetchCount, string cheezRootFolder, bool createRootFolderStructure);

        void CheezOperationFailed(CheezFail fail);

        void CheezOperationProgress(int progressPercentage, string currentItem);

        void LatestCheezArrived(List<CheezItem> cheezItems);

        void RandomCheezArrived(List<CheezItem> cheezItems);

        void LocalCheezArrived(List<CheezItem> cheezItems);
    }
}
