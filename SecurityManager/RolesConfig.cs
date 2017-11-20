using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public enum Permissions
    {
        Read = 0,
        Execute = 1,
        Delete = 2
    }

    public enum Roles
    {
        OrdinarySubscriber = 0,
        Moderator = 1,
        Administrator = 2
    }
    public class RolesConfig
    {
        static string[] OrdinarySubscriberPermissions = new string[] { Permissions.Read.ToString() };
        static string[] ModeratorPermissions = new string[] { Permissions.Read.ToString(), Permissions.Execute.ToString() };
        static string[] AdministratorPermissions = new string[] { Permissions.Read.ToString(), Permissions.Execute.ToString(), Permissions.Delete.ToString() };
        static string[] Empty = new string[] { };

        public static string[] GetPermissions(string role)
        {
            switch (role)
            {
                case "OrdinarySubscriber": return OrdinarySubscriberPermissions;
                case "Moderator": return ModeratorPermissions;
                case "Administrator": return AdministratorPermissions;
                default: return Empty;
            }
        }
    }
}
