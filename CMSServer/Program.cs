using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS;
using System.Security.Cryptography.X509Certificates;

namespace CMSServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CertManager certManager = new CertManager();

            certManager.Deserialize("Users");//iscitava iz fajla usere
            CertManager.GetCertificateFromFile("Users");//popunjava listu sertifikata
            certManager.Deserialize("RVUsers");//iscitava iz fajla RVusere
            CertManager.GetCertificateFromFile("RVUsers");//popunjava listu RVsertifikata

            //CertManager.CreateCertificate("isidora");
            //CertManager.CreateCertificate("ivona");


            //CertManager.CompromisedCert("isidora");

            certManager.Serialize("Users");
            certManager.Serialize("RVUsers");

            Console.ReadKey();
        }
    }
}
