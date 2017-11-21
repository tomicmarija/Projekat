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
            CertManager.CreateCertificate("CMSServer"); //sertifikat za server


            //otvaranje kanala
			string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:9999/Receiver";
            ServiceHost host = new ServiceHost(typeof(CertServices));
            host.AddServiceEndpoint(typeof(ICertServices), binding, address);

            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new Validation();
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            host.Credentials.ServiceCertificate.Certificate = CertOperations.GetCertificateFromFile("CMSServer.pfx");

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


          
            //citanje vec kreiranih sertifikata iz fajla - podesavanje in-memory baze
            CertOperations certManager = new CertOperations();
            certManager.Deserialize("Users");
            CertOperations.AddCertificatesToList("Users");
            certManager.Deserialize("RVUsers");
            CertOperations.AddCertificatesToList("RVUsers");


            Console.ReadKey();
            //pamecenje korisnika koji imaju sertifikate
            certManager.Serialize("Users");
            certManager.Serialize("RVUsers");            
        }
    }
}
