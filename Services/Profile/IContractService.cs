using DTOs;

public interface IContractService
{
    Task<int> CreateEmploymentContractAsync(int employeeID, string type, DateTime startDate, DateTime endDate);
    Task<List<ExpiringContractDTO>> GetActiveContractsAsync();
    Task<List<ExpiringContractDTO>> GetExpiringContractsAsync(int daysBefore);
    Task RenewContractAsync(int contractID, DateTime newEndDate);
    Task NotifyExpiringContractsAsync(int daysBefore);
}
