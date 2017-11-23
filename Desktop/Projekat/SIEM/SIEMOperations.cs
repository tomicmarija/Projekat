using SecurityManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIEM
{
    public class SIEMOperations
    {
        public static List<FailedEvents> _failedEvents = new List<FailedEvents>();
       
        public static void ReadEvents()
        {
               
                string logName = "MySecTest";
                EventLog eventLog = new EventLog(logName);
                EventLogEntryCollection entries = eventLog.Entries;
                bool found = false;

                EventLogEntry _lastEntry = null;

                if (entries.Count > 0)
                {
                    _lastEntry = entries[entries.Count - 1];
                }



                while (true)
                {
                    entries = eventLog.Entries;

                    if (entries.Count > 0)
                    {
                        if (_lastEntry == null)
                        {
                            EventLogEntry _newEntry = entries[entries.Count - 1];
                            if (_newEntry.Message.Contains("failed"))
                            {
                                string[] datas = _newEntry.Message.Split(' ');
                                FailedEvents newEvent = new FailedEvents();
                                newEvent.Username = datas[3];
                                newEvent.Service = datas[7];
                                _failedEvents.Add(newEvent);
                                _lastEntry = _newEntry;
                            }

                        }
                        else if (_lastEntry.Index != entries[entries.Count - 1].Index)
                        {
                            EventLogEntry _newEntry = entries[entries.Count - 1];
                            string[] datas = _newEntry.Message.Split(' ');

                            if (_newEntry.Message.Contains("failed"))
                            {

                                foreach (FailedEvents _event in _failedEvents)
                                {
                                    if (_event.Username == datas[3] && _event.Service == datas[7])
                                    {
                                        _event.Counter++;
                                        if (CheckCounter(_event))
                                        {
                                            _event.Counter = 0;
                                        }
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    FailedEvents newEvent = new FailedEvents();
                                    newEvent.Username = datas[3];
                                    newEvent.Service = datas[7];
                                    _failedEvents.Add(newEvent);
                                }

                                _lastEntry = _newEntry;
                                found = false;
                            }
                        }
                    }
                }
          
        }

        public static bool CheckCounter(FailedEvents _event)
        {
            if (_event.Counter == 3)
            {
                Console.WriteLine("ALERT: User {0} tried to access service {1} three times unsuccessfully!", _event.Username, _event.Service);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
