using System.Linq.Expressions;
using ToDoList.Common.Specifications;
using ToDoList.Entities;

namespace ToDoList.Specifications.OfToDos
{
    public class ByProjectId : Specification<ToDo>
    {
        private readonly int projectId;

        public ByProjectId(int projectId)
        {
            this.projectId = projectId;
        }

        public override Expression<Func<ToDo, bool>> Rule()
        {
            return x => x.Project.Id == projectId;
        }
    }
}