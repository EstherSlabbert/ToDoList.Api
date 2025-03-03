using ToDoList.Common.Specifications;
using ToDoList.Entities;

namespace ToDoList.Specifications.OfToDos
{
    public class AllSpecifications
    {
        public static Specification<ToDo> ByProject(Project project) => new ByProject(project);
        public static Specification<ToDo> ByProjectId(int projectId) => new ByProjectId(projectId);
        public static Specification<ToDo> All() => Specification<ToDo>.identification;
    }
}
