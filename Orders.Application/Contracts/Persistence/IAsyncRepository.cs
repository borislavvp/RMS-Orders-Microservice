using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Orders.Domain.Common;

namespace Orders.Application.Contracts.Persistence
{
	public interface IAsyncRepository<T> where T : EntityBase
	{
		// GET methods
		// Get all objects from a type
		Task<IReadOnlyList<T>> GetAllAsync();
		// Get - filtered by expressions
		Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);

		Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
										Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
										string includeString = null,
										bool disableTracking = true);
		Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
									   Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
									   List<Expression<Func<T, object>>> includes = null,
									   bool disableTracking = true);
		// Get type information by id
		Task<T> GetByIdAsync(int id);

		// POST/PUT/DELETE methods
		Task<T> AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);
	}
}
