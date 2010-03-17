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
    internal abstract class CheezCollectorBase<T> where T : CheezCollectorBase<T> {
        private static object _lock = new object();

        protected BackgroundWorker backgroundCheezCollector;
        
        public delegate void CheezArrivedEventHandler(List<CheezItem> cheezItems);
        public event CheezArrivedEventHandler CheezArrived;

        public delegate void CheezFailedEventHandler(CheezFail _fail);
        public event CheezFailedEventHandler CheezFailed;

        public delegate void CheezProgressHandler(int progressPercentage, string currentItem);
        public event CheezProgressHandler CheezProgress;
        
        protected CheezSite _currentCheezSite;
        protected CheezAPI _cheezOnlineResponse;
        protected List<CheezItem> _listCheezItems;
        
        private static T _instance = null;
        public static T Instance {
            get {
                if(_instance == null) {
                    lock(_lock) {
                        if(_instance == null) {
                            object obj = FormatterServices.GetUninitializedObject(typeof(T));
                            if(obj != null) {
                                _instance = obj as T;
                                _instance.Init();
                            }
                        }
                    }
                } else {
                    _instance.Refresh();
                }
                return _instance;
            }
        }

        protected virtual void Init() {
            InitBackgroundCheezCollector();
        }

        protected virtual void Refresh() {
        }

        public virtual bool IsBusy {
            get {
                return (backgroundCheezCollector != null) ? backgroundCheezCollector.IsBusy : false;
            }
        }

        public virtual void CreateCheezCollection(CheezSite cheezSite, int fetchCount) {
            if(!backgroundCheezCollector.IsBusy) {
                _currentCheezSite = cheezSite;
                backgroundCheezCollector.RunWorkerAsync();
            }
        }

        public void CancelCheezCollection() {
            backgroundCheezCollector.CancelAsync();
        }

        private void InitBackgroundCheezCollector() {
            backgroundCheezCollector = new BackgroundWorker();
            backgroundCheezCollector.WorkerReportsProgress = true;
            backgroundCheezCollector.WorkerSupportsCancellation = true;
            backgroundCheezCollector.DoWork += new DoWorkEventHandler(CollectCheez);
            backgroundCheezCollector.ProgressChanged += new ProgressChangedEventHandler(CollectCheezProgress);
            backgroundCheezCollector.RunWorkerCompleted += new RunWorkerCompletedEventHandler(NewCheezCollected);
        }

        protected void ReportFail(CheezFail fail) {
            CheezFailed(fail);
        }

        protected virtual void CollectCheez(object sender, DoWorkEventArgs e){
             _listCheezItems = new List<CheezItem>();
            try {
                foreach(CheezAsset currentCheez in _cheezOnlineResponse.CheezAssets) {
                    if ((backgroundCheezCollector.CancellationPending == true)) {
                        e.Cancel = true;
                        break;
                    }
                    WebClient myWebClient = new WebClient();
                    string tmpFileName = string.Empty;
                    tmpFileName = Path.Combine(Path.Combine(CheezManager.CheezRootFolder, _currentCheezSite.CheezSiteID), Path.GetFileName(currentCheez.ImageUrl));
                    if (!File.Exists(tmpFileName)) {
                            myWebClient.DownloadFile(currentCheez.ImageUrl, tmpFileName);                        
                    }
                    System.IO.File.WriteAllText(Path.ChangeExtension(tmpFileName, ".txt"), currentCheez.Title);
                    _listCheezItems.Add(new CheezItem(currentCheez.Title, tmpFileName, DateTime.Parse(currentCheez.TimeStamp), currentCheez));
                    backgroundCheezCollector.ReportProgress((int)((float)_cheezOnlineResponse.CheezAssets.IndexOf(currentCheez) / (float)_cheezOnlineResponse.CheezAssets.Count * 100), "["+ currentCheez.AssetId +"] " + currentCheez.Title);
                }
            } catch (Exception ee) {
                ReportFail(new CheezFail(ee));
            }
        }

        protected virtual void NewCheezCollected(object sender, RunWorkerCompletedEventArgs e) {
            CheezProgress(100, String.Empty);
            CheezArrived(_listCheezItems);
        }

        private void CollectCheezProgress(object sender, ProgressChangedEventArgs e) {
            CheezProgress(e.ProgressPercentage,((string)e.UserState != null) ? (string)e.UserState : String.Empty);
        }
    }
}
