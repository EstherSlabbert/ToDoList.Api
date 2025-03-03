using ToDoList.Common;

namespace ToDoList.Entities
{
    public class ToDo : Entity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
        public bool IsCompleted { get; set; }
        public Project Project { get; set; }
        public ToDo() { }
        public ToDo(string title, string description, DateTime dueDate, int priority, Project project, bool isCompleted = false)
        {
            title.ThrowIfNullOrWhiteSpace();
            project.ThrowIfNull();
            
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Project = project;
            IsCompleted = isCompleted;
        }

        public void UpdateToDo(string title, string description, DateTime dueDate, int priority, Project project, bool isCompleted)
        {
            title.ThrowIfNullOrWhiteSpace();
            project.ThrowIfNull();

            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Project = project;
            IsCompleted = isCompleted;
        }
    }
}
