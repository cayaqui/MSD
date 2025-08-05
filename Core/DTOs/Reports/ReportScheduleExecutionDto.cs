using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Reports
{

    public class ReportScheduleExecutionDto
    {
        public Guid Id { get; set; }
        public Guid ScheduleId { get; set; }
        public Guid? ReportId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public int? RecordCount { get; set; }
    }
}
