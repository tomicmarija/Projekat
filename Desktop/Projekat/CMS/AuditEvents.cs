using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CMS
{
    public enum AuditEventTypes
    {
        CreationCertificateSuccess = 0,
        CreationCertificationFailed = 1,
        ValidationSuccess = 2,
        ValidationFailed=3,
        RevocationSuccess=4,
            RevocationFailed=5

    }


    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMgr
        {
            get
            {
                lock (resourceLock)
                {
                    if (resourceManager == null)
                    {
                        resourceManager = new ResourceManager(typeof(AuditEventsFile).FullName, Assembly.GetExecutingAssembly());
                    }
                    return resourceManager;
                }
            }
        }

        public static string CreationCertificateSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.CreationCertificateSuccess.ToString());
            }
        }

        public static string CreationCertificationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.CreationCertificationFailed.ToString());
            }
        }

        public static string ValidationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.ValidationSuccess.ToString());
            }
        }

        public static string RevocationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.RevocationSuccess.ToString());
            }
        }

        public static string ValidationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.ValidationFailed.ToString());
            }
        }
        public static string RevocationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.RevocationFailed.ToString());
            }
        }
    }
}
