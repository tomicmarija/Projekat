using CommonContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Clients
{
    public class Client :  ChannelFactory<ISubscription> , ISubscription, IClient
    {
        ISubscription factory;
        public Client(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public List<string> AllEvents()
        {

            List<string> list = new List<string>();
            try
            {
                list = factory.AllEvents();
                //Console.WriteLine("Klijent se pretplatio na dogadjaj: " + topicName);

            }
            catch (Exception e)
            {
                Console.WriteLine("[AllEvents] ERROR = {0}", e.Message);

            }
            return list;
        }

        public void MessageReceived(string msg)
        {
            Console.WriteLine("New event: {0}", msg);
        }

        public void Publish(string msg, string issueNumber)
        {
            throw new NotImplementedException();
        }

        public List<string> ReadEvents()
        {
            List<string> list = new List<string>();
            try
            {
                list =  factory.ReadEvents();
                //Console.WriteLine("Klijent se pretplatio na dogadjaj: " + topicName);

            }
            catch (Exception e)
            {
                Console.WriteLine("[ReadFromFile] ERROR = {0}", e.Message);
                
            }
                return list;
            
        }
          

        public void Subscribe(string topicName)
        {
            try
            {
                factory.Subscribe(topicName);
                Console.WriteLine("Klijent se pretplatio na dogadjaj: " + topicName);

            }
            catch (Exception e)
            {
                Console.WriteLine("[Subscribe] ERROR = {0}", e.Message);
            }
        }

        public void UnSubscribe(string topicName)
        {
            try
            {
                factory.UnSubscribe(topicName);

            }
            catch (Exception e)
            {
                Console.WriteLine("[Unsubscribe] ERROR = {0}", e.Message);
            }
        }
    }
}
