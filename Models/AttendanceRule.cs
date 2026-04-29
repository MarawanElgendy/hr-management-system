using System;
using System.Collections.Generic;

namespace HRMS.Models
{
    public partial class AttendanceRule
    {
        public string RuleName { get; set; } = null!;
        public string? Value { get; set; }
        public int? NumericValue { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
