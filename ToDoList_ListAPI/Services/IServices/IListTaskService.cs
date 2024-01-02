using System.Linq.Expressions;
using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Models.DTO;

namespace ToDoList_ListAPI.Services.IServices
{
    public interface IListTaskService 
    {
        Task<List<ListTaskDTO>> GetAllAsync(string? category, string? search, int pageSize = 0, int pageNumber = 1);
        Task<ListTaskDTO> GetAsync(int id );
        Task<ListTaskDTO> CreateAsync(ListTaskCreateDTO entity);
        Task RemoveAsync(int id);
        Task SaveAsync();

        Task UpdateAsync(int id , ListTaskUpdateDTO task);
    }
}
