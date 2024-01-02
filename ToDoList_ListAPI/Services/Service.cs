using System.Linq.Expressions;
using ToDoList_ListAPI.Repository.IRepository;
using ToDoList_ListAPI.Services.IServices;

namespace ToDoList_ListAPI.Services
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IRepository<T> _repo;

        public Service(IRepository<T> repo) {
            _repo= repo;
        }

        public Task CreateAsync(T entity)
        {
            return _repo.CreateAsync(entity);
        }
        public Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true, string? includProperties = null)
        {
            return _repo.GetAsync(filter, tracked, includProperties);
        }

        public Task RemoveAsync(T entity)
        {
            return _repo.RemoveAsync(entity);
        }

        public Task SaveAsync()
        {
            return _repo.SaveAsync();
        }

        async Task<List<T>> IService<T>.GetAllAsync(string? category, string? search, int pageSize, int pageNumber)
        {
            List<T> list = (List<T>) await _repo.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);
            return list;
        }
    }
}
