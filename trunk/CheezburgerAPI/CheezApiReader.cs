using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Net;
using System.Xml;

namespace CheezburgerAPI {

    internal enum CheezApiRequestType {
        Featured,
        Random,
        Hai
    }

    internal static class CheezApiReader {

        private const String _cheezburgerHaiUri = @"http://api.cheezburger.com/xml/hai";
        private const String _cheezburgerSitesUri = @"http://api.cheezburger.com/xml/site";
        private const String _cheezburgerContentUri = @"http://api.cheezburger.com/xml/site/{CheezSiteID}/{RequestType}"; // {StartIndex}/{ItemCount}


        private static CheezApiResponse ReadCheezAPI(string streamUri) {
            try {
                WebClient client = new WebClient();
                client.Headers.Add("DeveloperKey", "78b2c670-48ee-410b-a882-c546278c99a3");
                client.Headers.Add("ClientID", "2055");
               
                Stream webStream = client.OpenRead(streamUri);
                string webResponseString = string.Empty;
                using (StreamReader reader = new StreamReader(webStream)) {
                    webResponseString = reader.ReadToEnd().Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<CheezApiResponse>\r\n");
                    webResponseString += "</CheezApiResponse>\r\n";
                }
                using (XmlSanitizingStream reader = new XmlSanitizingStream(new MemoryStream(UTF8Encoding.Default.GetBytes(webResponseString)))) {
                    System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(CheezApiResponse));
                    return (CheezApiResponse)xSerializer.Deserialize(reader);
                }
            } catch (Exception e) {
                return new CheezApiResponse(new CheezFail(e));
            }
        }

        public static CheezAPI ReadHai() {
            return new CheezAPI(CheezApiRequestType.Hai, ReadCheezAPI(_cheezburgerHaiUri));
        }

        private static CheezAPI ReadCheez(CheezApiRequestType reqestType, CheezSite cheezSite, int startIndex, int itemCount) {
            if (startIndex < 1) {
                startIndex = 1;
            }
            //Cheezburger API permits retrieval of maximum 100 lols           
            if (itemCount > 0) {
                if (itemCount > 100) {
                    itemCount = 100;
                } else if (itemCount < 1) {
                    itemCount = 1;
                }
            }
            string requestUri = cheezSite.SiteId;
            switch (reqestType) {
                case CheezApiRequestType.Featured:
                    requestUri += String.Format("/featured/{0}/{1}", startIndex, itemCount);
                    break;
                case CheezApiRequestType.Random:
                    requestUri += String.Format("featured/random/{0}", itemCount);
                    break;
                case CheezApiRequestType.Hai:
                default:
                    break;
            }
            CheezApiResponse cheezApiResponse = ReadCheezAPI(requestUri);
            return new CheezAPI(reqestType, cheezApiResponse);
        }

        public static CheezAPI ReadRandomCheez(CheezSite cheezSite, int itemCount) {
            return ReadCheez(CheezApiRequestType.Random, cheezSite, 0, itemCount);
        }

        public static CheezAPI ReadLatestCheez(CheezSite cheezSite, int startIndex, int itemCount) {
            return ReadCheez(CheezApiRequestType.Featured, cheezSite, startIndex, itemCount);
        }

        public static List<CheezSite> ReadCheezSites() {
            try {
                WebClient client = new WebClient();
                client.Headers.Add("DeveloperKey", "78b2c670-48ee-410b-a882-c546278c99a3");
                client.Headers.Add("ClientID", "2055");
                Stream sitesStream;
                try {
                    sitesStream = client.OpenRead(_cheezburgerSitesUri);
                } catch {
                    sitesStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CheezburgerAPI.Resources.CheezSites.xml");
                }
                string webResponseString = string.Empty;
                CheezSites cheezSites;
                using (XmlSanitizingStream reader = new XmlSanitizingStream(sitesStream)) {
                    System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(CheezSites));
                    cheezSites = (CheezSites)xSerializer.Deserialize(reader);
                }
                List<CheezSite> tmpList = cheezSites.Items[0].Site.ToList();
                tmpList.Sort();                
                string tmpPath = Path.Combine(CheezManager.CheezRootFolder, "Logos");
                if (!Directory.Exists(tmpPath)) {
                    Directory.CreateDirectory(tmpPath);
                }
                CheezLogo = Path.Combine(tmpPath, "cheeznet80.png");
                foreach (CheezSite site in tmpList) {
                    string localFile = Path.Combine(tmpPath, Path.GetFileName(site.SquareLogoUrl));
                    if(File.Exists(localFile)){
                        site.SquareLogoPath = localFile;
                    }else if (!String.IsNullOrEmpty(site.SquareLogoUrl)) {
                        try {
                            client.DownloadFile(site.SquareLogoUrl, localFile);
                            site.SquareLogoPath = localFile;
                        } catch {

                        }
                    } else if (File.Exists(CheezLogo)) {
                        site.SquareLogoPath = CheezLogo;
                    } else {
                        site.SquareLogoPath = string.Empty;
                    }
                }
                tmpList.RemoveAll(x => x.SiteCategory.Equals("STORE & CO."));
                return tmpList;
            } catch (Exception e) {
                throw e;
            }
        }

        public static string CheezLogo { get; private set; }
    }

   

    internal class CheezAPI {
        private CheezApiRequestType _requestType;
        private CheezApiResponse _cheezApiResponse;

        public CheezAPI(CheezApiRequestType requestType, CheezApiResponse cheezApiResponse) {
            this._requestType = requestType;
            this._cheezApiResponse = cheezApiResponse;
        }

        public CheezApiRequestType RequestType {
            get {
                return _requestType;
            }
        }

        public List<CheezAsset> CheezAssets {
            get {
                return _cheezApiResponse.CheezAssets;
            }
        }

        public CheezFail CheezFail {
            get {
                return (CheezFail)_cheezApiResponse;
            }
        }

        public CheezHai CheezHai {
            get {
                return (CheezHai)_cheezApiResponse;
            }
        }
    }
}
