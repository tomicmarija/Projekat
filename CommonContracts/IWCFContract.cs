﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommonContracts
{
    [ServiceContract]
    public interface IWCFContract
    {

        [OperationContract]
        void SendMessage(string message);
    }
}
