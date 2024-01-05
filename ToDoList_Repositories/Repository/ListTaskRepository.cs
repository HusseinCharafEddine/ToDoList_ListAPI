using ToDoList_ListAPI.Data;
using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Repository.IRepository;

namespace ToDoList_ListAPI.Repository
{
    public class ListTaskRepository : Repository<ListTask>, IListTaskRepository
    {
        private readonly ApplicationDbContext _db;
        public ListTaskRepository(ApplicationDbContext db): base(db) {
            _db = db;
        }
        public async Task<ListTask> UpdateAsync (ListTask task)
        {
            task.UpdatedDate = DateTime.Now;
            _db.ListTasks.Update(task);
            await _db.SaveChangesAsync();
            return task;
        }
    }
}
