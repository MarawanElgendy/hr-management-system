using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class PossessesSkill
{
    public int EmployeeId { get; set; }

    public int SkillId { get; set; }

    public int? ProficiencyLevel { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Skill Skill { get; set; } = null!;
}
