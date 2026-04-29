namespace Services.General;

using HRMS.Models;

public class AnalyticsService : IAnalyticsService
{
    private readonly HrmsContext context;

    public AnalyticsService(HrmsContext context)
    {
        this.context = context;
    }
}
