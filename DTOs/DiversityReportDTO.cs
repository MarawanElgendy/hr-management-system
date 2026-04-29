namespace DTOs;

public class NationalityStatDTO
{
    public string Country { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class AgeStatDTO
{
    public string AgeGroup { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ComplianceStatDTO
{
    public double AverageCompletion { get; set; }
    public int FullyCompliant { get; set; }
    public int NonCompliant { get; set; }
    public int TotalActive { get; set; }
}

public class DiversityReportDTO
{
    public List<NationalityStatDTO> NationalityStats { get; set; } = new();
    public List<AgeStatDTO> AgeStats { get; set; } = new();
    public ComplianceStatDTO ComplianceStats { get; set; } = new();
}
