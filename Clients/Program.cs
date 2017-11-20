using System;
using System.Collections.Generic;
using System.ServiceModel;
namespace Clients
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<string> list = new List<string>();
            //int counter = 0;

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:2113/ISubscription"));
            
            //list = SyslogFormat.GetAllEvents();

            using (Client proxy = new Client(binding, address))
            {
                //iscitati sve dogadjaje na konzolu i da on odabere na koji dodgadja hoce da se pretplti (bira prema facility kodu-tj. dogadjaju)
                string eventForSubscription = string.Empty;
                int evForSub = -1;
                
                while(evForSub < 0 || evForSub > 5)
                {
                    Console.WriteLine("Events for subscribe: ");
                    list = proxy.AllEvents();
                    int counter = 1;
                    foreach (string st in list)
                    {
                        Console.WriteLine("{0}. {1}", counter, st);

                    }
                    Console.WriteLine("Choose event to subscribe: ");

                    eventForSubscription = Console.ReadLine();
                    evForSub = Int32.Parse(eventForSubscription);
                }
                
                proxy.Subscribe(list[evForSub-1]);
                Console.ReadLine();
                proxy.UnSubscribe(list[evForSub - 1]);
            }

            
            
        }
    }
}
