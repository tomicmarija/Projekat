using CommonContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
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

        public bool Delete(string topicName)
        {
            bool allowed = false;

            try
            {
                allowed = _proxy.Delete(topicName);

                if (!allowed)
                {
                    Console.WriteLine("You are not allowed for deleting events!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[Edit] ERROR = {0}", e.Message);
            }

            return allowed;
        }

        public void Dispose()
        {
            if (_proxy != null)
                _proxy = null;

        }

        public bool Edit(string topicName)
        {
            bool allowed = false;

            try
            {
                allowed = _proxy.Edit(topicName);

                if (!allowed)
                {
                    Console.WriteLine("You are not allowed for editing events!");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("[Edit] ERROR = {0}", e.Message);
            }

            return allowed;
        }

        public void Publish(string e, string topicName)
        {
            if(e != string.Empty)
            {
                Console.WriteLine("Event: {0}", e);
            }
        }        

        public bool Subscribe(string topicName)
        {
            bool allowed = false;
            
            try
            {
                allowed = _proxy.Subscribe(topicName);

                if (!allowed)
                {
                    Console.WriteLine("You don't have permission for subscribing facility code for {0}!", topicName);
                }        
            }
            catch (Exception  e)
            {            
                Console.WriteLine("[Subscribe] ERROR = {0}", e.Message);
            }

            return allowed;
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
