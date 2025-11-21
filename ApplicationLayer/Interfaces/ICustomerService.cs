using PerfumeryBackend.ApplicationLayer.DTO.Auth;

namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface ICustomerService
{
    Task<CustomerData?> GetCabinet(int customerId);
}
