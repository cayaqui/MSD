using Core.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Projects
{
    public class ProjectTeamMemberDetailDto : ProjectTeamMemberDto
    {
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string? UserJobTitle { get; set; }
        public DateTime AssignedAt { get; set; }
        public string? AssignedBy { get; set; }
    }
}
