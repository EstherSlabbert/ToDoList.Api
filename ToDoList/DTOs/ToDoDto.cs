using ToDoList.Common;

namespace ToDoList.DTOs
{
    public class ToDoDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
        public bool IsCompleted { get; set; }
        public int ProjectId { get; set; }

        public ToDoDto(string title, string description, DateTime dueDate, int priority, int projectId, bool isCompleted = false) 
        {
            title.ThrowIfNullOrWhiteSpace();
            projectId.ThrowIfNull();

            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            ProjectId = projectId;
            IsCompleted = isCompleted;
        }
    }
}
