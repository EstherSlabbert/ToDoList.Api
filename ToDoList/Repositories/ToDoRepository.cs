using Microsoft.EntityFrameworkCore;
using ToDoList.Common;
using ToDoList.Common.Specifications;
using ToDoList.Data;
using ToDoList.Entities;

namespace ToDoList.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly DataContext _context;

        public ToDoRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ToDo>> GetAllToDosAsync()
        {
            return await _context.ToDos
                .Include(t => t.Project) // Eager load the Project
                .ToListAsync();
        }

        public async Task<ToDo?> GetToDoByIdAsync(int id)
        {
            return await _context.ToDos
                .Include(t => t.Project) // Eager load the Project
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<ToDo> AddToDoAsync(ToDo newToDo)
        {
            _context.ToDos.Add(newToDo);
            await _context.SaveChangesAsync();

            return newToDo;
        }

        public async Task<ToDo?> UpdateToDoAsync(ToDo updatedToDo)
        {
            _context.ToDos.Update(updatedToDo);
            await _context.SaveChangesAsync();

            return updatedToDo;
        }

        public async Task RemoveToDoAsync(int id)
        {
            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo is not null)
            {
                _context.ToDos.Remove(toDo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(params Specification<ToDo>[] filter)
        {
            filter.ThrowIfNull();

            var query = FilteredQuery(filter).Include(t => t.Project);
            var result = await query.AnyAsync();

            return result;
        }

        public async Task<List<ToDo>> Filter(params Specification<ToDo>[] filter)
        {
            var query = FilteredQuery(filter).Include(t => t.Project);
            var result = await query.ToArrayAsync();
            return result.ToList();
        }

        protected virtual IQueryable<ToDo> FilteredQuery(params Specification<ToDo>[] filter)
        {
            var query = _context.Set<ToDo>().AsQueryable();

            foreach (var specification in filter)
            {
                query = query.Where(specification.Rule());
            }

            return query;
        }
    }
}
