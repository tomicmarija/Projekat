using CommonContracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CMS
{
   public class ServiceValidation : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            X509Certificate2 srvCert = CertOperations.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, Formatter.ParseName(WindowsIdentity.GetCurrent().Name));

            if (!certificate.Issuer.Equals(srvCert.Issuer))
            {
                Audit.ValidationFailed("Certificate is not from the valid issuer.");
                throw new Exception("Certificate is not from the valid issuer.");
            }

            foreach (Certificate cert in CertManager.RevList)
            {
                if (certificate.Thumbprint == cert.cert.Thumbprint)
                {
                    Audit.ValidationFailed("Certificate is compromised");
                    throw new Exception("Certificate is not valid");
                }
            }

            DateTime d = Convert.ToDateTime(certificate.GetExpirationDateString());
            if (d < DateTime.Now)
            {
                Audit.ValidationFailed("Certificate has expired");
                throw new Exception("Certificate has expired");
            }

            Audit.ValidationSuccess();
        }
    }
}
