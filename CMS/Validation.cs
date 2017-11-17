using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
namespace CMS
{
    public class Validation : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            if (!certificate.Issuer.Equals(CertManager.Issuer))
            {
                throw new Exception("Certificate is not valid");
            }
        }
    }
}
