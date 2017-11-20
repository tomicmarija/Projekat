using CMS;
using CommonContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ChannelFactory<ICertServices> certFactory;
            ICertServices _certProxy;
            X509Certificate2 certificate = null;
            string nameStr="";


            Console.WriteLine("Choose mode:");
            Console.WriteLine("1. Create certificate");
            Console.WriteLine("2. Start service");
            string mode = Console.ReadLine();


            if (mode == "1")
            {
                NetTcpBinding bindingCert = new NetTcpBinding();
                EndpointAddress addressCert = new EndpointAddress(new Uri("net.tcp://localhost:9999/Receiver"));

                certFactory = new ChannelFactory<ICertServices>(bindingCert, addressCert);
                _certProxy = certFactory.CreateChannel();

                IdentityReference identity = WindowsIdentity.GetCurrent().User;
                SecurityIdentifier sid = (SecurityIdentifier)identity.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount));
                nameStr = name.ToString();
                nameStr = nameStr.Substring(nameStr.IndexOf('\\') + 1);

                certificate = _certProxy.CreateCertificate(nameStr);
            }
            else
            {
                IdentityReference identity = WindowsIdentity.GetCurrent().User;
                SecurityIdentifier sid = (SecurityIdentifier)identity.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount));
                nameStr = name.ToString();
                nameStr = nameStr.Substring(nameStr.IndexOf('\\') + 1);


                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                string address = "net.tcp://localhost:2110/IWCFContract";
                ServiceHost host = new ServiceHost(typeof(PubSubCode.WCFService));
                host.AddServiceEndpoint(typeof(IWCFContract), binding, address);

                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
                host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new Validation();
                host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
                host.Credentials.ServiceCertificate.Certificate = CertOperations.GetCertificateFromFile(nameStr + ".cer");


                string _subscribeAddress = "net.tcp://localhost:2113/ISubscription";
                NetTcpBinding _subscribeBinding = new NetTcpBinding(SecurityMode.None);
                ServiceHost _subscribeServiceHost = new ServiceHost(typeof(PubSubCode.Subscription));
                _subscribeServiceHost.AddServiceEndpoint(typeof(ISubscription), _subscribeBinding, _subscribeAddress);

                string _publishAddress = "net.tcp://localhost:2112/IPublishing";
                ServiceHost _publishServiceHost = new ServiceHost(typeof(PubSubCode.PublishingService));
                _publishServiceHost.AddServiceEndpoint(typeof(IPublishing), binding, _publishAddress);

                try
                {
                    host.Open();
                    _subscribeServiceHost.Open();
                    _publishServiceHost.Open();
                    Console.WriteLine("WCFService is started.\nPress <enter> to stop ...");
                    Console.ReadLine();

                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] {0}", e.Message);
                    Console.WriteLine("[Stacktrace] {0}", e.StackTrace);
                }
                finally
                {
                    host.Close();
                    _subscribeServiceHost.Close();
                    _publishServiceHost.Close();
                }
            }
        }
    }
}
