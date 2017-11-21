using System;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using CMS;
using System.ServiceModel.Security;
using CommonContracts;

namespace CMSServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CertManager.Set(); //kreiranje foldera i TestCA

            //otvaranje kanala
            NetTcpBinding binding = new NetTcpBinding();

            string address = "net.tcp://localhost:9999/Receiver";
            ServiceHost host = new ServiceHost(typeof(CertServices));
            host.AddServiceEndpoint(typeof(ICertServices), binding, address);

            try
            {
                host.Open();
                Console.WriteLine("CMSServer is started.\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                host.Close();
            }



            //   Console.ReadKey();
            //pamecenje korisnika koji imaju sertifikate
            CertOperations.Serialize("Users");
            CertOperations.Serialize("RVUsers");
        }
    }
}
