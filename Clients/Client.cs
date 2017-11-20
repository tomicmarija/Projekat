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
    public class Client : ISubscription, IPublishing, IDisposable
    {
        ISubscription _proxy;
        public Client(NetTcpBinding binding, EndpointAddress address)
        {
            DuplexChannelFactory <ISubscription> channelFactory = new DuplexChannelFactory<ISubscription>(new InstanceContext(this), binding, address);
            _proxy = channelFactory.CreateChannel();
        }

        public List<string> AllEvents()
        {

            List<string> list = new List<string>();
            try
            {
                list = _proxy.AllEvents();
            }
            catch (Exception e)
            {
                Console.WriteLine("[AllEvents] ERROR = {0}", e.Message);
            }
            return list;
        }

        public void Dispose()
        {
            if (_proxy != null)
                _proxy = null;

        }

        public void Publish(string e, string topicName)
        {
            if(e != string.Empty)
            {
                Console.WriteLine("Event: {0}", e);
            }
        }        

        public void Subscribe(string topicName)
        {
            try
            {
                _proxy.Subscribe(topicName);           
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
                _proxy.UnSubscribe(topicName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Unsubscribe] ERROR = {0}", e.Message);
            }
        }
    }
}
