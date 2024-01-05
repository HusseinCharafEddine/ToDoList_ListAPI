using ToDoList_Utility.Models;

namespace ToDoList_Repository.Repository.IRepository
{
    public interface IListTaskRepository : IRepository<ListTask>
    {
        Task<ListTask> UpdateAsync(ListTask task);
    }
}
