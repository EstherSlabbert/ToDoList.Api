using System.Linq.Expressions;
using ToDoList.Common.Specifications;
using ToDoList.Entities;

namespace ToDoList.Specifications.OfToDos
{
    public class ByProject : Specification<ToDo>
    {
        private readonly Project project;

        public ByProject(Project project)
        {
            this.project = project;
        }

        public override Expression<Func<ToDo, bool>> Rule()
        {
            return x => x.Project == project;
        }
    }
}