using Microsoft.EntityFrameworkCore;
using ToDoList_ListAPI.Models;

namespace ToDoList_ListAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base (options) { 
        }
        public DbSet<LocalUser> LocalUsers { get; set; }
        public DbSet<ListTask> ListTasks { get; set; }

    }
}
