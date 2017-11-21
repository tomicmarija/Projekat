using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CMS
{
    public class CertOperations
    {
        public static  void Deserialize(string str)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<string>));
            using (TextReader reader = new StreamReader(str + ".xml"))
            {
                object obj = deserializer.Deserialize(reader);
                if (str == "Users")
                    CertManager.Users = (List<string>)obj;
                else
                    CertManager.RVUsers = (List<string>)obj;
            }
        }


        public static  void Serialize(string str)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            using (TextWriter textWriter = new StreamWriter(str + ".xml"))
            {
                if (str == "Users")
                    serializer.Serialize(textWriter, CertManager.Users);
                else
                    serializer.Serialize(textWriter, CertManager.RVUsers);
            }
        }


        public static void AddCertificatesToList(string str)
        {
            X509Certificate2 certificate = null;

            if (str == "Users")
            {
                foreach (string u in CertManager.Users)
                {
                    try
                    {
                        certificate = new X509Certificate2(string.Format(@"Sertifikati\{0}.cer", u));
                        CertManager.Certificates.Add(new Certificate(certificate, u));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while trying to GetCertificateFromFile {0}. ERROR = {1}", u, e.Message);
                    }
                }
            }
            else
            {
                foreach (string u in CertManager.RVUsers)
                {
                    try
                    {
                        certificate = new X509Certificate2(string.Format(@"RV\{0}.cer", u));
                        CertManager.RevList.Add(new Certificate(certificate, u));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while trying to GetCertificateFromFile {0}. ERROR = {1}", u, e.Message);
                    }
                }
            }

        }



        public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string user)
        {
            string userCn = String.Format("CN={0}", user);
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2 certificate = new X509Certificate2();
            List<X509Certificate2> certCollection = new List<X509Certificate2>();
            foreach(var cert in store.Certificates)
            {
                certCollection.Add(cert);
            }

            foreach(X509Certificate2 cert in certCollection)
            {
                string[] names = cert.Subject.Split('_');

                if (names[0] == userCn)
                {
                    certificate = cert;
                    break;
                }
            }

            return certificate;
        }
    }
}
