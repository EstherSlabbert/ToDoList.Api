namespace ToDoList.DTOs
{
    public class ToDoDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
        public string ProjectName { get; set; }
        public bool IsCompleted { get; set; }
        public ToDoDetailsDto(int id, string title, string description, DateTime dueDate, int priority, string projectName, bool isCompleted)
        {
            Id = id;
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            ProjectName = projectName;
            IsCompleted = isCompleted;
        }
    }
}
