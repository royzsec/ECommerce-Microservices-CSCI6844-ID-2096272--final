namespace OrderService.Services
{
    public interface ICustomerClient
    {
        Task<bool> CustomerExistsAsync(int customerId);
    }
}
