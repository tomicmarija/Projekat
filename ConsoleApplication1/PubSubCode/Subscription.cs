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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Subscription : ISubscription
    {
        public static event NewIssueAvailableHanlder NewIssueAvailableEvent;
        public delegate void NewIssueAvailableHanlder(object sender, NewIssueAvailableEventArgs e);
        NewIssueAvailableHanlder newIssueHandler;
        IClient callback = null;

        static string path = "Events.txt";

        public List<string> ReadEvents()
        {
            List<string> events = new List<string>();
            string line;
            string[] splitedLine;

            try
            {
                StreamReader sr = new StreamReader(path);
                line = sr.ReadLine();

                while (line != null)
                {
                    splitedLine = line.Split('-');
                    if (!events.Contains(splitedLine[3]))
                        events.Add(splitedLine[3]);
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
            list.Add("1." + Events.UserLevelMessages.ToString());
            list.Add("2." + Events.SecurityAuthorizationMessages.ToString());
            list.Add("3." + Events.MessagesGeneratedBySyslog.ToString());
            list.Add("4." + Events.LogAudit.ToString());
            list.Add("5." + Events.LogAlert.ToString());

            return list;
        }

        public void Subscribe(string topicName)
        {
            /*IClient subscriber = OperationContext.Current.GetCallbackChannel<IClient>();

            PubSubFilter.AddSubscriber(topicName, subscriber);*/

            callback = OperationContext.Current.GetCallbackChannel<IClient>();
            newIssueHandler = new NewIssueAvailableHanlder(Publish_NewIssueAvailableEvent);
            NewIssueAvailableEvent += newIssueHandler;

        }

        public void UnSubscribe(string topicName)
        {
            /*IClient subscriber = OperationContext.Current.GetCallbackChannel<IClient>(); //uzimam bas tog subscriber-a
            PubSubFilter.RemoveSubscriber(topicName, subscriber);*/

            NewIssueAvailableEvent -= newIssueHandler;
        }

        public void Publish(string msg, string issueNumber)
        {
            NewIssueAvailableEventArgs eargs = new NewIssueAvailableEventArgs();
            eargs.msg = msg;
            eargs.issueNumber = issueNumber;

            NewIssueAvailableEvent(this, eargs);
        }

        public void Publish_NewIssueAvailableEvent(object sender, NewIssueAvailableEventArgs e)
        {
            callback.MessageReceived(e.msg);
        }
    }
}
