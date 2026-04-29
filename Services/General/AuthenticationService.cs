namespace Services.General;

using HRMS.Models;

public class AuthenticationService : IAuthenticationService
{
    private readonly HrmsContext context;

    public AuthenticationService(HrmsContext context)
    {
        this.context = context;
    }
}
