﻿using CommonContracts;
using SyslogServer;
using SyslogServer.PubSubCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.PubSubCode
{
    public class SyslogFormat
    {
        static string path = "Events.txt";

        public static string GetSeverityLevel(int eventData)
        {
            string severityLevel = string.Empty;

            switch (eventData)
            {
                case 1:
                    severityLevel = "5"; //user level messages - notice
                    break;
                case 2:
                    severityLevel = "6"; //secutiry authorization messages - information
                    break;
                case 3:
                    severityLevel = "5"; // messages generated by syslog
                    break;
                case 4:
                    severityLevel = "6"; //log audit
                    break;
                case 5:
                    severityLevel = "1"; //log alert
                    break;
                default:
                    severityLevel = "4"; //warning
                    break;
            }

            return severityLevel;
        }

        public static string GetEvent(int eventData)
        {
            string eventD = string.Empty;

            switch (eventData)
            {
                case 1:
                    eventD = Events.UserLevelMessages.ToString();
                    break;
                case 2:
                    eventD = Events.SecurityAuthorizationMessages.ToString();
                    break;
                case 3:
                    eventD = Events.MessagesGeneratedBySyslog.ToString();
                    break;
                case 4:
                    eventD = Events.LogAudit.ToString();
                    break;
                case 5:
                    eventD = Events.LogAlert.ToString();
                    break;
            }

            return eventD;
        }

        public static List<string> GetAllEvents()
        {
            List<string> list = new List<string>();
            foreach (string evenetD in WCFService.listOfEvents)
            {
                list.Add(evenetD);
            }
            return list;
        }


        public static void WriteInFile(string message)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path, true);
                sw.WriteLine(message);
                sw.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("[WriteInFile] ERROR = {0}", e.Message);
            }
        }

    }
}
