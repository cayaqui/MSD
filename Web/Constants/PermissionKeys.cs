using System.Collections.Generic;
using System.Reflection;

namespace Web.Constants
{
    /// <summary>
    /// Define todas las constantes de permisos del sistema siguiendo el patrón module.resource.action
    /// </summary>
    public static class PermissionKeys
    {
        // Sistema y Configuración
        public static class System
        {
            public const string ViewConfiguration = "system.configuration.view";
            public const string EditConfiguration = "system.configuration.edit";
            public const string ViewUsers = "system.users.view";
            public const string ManageUsers = "system.users.manage";
            public const string ViewRoles = "system.roles.view";
            public const string ManageRoles = "system.roles.manage";
            public const string ViewPermissions = "system.permissions.view";
            public const string ManagePermissions = "system.permissions.manage";
        }

        // Empresas
        public static class Company
        {
            public const string View = "company.view";
            public const string Create = "company.create";
            public const string Edit = "company.edit";
            public const string Delete = "company.delete";
            public const string ViewOperations = "company.operations.view";
            public const string ManageOperations = "company.operations.manage";
        }

        // Proyectos
        public static class Project
        {
            public const string View = "project.view";
            public const string Create = "project.create";
            public const string Edit = "project.edit";
            public const string Delete = "project.delete";
            public const string ViewTeam = "project.team.view";
            public const string ManageTeam = "project.team.manage";
            public const string ViewCharter = "project.charter.view";
            public const string EditCharter = "project.charter.edit";
        }

        // WBS/EDT (Work Breakdown Structure)
        public static class WBS
        {
            public const string View = "project.wbs.view";
            public const string Create = "project.wbs.create";
            public const string Edit = "project.wbs.edit";
            public const string Delete = "project.wbs.delete";
            public const string Approve = "project.wbs.approve";
        }

        // Cronograma
        public static class Schedule
        {
            public const string View = "project.schedule.view";
            public const string Edit = "project.schedule.edit";
            public const string ViewGantt = "project.schedule.gantt.view";
            public const string ManageActivities = "project.schedule.activities.manage";
            public const string ManageMilestones = "project.schedule.milestones.manage";
            public const string ViewCriticalPath = "project.schedule.criticalpath.view";
        }

        // Costos
        public static class Cost
        {
            public const string View = "project.cost.view";
            public const string Edit = "project.cost.edit";
            public const string ViewBudget = "project.cost.budget.view";
            public const string EditBudget = "project.cost.budget.edit";
            public const string ViewActuals = "project.cost.actuals.view";
            public const string EditActuals = "project.cost.actuals.edit";
            public const string ViewForecast = "project.cost.forecast.view";
            public const string EditForecast = "project.cost.forecast.edit";
            public const string ViewCBS = "project.cost.cbs.view";
            public const string EditCBS = "project.cost.cbs.edit";
            public const string ViewEVM = "project.cost.evm.view";
            public const string ViewControlAccounts = "project.cost.controlaccounts.view";
            public const string ManageControlAccounts = "project.cost.controlaccounts.manage";
        }

        // Contratos
        public static class Contract
        {
            public const string View = "project.contract.view";
            public const string Create = "project.contract.create";
            public const string Edit = "project.contract.edit";
            public const string Delete = "project.contract.delete";
            public const string ManageChanges = "project.contract.changes.manage";
            public const string ManageValuations = "project.contract.valuations.manage";
            public const string ManageRetentions = "project.contract.retentions.manage";
        }

        // Documentos
        public static class Document
        {
            public const string View = "project.document.view";
            public const string Upload = "project.document.upload";
            public const string Edit = "project.document.edit";
            public const string Delete = "project.document.delete";
            public const string Download = "project.document.download";
            public const string ManageTransmittals = "project.document.transmittals.manage";
            public const string ManageDistribution = "project.document.distribution.manage";
        }

        // Calidad
        public static class Quality
        {
            public const string View = "project.quality.view";
            public const string ManagePlan = "project.quality.plan.manage";
            public const string ManageChecklists = "project.quality.checklists.manage";
            public const string ManageNonConformities = "project.quality.nonconformities.manage";
            public const string ManageAudits = "project.quality.audits.manage";
        }

        // Riesgos
        public static class Risk
        {
            public const string View = "project.risk.view";
            public const string Create = "project.risk.create";
            public const string Edit = "project.risk.edit";
            public const string Delete = "project.risk.delete";
            public const string ManageResponses = "project.risk.responses.manage";
            public const string PerformAnalysis = "project.risk.analysis.perform";
        }

        // Reportes
        public static class Report
        {
            public const string ViewExecutiveDashboard = "report.executive.view";
            public const string ViewProjectReports = "report.project.view";
            public const string CreateCustomReports = "report.custom.create";
            public const string ExportReports = "report.export";
            public const string ViewKPIs = "report.kpis.view";
        }

        // Obtener todos los permisos
        public static List<string> GetAllPermissions()
        {
            var permissions = new List<string>();
            var permissionTypes = typeof(PermissionKeys).GetNestedTypes();

            foreach (var type in permissionTypes)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(string))
                    {
                        permissions.Add((string)field.GetValue(null)!);
                    }
                }
            }

            return permissions;
        }
    }
}