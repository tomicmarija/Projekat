using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIEM
{
    public class FailedEvents
    {
        public string Username { get; set; }
        public string Service { get; set; }
        public int Counter { get; set; }

        public FailedEvents()
        {
            this.Counter = 1;
        }
    }
}
