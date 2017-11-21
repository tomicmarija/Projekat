using CMS;
using CommonContracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
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
                EndpointAddress addressCert = new EndpointAddress(new Uri("net.tcp://localhost:100/Receiver"));

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
                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                string address = "net.tcp://localhost:300/IWCFContract";
                ServiceHost host = new ServiceHost(typeof(PubSubCode.WCFService));
                host.AddServiceEndpoint(typeof(IWCFContract), binding, address);

                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
                host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceValidation();
                host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
                string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
                host.Credentials.ServiceCertificate.Certificate = CertOperations.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);


                string _subscribeAddress = "net.tcp://localhost:301/ISubscription";
                NetTcpBinding _subscribeBinding = new NetTcpBinding();
                ServiceHost _subscribeServiceHost = new ServiceHost(typeof(PubSubCode.Subscription));
                _subscribeServiceHost.AddServiceEndpoint(typeof(ISubscription), _subscribeBinding, _subscribeAddress);

                _subscribeServiceHost.Authorization.ServiceAuthorizationManager = new CustomAuthorizationManager();

                List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
                policies.Add(new CustomAuthorizationPolicy());
                _subscribeServiceHost.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

                _subscribeServiceHost.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

                ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
                newAudit.AuditLogLocation = AuditLogLocation.Application;
                newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
                newAudit.SuppressAuditFailure = true;

                _subscribeServiceHost.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
                _subscribeServiceHost.Description.Behaviors.Add(newAudit);

                _subscribeServiceHost.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
                _subscribeServiceHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

                string _publishAddress = "net.tcp://localhost:302/IPublishing";
                ServiceHost _publishServiceHost = new ServiceHost(typeof(PubSubCode.PublishingService));
                _publishServiceHost.AddServiceEndpoint(typeof(IPublishing), new NetTcpBinding(), _publishAddress);

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
