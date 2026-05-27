using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sicoain.shared.Constants
{
    public class AppPermissions
    {
        // Accidents
        public const string AccidentsView = "Accidents.View";
        public const string AccidentsCreate = "Accidents.Create";
        public const string AccidentsEdit = "Accidents.Edit";
        public const string AccidentsDelete = "Accidents.Delete";
        public const string AccidentsApprove = "Accidents.Approve";

        // Employees
        public const string EmployeesView = "Employees.View";
        public const string EmployeesCreate = "Employees.Create";
        public const string EmployeesEdit = "Employees.Edit";
        public const string EmployeesDelete = "Employees.Delete";

        // Reports
        public const string ReportsView = "Reports.View";
        public const string ReportsExport = "Reports.Export";

        // Users
        public const string UsersView = "Users.View";
        public const string UsersCreate = "Users.Create";
        public const string UsersEdit = "Users.Edit";
        public const string UsersDelete = "Users.Delete";
        public const string UsersAssignRoles = "Users.AssignRoles";

        // Settings
        public const string SettingsView = "Settings.View";
        public const string SettingsEdit = "Settings.Edit";

        // Permissions management (only for superadmin)
        public const string PermissionsManage = "Settings.Manage";

    }
}
