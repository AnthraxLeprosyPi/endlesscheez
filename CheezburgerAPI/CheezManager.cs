using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CheezburgerAPI {
    public static class CheezManager {

        private static string _cheezRootFolder = Path.Combine(System.Environment.SpecialFolder.MyDocuments.ToString(), "EndlessCheez");
        private static int _fetchCount = 1;
        private static List<CheezSite> _cheezSites = null;

        private static AutoResetEvent _cheezBusyEvent = new AutoResetEvent(true);
       
        public enum CheezCollectionTypes {
            Local,
            Random,
            Latest
        }

        public delegate void LocalCheezArrivedEventHandler(List<CheezItem> cheezItems);
        public static event LocalCheezArrivedEventHandler EventLocalCheezArrived;

        public delegate void RandomCheezArrivedEventHandler(List<CheezItem> cheezItems);
        public static event RandomCheezArrivedEventHandler EventRandomCheezArrived;

        public delegate void LatestCheezArrivedEventHandler(List<CheezItem> cheezItems);
        public static event LatestCheezArrivedEventHandler EventLatestCheezArrived;

        public delegate void CheezFailedEventHandler(Fail _fail);
        public static event CheezFailedEventHandler EventCheezFailed;

        public delegate void CheezProgressHandler(int progressPercentage, string currentItem);
        public static event CheezProgressHandler EventCheezProgress;

        static CheezManager() {
            CheezCollectorLocal.Instance.CheezArrived += new CheezCollectorBase<CheezCollectorLocal>.CheezArrivedEventHandler(Local_CheezArrived);
            CheezCollectorLocal.Instance.CheezFailed += new CheezCollectorBase<CheezCollectorLocal>.CheezFailedEventHandler(Global_CheezFailed);
            CheezCollectorLocal.Instance.CheezProgress += new CheezCollectorBase<CheezCollectorLocal>.CheezProgressHandler(Global_CheezProgress);
            CheezCollectorRandom.Instance.CheezArrived += new CheezCollectorBase<CheezCollectorRandom>.CheezArrivedEventHandler(Random_CheezArrived);
            CheezCollectorRandom.Instance.CheezFailed += new CheezCollectorBase<CheezCollectorRandom>.CheezFailedEventHandler(Global_CheezFailed);
            CheezCollectorRandom.Instance.CheezProgress += new CheezCollectorBase<CheezCollectorRandom>.CheezProgressHandler(Global_CheezProgress);
            CheezCollectorLatest.Instance.CheezArrived += new CheezCollectorBase<CheezCollectorLatest>.CheezArrivedEventHandler(Latest_CheezArrived);
            CheezCollectorLatest.Instance.CheezFailed += new CheezCollectorBase<CheezCollectorLatest>.CheezFailedEventHandler(Global_CheezFailed);
            CheezCollectorLatest.Instance.CheezProgress += new CheezCollectorBase<CheezCollectorLatest>.CheezProgressHandler(Global_CheezProgress);
        }

        public static bool InitCheezManager(int fetchCount, string cheezRootFolder, bool createRootFolderStructure) {
            _fetchCount = fetchCount;
            _cheezRootFolder = cheezRootFolder;
            _cheezBusyEvent.Set();
            if(!Directory.Exists(_cheezRootFolder)) {
                try {
                    Directory.CreateDirectory(_cheezRootFolder);
                } catch {
                }
            }
            _cheezSites = CheezApiReader.ReadCheezSites();
            if(createRootFolderStructure) {
                CreateCheezFolderStructure(_cheezSites);
            }
            return CheckCheezConnection();
        }

        private static void Global_CheezProgress(int progressPercentage, string currentItem) {
            EventCheezProgress(progressPercentage, currentItem);
        }

        private static void Global_CheezFailed(Fail fail) {
            EventCheezFailed(fail);
            //_cheezBusyEvent.Set();
        }

        private static void Random_CheezArrived(List<CheezItem> cheezItems) {
            EventRandomCheezArrived(cheezItems);
            _cheezBusyEvent.Set();
        }

        private static void Local_CheezArrived(List<CheezItem> cheezItems) {
            EventLocalCheezArrived(cheezItems);
            _cheezBusyEvent.Set();
        }

        static void Latest_CheezArrived(List<CheezItem> cheezItems) {
            EventLatestCheezArrived(cheezItems);
            _cheezBusyEvent.Set();
        }

        public static string CheezRootFolder {
            get {
                return _cheezRootFolder;
            }
        }

        public static bool IsBusy {
            get {
                return (CheezCollectorLocal.Instance.IsBusy || CheezCollectorRandom.Instance.IsBusy || CheezCollectorLatest.Instance.IsBusy);
            }
        }

        public static int FetchCount {
            get {
                return _fetchCount;
            }
        }

        public static List<CheezSite> CheezSites {
            get {
                return _cheezSites;
            }
        }

        public static CheezSite GetCheezSiteByID(string siteID) {
            if(_cheezSites != null) {
                foreach(CheezSite currentSite in _cheezSites) {
                    if(currentSite.SiteId.EndsWith("/" + siteID)) {
                        return currentSite;
                    }
                }
            }
            return null;
        }
        public static CheezSite GetCheezSiteByID(int siteID) {
            return GetCheezSiteByID(siteID.ToString());
        }

        public static bool CheckCheezConnection() {
            return (CheezApiReader.ReadHai() != null);
        }

        private static bool CreateCheezFolderStructure(List<CheezSite> cheezSites) {
            try {
                foreach(CheezSite cheezSite in cheezSites) {
                    if(!Directory.Exists(Path.Combine(_cheezRootFolder, cheezSite.CheezSiteID))) {
                        Directory.CreateDirectory(Path.Combine(_cheezRootFolder, cheezSite.CheezSiteID));
                    }
                }
                return true;
            } catch {
                return false;
            }
        }

        public static bool DeleteAllTheCheez() {
            bool allOk = true;
            foreach(string filePath in Directory.GetFiles(_cheezRootFolder, "*.*", SearchOption.AllDirectories)) {
                try {
                    File.Delete(filePath);
                } catch {
                    allOk = false;
                }
            }
            return allOk;
        }

        public static void CollectCheez(CheezCollectionTypes cheezType, CheezSite cheezSite) {
            _cheezBusyEvent.Reset();
            switch(cheezType) {
                case CheezCollectionTypes.Local:
                    CheezCollectorLocal.Instance.CreateCheezCollection(cheezSite, _fetchCount);
                    break;
                case CheezCollectionTypes.Latest:
                    CheezCollectorLatest.Instance.CreateCheezCollection(cheezSite, _fetchCount);
                    break;
                case CheezCollectionTypes.Random:
                    CheezCollectorRandom.Instance.CreateCheezCollection(cheezSite, _fetchCount);
                    break;
            }
            _cheezBusyEvent.WaitOne(60000, false);
        }

        public static void CancelCheezCollection() {
            if(IsBusy) {
                CheezCollectorLocal.Instance.CancelCheezCollection();
                CheezCollectorLatest.Instance.CancelCheezCollection();
                CheezCollectorRandom.Instance.CancelCheezCollection();
            }
        }
    }

    public class CheezItem {
        private string _cheezImagePath;
        private string _cheezTitle;
        private DateTime _cheezCreationTime;
        private CheezAsset _cheezAsset;

        public CheezItem(string cheezTitle, string cheezImagePath, DateTime cheezCreationTime) {
            this._cheezTitle = cheezTitle;
            this._cheezImagePath = cheezImagePath;
            this._cheezCreationTime = cheezCreationTime;
            this._cheezAsset = null;
        }

        public CheezItem(string cheezTitle, string cheezImagePath, DateTime cheezCreationTime, CheezAsset cheezAsset) {
            this._cheezTitle = cheezTitle;
            this._cheezImagePath = cheezImagePath;
            this._cheezCreationTime = cheezCreationTime;
            this._cheezAsset = cheezAsset;
        }

        public string CheezImagePath {
            get {
                return _cheezImagePath;
            }
        }

        public string CheezCreationDate {
            get {
                return _cheezCreationTime.ToShortDateString();
            }
        }

        public CheezAsset CheezAsset {
            get {
                return _cheezAsset;
            }
        }

        public string CheezTitle {
            get {
                return _cheezTitle;
            }
        }
    }
}
