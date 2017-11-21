using CommonContracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            NetTcpBinding binding = new NetTcpBinding();
            //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:2110/IWCFContract";
            ServiceHost host = new ServiceHost(typeof(PubSubCode.WCFService));
            host.AddServiceEndpoint(typeof(IWCFContract), binding, address);

            string _subscribeAddress = "net.tcp://localhost:2113/ISubscription";
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

            string _publishAddress = "net.tcp://localhost:2115/IPublishing";
            ServiceHost _publishServiceHost = new ServiceHost(typeof(PubSubCode.PublishingService));
            _publishServiceHost.AddServiceEndpoint(typeof(IPublishing), binding, _publishAddress);

            try
            {
                host.Open();
                _subscribeServiceHost.Open();
                _publishServiceHost.Open();
                Console.WriteLine("WCFService is started.\nPress <enter> to stop ...");
                Console.ReadLine();

            }catch(Exception e)
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
