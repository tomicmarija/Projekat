﻿using CMS;
using CommonContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyslogClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<ICertServices> certFactory;
            ICertServices _certProxy;
            X509Certificate2 certificate = null;



            Console.WriteLine("Choose mode:");
            Console.WriteLine("1. Create certificate");
            Console.WriteLine("2. Compromised certificate");
            Console.WriteLine("3. Establish connetion to server");
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
                string nameStr = name.ToString();
                nameStr = nameStr.Substring(nameStr.IndexOf('\\') + 1);

                certificate = _certProxy.CreateCertificate(nameStr);

            }
            else if(mode == "3")
            {
                IdentityReference identity = WindowsIdentity.GetCurrent().User;
                SecurityIdentifier sid = (SecurityIdentifier)identity.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount));
                string nameStr = name.ToString();

                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                string srvCertCN = "server";
                X509Certificate2 certificateserv = CertOperations.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);

                EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:300/IWCFContract"), new X509CertificateEndpointIdentity(certificateserv));


                srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name); ;
                certificate = CertOperations.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

                string eventData = "-1";
                int evNo = -1;
                using (WCFClient proxy = new WCFClient(binding, address,certificate))
                {
                    while (true)
                    {                      
                                        
                        while (Int32.Parse(eventData) < 1 || Int32.Parse(eventData) > 5)
                        {
                            Console.WriteLine("Make new event: ");
                            Console.WriteLine("1. User level messages");
                            Console.WriteLine("2. Security/Authorization messages");
                            Console.WriteLine("3. Messages generated by syslog");
                            Console.WriteLine("4. Log audit");
                            Console.WriteLine("5. Log alert");

                            eventData = Console.ReadLine();

                            if (!Int32.TryParse(eventData, out evNo))
                            {
                                eventData = "-1";
                                Console.WriteLine("Wrong choice! Try again..");
                            }
                        }

                        Console.WriteLine("Enter message: ");
                        string messText = Console.ReadLine();                       

                        if(!proxy.SendMessage(eventData + messText, nameStr))
                        {
                            Console.WriteLine("Certificate is not valid! Closing client application...");
                            Console.ReadKey();
                            proxy.Abort();
                            return;
                        }

                        eventData = "-1";
                    }
                }
            }
            else
            {
                NetTcpBinding bindingCert = new NetTcpBinding();
                EndpointAddress addressCert = new EndpointAddress(new Uri("net.tcp://localhost:100/Receiver"));

                certFactory = new ChannelFactory<ICertServices>(bindingCert, addressCert);
                _certProxy = certFactory.CreateChannel();

                IdentityReference identity = WindowsIdentity.GetCurrent().User;
                SecurityIdentifier sid = (SecurityIdentifier)identity.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount));
                string nameStr = name.ToString();
                nameStr = nameStr.Substring(nameStr.IndexOf('\\') + 1);

                _certProxy.CompromisedCert(nameStr);
            }

        }
    }
}
