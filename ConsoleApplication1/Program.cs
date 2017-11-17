using CommonContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer
{
    class Program
    {

        static void Main(string[] args)
        {

            NetTcpBinding binding = new NetTcpBinding();
            //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:2110/IWCFContract";
            ServiceHost host = new ServiceHost(typeof(PubSubCode.WCFService));
            host.AddServiceEndpoint(typeof(IWCFContract), binding, address);

            string address1 = "net.tcp://localhost:2110/ISubscription";

            ServiceHost host1 = new ServiceHost(typeof(PubSubCode.Subscription));
            host1.AddServiceEndpoint(typeof(ISubscription), binding, address1);

            try
            {
                host.Open();
                host1.Open();
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
                host1.Close();
            }
        }
    }
}
