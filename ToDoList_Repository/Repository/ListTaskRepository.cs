using System;
using System.Threading.Tasks;
using ToDoList_Repository.Data;
using ToDoList_Utility.Models;
using ToDoList_Repository.Repository.IRepository;

namespace ToDoList_Repository.Repository
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
