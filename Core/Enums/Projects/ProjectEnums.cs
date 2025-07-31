using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.Projects;

#region Enums
public enum WBSElementStatus
{
    NotStarted,
    InProgress,
    Completed,
    OnHold,
    Cancelled,
    Delayed,
    AtRisk
}
public enum WBSReportType
{
    Summary,
    Detailed,
    Progress,
    Resource,
    Cost,
    Schedule,
    Milestone
}

public enum ExportFormat
{
    Excel,
    PDF,
    MSProject,
    CSV,
    JSON,
    XML
}

public enum WBSImportMode
{
    Replace,
    Merge,
    Append
}

#endregion; 
