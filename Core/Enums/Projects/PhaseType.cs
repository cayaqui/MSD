using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums.Projects;

#region Phase Enums
/// <summary>
/// Project Phase Type according to PMBOK
/// </summary>
public enum PhaseType
{
    Initiation = 1,
    Planning = 2,
    Execution = 3,
    MonitoringAndControl = 4,
    Closing = 5,
    // Engineering & Construction specific
    ConceptualDesign = 10,
    BasicEngineering = 11,
    DetailedEngineering = 12,
    Procurement = 13,
    Construction = 14,
    Commissioning = 15,
    Handover = 16
}


#endregion
