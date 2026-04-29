using System;
using System.Collections.Generic;

namespace HRMS.Models;

public partial class Skill
{
    public int SkillId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<PossessesSkill> PossessesSkills { get; set; } = new List<PossessesSkill>();
}
