using EcommerceBlazor.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EcommerceBlazor.Server.Base
{
	public class EntityBaseService<T> : IEntityBaseService<T> where T : class,IEntityBase,new()
	{

		private readonly AppDbContext _context;

		public EntityBaseService(AppDbContext context)
		{
			_context = context;
		}

		public async Task DeleteAsync(int id)
		{
			var entity = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
			EntityEntry entry = _context.Entry(entity);
			entry.State = EntityState.Deleted;
			await _context.SaveChangesAsync();
		}

		public async Task<List<T>> GetAllAsync()
		{
			var items = await _context.Set<T>().ToListAsync();
			return items;
		}

		public async Task<T> GetByIdAsync(int id)
		{
			var item = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
			return item;
		}

		public async Task InsertAsync(T entity)
		{
			await _context.Set<T>().AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(int id, T entity)
		{
			EntityEntry entry = _context.Entry<T>(entity);
			entry.State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}
	}
}
