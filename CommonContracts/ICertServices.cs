using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;

namespace CommonContracts
{
    [ServiceContract]
    public interface ICertServices
    {
        [OperationContract]
        X509Certificate2 CreateCertificate(string user);

        [OperationContract]
        void CompromisedCert(string user);
    }
}
