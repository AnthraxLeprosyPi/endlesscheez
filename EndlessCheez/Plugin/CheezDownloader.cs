using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Threading;

namespace EndlessCheez.Plugin {
    static class CheezDownloader {

        private static ThreadPool ThreadPool { get; set; }

        static CheezDownloader() {
            ThreadPool = new ThreadPool(0, 10);
        }

        static void DownloadArtwork(CheezListItem listItem) {

        }

    }

    class DownloadQueueItem : IWork {


        public string Description {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public Exception Exception {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public void Process() {
            throw new NotImplementedException();
        }

        public WorkState State {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public System.Threading.ThreadPriority ThreadPriority {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }
    }
}

