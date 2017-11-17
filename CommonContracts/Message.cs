using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonContracts
{
    [DataContract]
    public class Message
    {
        private string topicName;
        private string eventData;

        [DataMember]
        public string TopicName
        {
            get
            {
                return topicName;
            }
            set
            {
                topicName = value;
            }
        }

        [DataMember]
        public string EventData
        {
            get
            {
                return eventData;
            }
            set
            {
                eventData = value;
            }
        }
    }
}
