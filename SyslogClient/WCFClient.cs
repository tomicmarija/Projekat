using CommonContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SyslogClient
{
    public class WCFClient : ChannelFactory<IWCFContract>, IWCFContract
    {
        IWCFContract factory;

        public WCFClient(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            factory = this.CreateChannel();
        }
        public void SendMessage(string message)
        {
            try
            {
                factory.SendMessage(message);

            }catch(Exception e)
            {
                Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
            }
        }
    }
}
