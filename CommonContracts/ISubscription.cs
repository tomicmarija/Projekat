using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommonContracts
{
    [ServiceContract(CallbackContract = typeof(IPublishing))]
    public interface ISubscription
    {
        [OperationContract]
        List<string> AllEvents();

        [OperationContract]
        void Subscribe(string topicName);

        [OperationContract]
        void UnSubscribe(string topicName);
    }
}
