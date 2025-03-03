using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Entities;

namespace ToDoList.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DataContext _context;

        public ProjectRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            var heroes = await _context.Projects.ToListAsync();
            return heroes;
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            return project;
        }

        public async Task<Project> AddProjectAsync(Project newProject)
        {
            _context.Projects.Add(newProject);
            await _context.SaveChangesAsync();

            return newProject;
        }

        public async Task<Project?> UpdateProjectAsync(Project updatedProject)
        {
            _context.Projects.Update(updatedProject);
            await _context.SaveChangesAsync();

            return updatedProject;
        }

        public async Task RemoveProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project is not null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }
    }
}
