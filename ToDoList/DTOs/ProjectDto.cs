using ToDoList.Common;

namespace ToDoList.DTOs
{
    public class ProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ProjectDto(string name, string description = "")
        {
            name.ThrowIfNullOrWhiteSpace();

            Name = name;
            Description = description;
        }
    }
}
