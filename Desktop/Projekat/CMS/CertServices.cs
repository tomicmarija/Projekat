using CommonContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CMS
{
    public class CertServices : ICertServices
    {
        public void CompromisedCert(string user)
        {
            CertManager.CompromisedCert(user);
        }

        public X509Certificate2 CreateCertificate(string user)
        {
            X509Certificate2 certificate = CertManager.CreateCertificate(user);
            return certificate;
        }

        public bool Validate(X509Certificate2 certificate)
        {
            bool rez = CertManager.Validate(certificate);
            return rez;
        }
    }
}
