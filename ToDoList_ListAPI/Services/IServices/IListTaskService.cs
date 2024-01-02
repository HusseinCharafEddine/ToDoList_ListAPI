using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Models.DTO;

namespace ToDoList_ListAPI.Services.IServices
{
    public interface IListTaskService : IService<ListTask>
    {

        
        Task UpdateAsync(int id , ListTask task);
    }
}
