using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using static CMS.CertManager;

namespace CMS
{
    public class Validation : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            if (!certificate.Issuer.Equals("CN="+CertManager.Issuer))
            {
                Audit.ValidationFailed("Issuer is not familiar");
                throw new Exception("Certificate is not valid");
            }

            foreach(Certificate cert in CertManager.RevList)
            {
                if(certificate.Thumbprint==cert.cert.Thumbprint)
                {
                    Audit.ValidationFailed("Certificate is compromised");
                    throw new Exception("Certificate is not valid");
                }
            }

            DateTime d = Convert.ToDateTime(certificate.GetExpirationDateString());
            if(d<DateTime.Now)
            {
                Audit.ValidationFailed("Certificate has expired");
                throw new Exception("Certificate has expired");
            }

            Audit.ValidationSuccess();
        
        }
    }
}
