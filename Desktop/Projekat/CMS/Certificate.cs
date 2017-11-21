using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CMS
{
    
    public class Certificate
    {
       
        public string name { get; set; }
      
        public X509Certificate2 cert { get; set; }
        public Certificate()
        {

        }
        public Certificate(X509Certificate2 c, string n)
        {
            cert = c;
            name = n;
        }

    }

  
}
