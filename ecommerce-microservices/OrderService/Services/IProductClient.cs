namespace OrderService.Services
{
    public interface IProductClient
    {
        Task<bool> ProductExistsAsync(int productId);
        Task<ProductInfo> GetProductAsync(int productId);
    }

    public class ProductInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
