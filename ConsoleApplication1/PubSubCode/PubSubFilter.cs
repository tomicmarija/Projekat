using CommonContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.PubSubCode
{
    public class PubSubFilter
    {
        static Dictionary<string, List<IClient>> subscribersList = new Dictionary<string, List<IClient>>();

        public static Dictionary<string,List<IClient>> SubscribersList
        {
            get
            {
                lock (typeof(PubSubFilter))
                {
                    return subscribersList;
                }
            }
        }

        public static List<IClient> GetSubscribers(string topicName)
        {
            lock(typeof(PubSubFilter))
            {
                if(SubscribersList.ContainsKey(topicName))
                {
                    return SubscribersList[topicName];

                }else
                    return null;
            }
        }

        public static void AddSubscriber(string topicName, IClient subscriber)
        {
            lock(typeof(PubSubFilter))
            {
                if(SubscribersList.ContainsKey(topicName))
                {
                    if(!SubscribersList[topicName].Contains(subscriber))
                    {
                        SubscribersList[topicName].Add(subscriber);
                    }
                }
                else
                {
                    List<IClient> newSubscribersList = new List<IClient>();
                    newSubscribersList.Add(subscriber);
                    SubscribersList.Add(topicName, newSubscribersList);
                }
            }
                    
        }

        public static void RemoveSubscriber(string topicName, IClient subscriber)
        {
            lock(typeof(PubSubFilter))
            {
                if(SubscribersList.ContainsKey(topicName))
                {
                    if(SubscribersList[topicName].Contains(subscriber))
                    {
                        SubscribersList[topicName].Remove(subscriber);
                    }
                }
            }
        }

    }
}
