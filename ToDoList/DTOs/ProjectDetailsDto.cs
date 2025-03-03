namespace ToDoList.DTOs
{
    public class ProjectDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectDetailsDto(int id, string name, string description) 
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
