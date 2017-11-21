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
        public void Deserialize(string str)
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


        public void Serialize(string str)
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



        public static X509Certificate2 GetCertificateFromFile(string fileName)
        {
            X509Certificate2 certificate = null;
            try
            {
                certificate = new X509Certificate2(fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to GetCertificateFromFile {0}. ERROR = {1}", fileName, e.Message);
            }

            return certificate;
        }
    }
}
