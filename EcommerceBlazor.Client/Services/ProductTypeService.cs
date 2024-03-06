using EcommerceBlazor.Shared;
using System.Net.Http.Json;

namespace EcommerceBlazor.Client.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly HttpClient _http;

        public ProductTypeService(HttpClient http)
        {
            _http = http;
        }
        public List<ProductType> ProductTypes { get; set; } = new List<ProductType>();

        public event Action OnChange;

		public async Task AddProductType(ProductType productType)
		{
			var response = await _http.PostAsJsonAsync("/api/ProductType", productType);
			ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
			OnChange.Invoke();
		}

		public ProductType CreateNewProductType()
		{
			ProductType newProductType = new ProductType { IsNew = true , Editing = true };
			ProductTypes.Add(newProductType);
			OnChange.Invoke();
			return newProductType;
		}

		public async Task GetProductTypes()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<List<ProductType>>>("/api/ProductType");
            ProductTypes = response.Data;
        }

		public async Task UpdateProductType(ProductType productType)
		{
			var response = await _http.PutAsJsonAsync("/api/ProductType", productType);
			ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
			OnChange.Invoke();
		}
	}
}
