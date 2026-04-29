using DTOs;

namespace HRMS.ViewModels
{
    public class ContractsDashboardViewModel
    {
        public List<ExpiringContractDTO> ActiveContracts { get; set; } = new();
        public List<ExpiringContractDTO> ExpiringContracts { get; set; } = new();
        public int ExpiringDaysFilter { get; set; } = 30;
    }
}
