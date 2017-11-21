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
        Edit = 1,
        Delete = 2
    }

    public enum Roles
    {
        OrdinarySubscribers = 0,
        Moderators = 1,
        Admins = 2
    }
    public class RolesConfig
    {
        static string[] OrdinarySubscriberPermissions = new string[] { Permissions.Read.ToString() };
        static string[] ModeratorPermissions = new string[] { Permissions.Read.ToString(), Permissions.Edit.ToString() };
        static string[] AdministratorPermissions = new string[] { Permissions.Read.ToString(), Permissions.Edit.ToString(), Permissions.Delete.ToString() };
        static string[] Empty = new string[] { };

        public static string[] GetPermissions(string role)
        {
            switch (role)
            {
                case "OrdinarySubscribers": return OrdinarySubscriberPermissions;
                case "Moderators": return ModeratorPermissions;
                case "Admins": return AdministratorPermissions;
                default: return Empty;
            }
        }
    }
}
