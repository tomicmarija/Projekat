using CommonContracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CMS
{
   public class ServiceValidation : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            ChannelFactory<ICertServices> certFactory;
            ICertServices _certProxy;
            NetTcpBinding bindingCert = new NetTcpBinding();
            EndpointAddress addressCert = new EndpointAddress(new Uri("net.tcp://10.1.212.183:100/Receiver"));

            certFactory = new ChannelFactory<ICertServices>(bindingCert, addressCert);
            _certProxy = certFactory.CreateChannel();

            if (!_certProxy.Validate(certificate))
            {
                Console.WriteLine("Clients certificate compromised!");
                throw new Exception("Certificate is not valid.");
            }
        }
    }
}
