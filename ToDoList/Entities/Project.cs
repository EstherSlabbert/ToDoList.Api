using System.Collections.Immutable;
using ToDoList.Common;

namespace ToDoList.Entities
{
    public class Project : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        private readonly IList<ToDo> toDos = new List<ToDo>();
        public virtual IReadOnlyList<ToDo> ToDos => toDos.ToImmutableList();
        public Project() { }
        public Project(string name, string description = "")
        {
            name.ThrowIfNullOrWhiteSpace();

            Name = name;
            Description = description;
        }

        public void UpdateProject(string name, string description = "")
        {
            name.ThrowIfNullOrWhiteSpace();

            Name = name;
            Description = description;
        }
    }
}
