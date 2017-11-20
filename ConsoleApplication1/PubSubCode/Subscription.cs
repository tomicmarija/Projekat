using CommonContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using static SyslogServer.PubSubCode.Subscription;

namespace SyslogServer.PubSubCode
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Subscription : ISubscription
    {
        
        static string path = "Events.txt";
        private PublishingService _publishingService = new PublishingService();
        public List<string> ReadEvents(string topicName)
        {
            List<string> events = new List<string>();
            string line;
            string[] splitedLine;

            try
            {
                StreamReader sr = new StreamReader(path);
                line = sr.ReadLine();

                while(line != null)
                {
                    splitedLine = line.Split('-');
                    if (splitedLine[3] == topicName)
                        events.Add(line);
                    line = sr.ReadLine();
                }
                
                
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ReadFromFile] ERROR = {0}", e.Message);
            }

            return events;
        }

        public List<string> AllEvents()
        {
            List<string> list = new List<string>();
            list.Add(Events.UserLevelMessages.ToString());
            list.Add( Events.SecurityAuthorizationMessages.ToString());
            list.Add(Events.MessagesGeneratedBySyslog.ToString());
            list.Add(Events.LogAudit.ToString());
            list.Add(Events.LogAlert.ToString());

            return list;
        }

        public void Subscribe(string topicName)
        {
            IPublishing subscriber = OperationContext.Current.GetCallbackChannel<IPublishing>();
            PubSubFilter.AddSubscriber(topicName, subscriber);

            List<string> _historyEvents = new List<string>();
            _historyEvents = ReadEvents(topicName);
            
            foreach(string _event in _historyEvents)
            {
                _publishingService.Publish(_event, topicName);
            }
          

            
        }

        public void UnSubscribe(string topicName)
        {
            IPublishing subscriber = OperationContext.Current.GetCallbackChannel<IPublishing>(); //uzimam bas tog subscriber-a
            PubSubFilter.RemoveSubscriber(topicName, subscriber);
        }

    }
}
