using System;
using System.Collections.Generic;
using System.ServiceModel;
namespace Clients
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> list = new List<string>();
            //int counter = 0;

            NetTcpBinding binding = new NetTcpBinding();

            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:2110/ISubscription"));
            //list = SyslogFormat.GetAllEvents();

            using (Client proxy = new Client(binding, address))
            {
                //iscitati sve dogadjaje na konzolu i da on odabere na koji dodgadja hoce da se pretplti (bira prema facility kodu-tj. dogadjaju)
                Console.WriteLine("Events for subscribe: ");
                list = proxy.AllEvents();
                foreach(string st in list)
                {
                    Console.WriteLine(st);

                }
                Console.WriteLine("Choose event to subscribe: ");

                string eventForSubscription = Console.ReadLine();
                int evForSub = Int32.Parse(eventForSubscription);

                //proxy.Subscribe(list[evForSub]);
            }
        }
    }
}
