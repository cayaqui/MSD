namespace Web.Constants
{
    public static class Routes
    {
        // Authentication
        public static class Authentication
        {
            public const string Login = "/authentication/login";
            public const string Logout = "/authentication/logout";
            public const string AccessDenied = "/authentication/access-denied";
            public const string Callback = "/authentication/callback";
        }

        // Main
        public static class Main
        {
            public const string Dashboard = "/";
            public const string Home = "/home";
        }

        // Configuration
        public static class Configuration
        {
            public const string Index = "/configuration";
            public const string Companies = "/configuration/companies";
            public const string CompanyNew = "/configuration/companies/new";
            public const string CompanyDetail = "/configuration/companies/{0}"; // {0} = id
            public const string Users = "/configuration/users";
            public const string UserDetail = "/configuration/users/{0}"; // {0} = id
            public const string Roles = "/configuration/roles";
            public const string Permissions = "/configuration/permissions";
        }

        // Projects
        public static class Projects
        {
            public const string List = "/projects";
            public const string MyProjects = "/projects/my-projects";
            public const string New = "/projects/new";
            public const string Dashboard = "/projects/{0}"; // {0} = id
            public const string Details = "/projects/{0}/details";
            public const string Team = "/projects/{0}/team";
            public const string Charter = "/projects/{0}/charter";

            // WBS/EDT
            public const string WBS = "/projects/{0}/wbs";
            public const string WBSNew = "/projects/{0}/wbs/new";
            public const string WBSEdit = "/projects/{0}/wbs/{1}"; // {1} = wbsId
            public const string ControlAccounts = "/projects/{0}/control-accounts";

            // Schedule
            public const string Schedule = "/projects/{0}/schedule";
            public const string Gantt = "/projects/{0}/schedule/gantt";
            public const string Activities = "/projects/{0}/schedule/activities";
            public const string Milestones = "/projects/{0}/schedule/milestones";
            public const string CriticalPath = "/projects/{0}/schedule/critical-path";

            // Costs
            public const string Costs = "/projects/{0}/costs";
            public const string Budget = "/projects/{0}/costs/budget";
            public const string ActualCosts = "/projects/{0}/costs/actual";
            public const string Forecast = "/projects/{0}/costs/forecast";
            public const string CBS = "/projects/{0}/costs/cbs";
            public const string EVM = "/projects/{0}/costs/evm";

            // Contracts
            public const string Contracts = "/projects/{0}/contracts";
            public const string ContractNew = "/projects/{0}/contracts/new";
            public const string ContractDetail = "/projects/{0}/contracts/{1}"; // {1} = contractId
            public const string ContractChanges = "/projects/{0}/contracts/{1}/changes";

            // Documents
            public const string Documents = "/projects/{0}/documents";
            public const string DocumentUpload = "/projects/{0}/documents/upload";
            public const string DocumentViewer = "/projects/{0}/documents/{1}"; // {1} = docId
            public const string Transmittals = "/projects/{0}/transmittals";
        }

        // Reports
        public static class Reports
        {
            public const string Index = "/reports";
            public const string ExecutiveDashboard = "/reports/executive-dashboard";
            public const string ProjectStatus = "/reports/project-status";
            public const string SCurves = "/reports/s-curves";
            public const string CashFlow = "/reports/cash-flow";
            public const string ResourceHistogram = "/reports/resource-histogram";
            public const string Custom = "/reports/custom";
        }

        // Admin
        public static class Admin
        {
            public const string Index = "/admin";
            public const string SystemSettings = "/admin/system-settings";
            public const string AuditLog = "/admin/audit-log";
            public const string Integrations = "/admin/integrations";
            public const string Backup = "/admin/backup";
        }

        // Helper methods
        public static string GetProjectRoute(string template, Guid projectId, params object[] args)
        {
            var route = string.Format(template, projectId);
            if (args.Length > 0)
            {
                route = string.Format(route.Replace("{0}", projectId.ToString()), args);
            }
            return route;
        }

        public static string GetCompanyRoute(Guid companyId) =>
            string.Format(Configuration.CompanyDetail, companyId);

        public static string GetUserRoute(Guid userId) =>
            string.Format(Configuration.UserDetail, userId);

        public static string GetProjectDashboard(Guid projectId) =>
            string.Format(Projects.Dashboard, projectId);

        public static string GetWBSEdit(Guid projectId, Guid wbsId) =>
            string.Format(Projects.WBSEdit, projectId, wbsId);

        public static string GetContractDetail(Guid projectId, Guid contractId) =>
            string.Format(Projects.ContractDetail, projectId, contractId);

        public static string GetDocumentViewer(Guid projectId, Guid documentId) =>
            string.Format(Projects.DocumentViewer, projectId, documentId);
    }
}