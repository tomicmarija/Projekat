using CommonContracts;
using SyslogServer.PubSubCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyslogServer.PubSubCode
{
    public enum Events { UserLevelMessages = 1, SecurityAuthorizationMessages, MessagesGeneratedBySyslog, LogAudit, LogAlert};
    public enum Severities { Emergency, Alert, Critical, Error, Warning, Notice, Information, Debug};
    public class WCFService : PublishingService, IWCFContract 
    {
        public static List<string> listOfEvents = new List<string>();

        int messNum = 0;
        public void SendMessage(string message)
        {
            //upis u syslog formatu...
            //dobili smo broj za event i poruku/tekst poruke

            string  eventData = null;
            eventData = message[0].ToString();
            int eventD = Int32.Parse(eventData);
            string text = message.Substring(1);
            messNum++;
            //preuzimam host name preko windowsIdentity
            WindowsIdentity identity = (WindowsIdentity)Thread.CurrentPrincipal.Identity;

            string eventMess = SyslogFormat.GetEvent(eventD);
            string severity = SyslogFormat.GetSeverityLevel(eventD);

            string syslogMessage = DateTime.Now.ToString() + "-" + identity.Name + "-" + eventMess + "-" + severity + "-" + messNum.ToString() + "-" + text;

           // listOfEvents.Add(syslogMessage); //dodajem dogadjaj na listu dogadjaja
            SyslogFormat.WriteInFile(syslogMessage);
            Publish(syslogMessage, eventMess); //publishujem dogadjaj

            Console.WriteLine(syslogMessage);

        }
    }
}
