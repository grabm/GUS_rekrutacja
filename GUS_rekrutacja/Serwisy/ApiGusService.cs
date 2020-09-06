using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace GUS_rekrutacja.Serwisy
{
    public class ApiGusService
    {
        public string CallWebService()
        {
            var _url = "https://wyszukiwarkaregontest.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc";
            var _akcja = "https://CIS/BIR/PUBL/2014/07/IUslugaBIRzewnPubl/Zaloguj";

            XmlDocument soapEnvelopeXml = TworzXML();
            HttpWebRequest webZadanie = UtworzZadanieWeb(_url, _akcja);
            DodajSoapEnvelopeDoZadaniaWeb(soapEnvelopeXml, webZadanie);

            IAsyncResult asyncRezultat = webZadanie.BeginGetResponse(null, null);

            var request = asyncRezultat.AsyncWaitHandle.WaitOne();

            string soapRezultat = "";

            using (WebResponse webResponse = webZadanie.EndGetResponse(asyncRezultat))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapRezultat = rd.ReadToEnd();
                }
                Console.Write(soapRezultat);
            }

            return null;
        }



        private HttpWebRequest UtworzZadanieWeb(string url, string action)
        {
            HttpWebRequest webZadanie = (HttpWebRequest)WebRequest.Create(url);
            webZadanie.Headers.Add("SOAPAction", action);
            webZadanie.Accept = "text/xml";
            webZadanie.ContentType = "application/x-www-form-urlencoded";
            webZadanie.Method = "POST";

            return webZadanie;
        }

        private void DodajSoapEnvelopeDoZadaniaWeb(XmlDocument soapEnvelopeXml, HttpWebRequest zadanieWeb)
        {
            using (Stream stream = zadanieWeb.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        private XmlDocument TworzXML()
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
