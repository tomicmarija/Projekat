using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace Clients
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:2113/ISubscription"));

            List<string> _listOfEvents = new List<string>();
            List<int> _choosenEvents;

            string eventsForSubscription = string.Empty;
            bool goodChoice = true;

            using (Client proxy = new Client(binding, address))
            {
                //iscitati sve dogadjaje na konzolu i da on odabere na koji dodgadja hoce da se pretplti (bira prema facility kodu-tj. dogadjaju)
                
                _listOfEvents = proxy.AllEvents();
                
                Console.WriteLine("Events for subscribe: ");
                int counter = 1;
                foreach (string st in _listOfEvents)
                {
                    Console.WriteLine("{0}. {1}", counter++, st);
                    
                }

                do
                {
                    goodChoice = true;
                    _choosenEvents = new List<int>();       
                    Console.WriteLine("Choose events to subscribe (separate numbers with space): ");
                    eventsForSubscription = Console.ReadLine();
                    string[] _stringEvents = eventsForSubscription.Split(' ');

                    int convertedNumber = -1;

                    foreach(string _event in _stringEvents)
                    {
                        if(Int32.TryParse(_event, out convertedNumber))
                        {
                            if(convertedNumber < 1 || convertedNumber > 5)
                            {
                                Console.WriteLine("There is not event with number {0}", convertedNumber);
                            }
                            else
                            {
                                _choosenEvents.Add(convertedNumber);
                            }
                            
                        }
                        else
                        {
                            goodChoice = false;
                            break;
                        }
                    }

                } while (!goodChoice);

                foreach (int _event in _choosenEvents)
                {
                    proxy.Subscribe(_listOfEvents[_event - 1]);
                }

                Thread.Sleep(100);
                proxy.Edit(_listOfEvents[4]);
                proxy.Delete(_listOfEvents[0]);

                Console.ReadLine();

                foreach (int _event in _choosenEvents)
                {
                    proxy.UnSubscribe(_listOfEvents[_event - 1]);
                }
                              
                
            }

        }
    }
}
