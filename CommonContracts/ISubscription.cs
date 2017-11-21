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
        bool Subscribe(string topicName);

        [OperationContract]
        void UnSubscribe(string topicName);

        [OperationContract]
        bool Edit(string topicName);

        [OperationContract]
        bool Delete(string topicName);
    }
}
