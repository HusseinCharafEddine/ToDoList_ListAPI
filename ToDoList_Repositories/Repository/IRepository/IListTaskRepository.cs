using ToDoList_ListAPI.Models;

namespace ToDoList_ListAPI.Repository.IRepository
{
    public interface IListTaskRepository : IRepository<ListTask>
    {
        Task<ListTask> UpdateAsync(ListTask task);
    }
}
