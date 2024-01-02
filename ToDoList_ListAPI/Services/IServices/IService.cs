using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ToDoList_ListAPI.Services.IServices
{
    public interface IService <T> where T : class
    {
        Task<List<T>> GetAllAsync(string? category, string? search, int pageSize = 0, int pageNumber = 1);
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, string? includProperties = null);
        Task<T> CreateAsync(T entity);
        //Task RemoveAsync(int id );
        Task SaveAsync();
    }
}
