using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS;

namespace CMSServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CertManager certManager = new CertManager();
             CertManager.CreateCertificate("pera");
            //CertManager.CompromisedCert("Administrator");
            Console.ReadKey();
        }
    }
}
