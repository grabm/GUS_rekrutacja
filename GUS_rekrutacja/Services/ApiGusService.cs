using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace GUS_rekrutacja.Services
{
    public class ApiGusService
    {
        public string CallWebService()
        {
            var _url = "https://wyszukiwarkaregontest.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc";
            var _action = "https://CIS/BIR/PUBL/2014/07/IUslugaBIRzewnPubl/Zaloguj";

            XmlDocument soapEnvelopeXml = CreateXML();
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            var request = asyncResult.AsyncWaitHandle.WaitOne();

            string soapResult = "";
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                Console.Write(soapResult);
            }

            return null;
        }



        private HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.Accept = "text/xml";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            return webRequest;
        }

        private void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        private XmlDocument CreateXML()
        {
            XNamespace soap = "http://www.w3.org/2003/05/soap-envelope";
            XNamespace ns = "http://CIS/BIR/PUBL/2014/07";
            XNamespace wsa = "http://www.w3.org/2005/08/addressing";
            string wsaAction = "http://CIS/BIR/PUBL/2014/07/IUslugaBIRzewnPubl/Zaloguj";
            string wsaTo = "https://wyszukiwarkaregontest.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc";
            string klucz = "abcde12345abcde12345";


            var xDocument = new XDocument(
                new XElement(soap + "Envelope", new XAttribute(XNamespace.Xmlns + "soap", soap), new XAttribute(XNamespace.Xmlns + "ns", ns),
                    new XElement(soap + "Header", new XAttribute(XNamespace.Xmlns + "wsa", wsa),
                        new XElement(wsa + "Action", wsaAction),
                        new XElement(wsa + "To", wsaTo)),
                    new XElement(soap + "Body",
                    new XElement(ns + "Zaloguj",
                        new XElement(ns + "pKluczUzytkownika", klucz))))
                );

            string xmlDocument = xDocument.ToString();

            XmlDocument soapEnvelopeDocument = new XmlDocument();

            soapEnvelopeDocument.LoadXml(xmlDocument);

            return soapEnvelopeDocument;
        }
    }
}
