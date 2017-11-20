using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS
{
    class Audit:IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "CMS.Audit";
        const string LogName = "CMSlog";

        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }

                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void CreationCertificateSuccess(string userName)
        {
            
            if (customLog != null)
            {
               
                string message = AuditEvents.CreationCertificateSuccess;
                message = String.Format(message, userName);

                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.CreationCertificateSuccess));
            }
        }


        public static void CreationCertificationFailed(string userName,string reason)
        {

            if (customLog != null)
            {

                string message = AuditEvents.CreationCertificationFailed;
                message = String.Format(message, userName,reason);

                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.CreationCertificationFailed));
            }
        }

        public static void ValidationSuccess()
        {

            if (customLog != null)
            {

                string message = AuditEvents.ValidationSuccess;
                message = String.Format(message);

                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.ValidationSuccess));
            }
        }

        public static void ValidationFailed(string reason)
        {

            if (customLog != null)
            {

                string message = AuditEvents.ValidationFailed;
                message = String.Format(message, reason);

                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.ValidationFailed));
            }
        }

        public static void RevocationSuccess(string user)
        {

            if (customLog != null)
            {

                string message = AuditEvents.RevocationSuccess;
                message = String.Format(message, user);

                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.RevocationSuccess));
            }
        }

        public static void RevocationFailed(string reason)
        {

            if (customLog != null)
            {

                string message = AuditEvents.RevocationFailed;
                message = String.Format(message, reason);

                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.RevocationFailed));
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
