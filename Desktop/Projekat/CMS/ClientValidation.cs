using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using static CMS.CertManager;
using System.ServiceModel;
using CommonContracts;
using System.Security;

namespace CMS
{
    public class ClientValidation : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            ChannelFactory<ICertServices> certFactory;
            ICertServices _certProxy;
            NetTcpBinding bindingCert = new NetTcpBinding();
            EndpointAddress addressCert = new EndpointAddress(new Uri("net.tcp://10.1.212.183:100/Receiver"));

            certFactory = new ChannelFactory<ICertServices>(bindingCert, addressCert);
            _certProxy = certFactory.CreateChannel();
            
            if(! _certProxy.Validate(certificate))
            {
                throw new SecurityException("Certificate is not valid.");
            }
         
            

   
        }
    }
}
