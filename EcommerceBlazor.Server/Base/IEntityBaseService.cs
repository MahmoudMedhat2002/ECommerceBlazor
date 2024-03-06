namespace EcommerceBlazor.Server.Base
{
	public interface IEntityBaseService<T> where T : class , IEntityBase , new()
	{
		Task<List<T>> GetAllAsync();
		Task<T> GetByIdAsync(int id);
        Task InsertAsync(T entity);
		Task UpdateAsync(int id , T entity);
		Task DeleteAsync(int id);
    }
}
