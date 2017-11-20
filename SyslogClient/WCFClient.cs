using CommonContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CMS;

namespace SyslogClient
{
    public class WCFClient : ChannelFactory<IWCFContract>, IWCFContract
    {
        IWCFContract factory;

        public WCFClient(NetTcpBinding binding, EndpointAddress address, X509Certificate2 certificate)
            : base(binding, address)
        {
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new Validation();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            this.Credentials.ClientCertificate.Certificate = certificate;

            factory = this.CreateChannel();
            
        }
        public void SendMessage(string message)
        {
            try
            {
                factory.SendMessage(message);

            }catch(Exception e)
            {
                Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
            }
        }
    }
}
