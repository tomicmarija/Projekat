using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.PubSubCode
{
    public class NewIssueAvailableEventArgs : EventArgs
    {
        public string msg;
        public string issueNumber;
    }
}
