using Microsoft.EntityFrameworkCore;
using ToDoList.Entities;

namespace ToDoList.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // The DbSet<Entity> is the way EntityFramework defines new tables and their contents
        public DbSet<Project> Projects { get; set; }
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<UnhandledException> UnhandledExceptions { get; set; }
    }
}
