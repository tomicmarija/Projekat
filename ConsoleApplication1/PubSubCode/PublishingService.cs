using CommonContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.PubSubCode
{

    public class PublishingService : IPublishing
    {
        public void Publish(string e , string topicName)
        { 
            List<IClient> subscribers = PubSubFilter.GetSubscribers(topicName);

            if(subscribers == null)
            {
                return;
            }

            //calling the method
            MethodInfo publishMethod = typeof(IClient).GetMethod("Publish");

            foreach(IClient subscriber in subscribers)
            {
                try
                {
                    publishMethod.Invoke(subscriber, new object[] { e, topicName });
                }
                catch
                {

                }
            }

        }
    }
}
