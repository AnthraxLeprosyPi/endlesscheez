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

    public enum CheezApiRequestType {
        Featured,
        Random
    }

    static class CheezApiReader {

        private const String _cheezburgerHaiUri = @"http://api.cheezburger.com/xml/hai";
        private const String _cheezburgerSitesUri = @"http://api.cheezburger.com/xml/site";
        private const String _cheezburgerContentUri = @"http://api.cheezburger.com/xml/site/{CheezSiteID}/{RequestType}"; // {StartIndex}/{ItemCount}


        private static CheezApiResponse ReadCheezAPI(string streamUri) {
            try {
                WebClient client = new WebClient();
                Stream webStream = client.OpenRead(streamUri);
                string webResponseString = string.Empty;
                using(StreamReader reader = new StreamReader(webStream)) {
                    webResponseString = reader.ReadToEnd().Replace("<?xml version=\"1.0\"?>\r\n", "<?xml version=\"1.0\"?>\r\n<CheezApiResponse>\r\n");
                    webResponseString += "</CheezApiResponse>\r\n";
                }
                using(XmlSanitizingStream reader = new XmlSanitizingStream(new MemoryStream(UTF8Encoding.Default.GetBytes(webResponseString)))) {
                    System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(CheezApiResponse));
                    return (CheezApiResponse)xSerializer.Deserialize(reader);
                }
            } catch(Exception e) {
                throw e;
            }
        }

        public static Hai ReadHai() {
            return (Hai)ReadCheezAPI(_cheezburgerHaiUri);
        }

        private static CheezAPI ReadCheez(CheezApiRequestType reqestType, CheezSite cheezSite, int startIndex, int itemCount) {
            string requestUri = cheezSite.SiteId.ToString();
            requestUri += "/" + reqestType.ToString();            
            if(startIndex < 1) {
                startIndex = 1;
            }
            requestUri += ("/" + startIndex.ToString());
            
            //Cheezburger API permits retrieval of maximum 100 lols           
            if(itemCount > 0) {
                if(itemCount > 100) {
                    itemCount = 100;
                } else if(itemCount < 1) {
                    itemCount = 1;
                }
                requestUri += "/" + itemCount.ToString();
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
                Stream sitesStream;
                try {
                    sitesStream = client.OpenRead(_cheezburgerSitesUri);
                } catch {
                    sitesStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CheezburgerAPI.Resources.CheezSites.xml");
                }
                string webResponseString = string.Empty;
                CheezSites cheezSites;
                using(XmlSanitizingStream reader = new XmlSanitizingStream(sitesStream)) {
                    System.Xml.Serialization.XmlSerializer xSerializer = new System.Xml.Serialization.XmlSerializer(typeof(CheezSites));
                    cheezSites = (CheezSites)xSerializer.Deserialize(reader);
                }
                return cheezSites.Items[0].Site.ToList();
            } catch(Exception e) {
                return null;
            }
        }
    }

    public class CheezAPI {
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

        public Fail Fail {
            get {
                return (Fail)_cheezApiResponse;
            }
        }

        public Hai Hai {
            get {
                return (Hai)_cheezApiResponse;
            }
        }
    }
}
