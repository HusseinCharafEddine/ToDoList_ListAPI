using Microsoft.EntityFrameworkCore;
using ToDoList_Utility.Models;

namespace ToDoList_Repository.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base (options) { 
        }
        public DbSet<LocalUser> LocalUsers { get; set; }
        public DbSet<ListTask> ListTasks { get; set; }

    }
}
