using CommonContracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SyslogServer.PubSubCode.Subscription;

namespace SyslogServer.PubSubCode
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Subscription : ISubscription
    {
        
        static string path = "Events.txt";
        private PublishingService _publishingService = new PublishingService();
        public static Mutex mutex = new Mutex();

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
            list.Add(Events.SecurityAuthorizationMessages.ToString());
            list.Add(Events.MessagesGeneratedBySyslog.ToString());
            list.Add(Events.LogAudit.ToString());
            list.Add(Events.LogAlert.ToString());

            return list;
        }
        
        public bool Subscribe(string topicName)
        {
            bool allowed = false;

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal.IsInRole(Permissions.Edit.ToString()))
            {
                allowed = true;
            }
            else if ((principal.IsInRole(Permissions.Read.ToString()) && (topicName == Events.MessagesGeneratedBySyslog.ToString() || topicName == Events.UserLevelMessages.ToString())))
            {              
                allowed = true;
            }

            if (allowed)
            {
                IPublishing subscriber = OperationContext.Current.GetCallbackChannel<IPublishing>();
                PubSubFilter.AddSubscriber(topicName, subscriber);

                List<string> _historyEvents = new List<string>();
                _historyEvents = ReadEvents(topicName);

                foreach (string _event in _historyEvents)
                {
                    MethodInfo publishMethod = typeof(IPublishing).GetMethod("Publish");
                    publishMethod.Invoke(subscriber, new object[] { _event, topicName });
                }

                Audit.AuthorizationSuccess(Formatter.ParseName(principal.Identity.Name), "Subscribe");
            }
            else
            {
                SecurityException ex = new SecurityException();
                Audit.AuthorizationFailed(Formatter.ParseName(principal.Identity.Name), "Subscribe", ex.Message);
            }

            return allowed;              
        }

        public void UnSubscribe(string topicName)
        {
            IPublishing subscriber = OperationContext.Current.GetCallbackChannel<IPublishing>(); //uzimam bas tog subscriber-a
            PubSubFilter.RemoveSubscriber(topicName, subscriber);
        }

        public bool Edit(string topicName)
        {
            bool allowed = false;

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal.IsInRole(Permissions.Edit.ToString()))
            {
                allowed = true;
                mutex.WaitOne();

                string[] allLines = File.ReadAllLines(path);
                using (StreamWriter sw = new StreamWriter(path))
                    {
                        for (int i = 0; i < allLines.Length; i++)
                        {
                            if (allLines[i].Contains(topicName))
                            {
                                allLines[i] += "_edited";
                            }

                            sw.WriteLine(allLines[i]);
                        }
                    }

                mutex.ReleaseMutex();

                Audit.AuthorizationSuccess(Formatter.ParseName(principal.Identity.Name), "Edit");
            }
            else
            {
                SecurityException ex = new SecurityException();
                Audit.AuthorizationFailed(Formatter.ParseName(principal.Identity.Name), "Edit", ex.Message);
            }

            return allowed;
            
        }

        public bool Delete(string topicName)
        {
            bool allowed = false;

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal.IsInRole(Permissions.Delete.ToString()))
            {
                allowed = true;

                mutex.WaitOne();
                    string[] allLines = File.ReadAllLines(path);

                using(StreamWriter sw = new StreamWriter(path))
                {
                    for(int i = 0; i< allLines.Length; i++)
                    {
                        if (!allLines[i].Contains(topicName))
                        {
                            sw.WriteLine(allLines[i]);
                        }
                    }                
                }
                mutex.ReleaseMutex();

                Audit.AuthorizationSuccess(Formatter.ParseName(principal.Identity.Name), "Delete");
            }
            else
            {
                SecurityException ex = new SecurityException();
                Audit.AuthorizationFailed(Formatter.ParseName(principal.Identity.Name), "Delete", ex.Message);
            }

            return allowed;
        }
    }
}
