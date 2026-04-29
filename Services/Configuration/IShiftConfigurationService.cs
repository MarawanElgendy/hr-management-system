namespace Services.Configuration;

using DTOs;

public interface IShiftConfigurationService
{
    Task ConfigureShiftAllowanceAsync(string shiftType, decimal allowanceAmount, int createdBy);
    Task<List<ShiftConfigurationDTO>> GetAllShiftTypesAsync();
}
