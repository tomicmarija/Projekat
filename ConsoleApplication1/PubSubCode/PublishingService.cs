using CommonContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.PubSubCode
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class PublishingService : IPublishing
    {
        public void Publish(string e , string topicName)
        { 
            List<IPublishing> subscribers = PubSubFilter.GetSubscribers(topicName);

            if(subscribers == null)
            {
                return;
            }

            MethodInfo publishMethod = typeof(IPublishing).GetMethod("Publish");

            foreach(IPublishing subscriber in subscribers)
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
