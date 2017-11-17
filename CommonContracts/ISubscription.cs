using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommonContracts
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IClient))]
    public interface ISubscription
    {
        [OperationContract]
        List<string> ReadEvents();

        [OperationContract]
        List<string> AllEvents();

        [OperationContract(IsOneWay = false, IsTerminating = true)]
        void Subscribe(string topicName);

        [OperationContract(IsOneWay = false, IsTerminating = true)]
        void UnSubscribe(string topicName);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Publish(string msg, string issueNumber);
    }
}
