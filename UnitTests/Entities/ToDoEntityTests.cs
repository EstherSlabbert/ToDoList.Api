using ToDoList.Entities;
using ToDoList.Common;

namespace UnitTests.Entities
{
    public class ToDoEntityTests()
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void When_Creating_ToDo_With_Invalid_Title_Throws_InvalidParameterException(string name)
        {
            //Arrange & Act
            var act = () => new ToDo(name, "", new DateTime(2025, 02, 25, 14, 30, 00), 1, new Project("New Project"));

            //Assert
            act.ShouldThrow<InvalidParameterException>(@$"The argument ""name"" was not provided.");
        }

        [Fact]
        public void When_Creating_ToDo_With_Invalid_Project_Throws_InvalidParameterException()
        {
            //Arrange & Act
            var act = () => new ToDo("Title", "", new DateTime(2025, 02, 25, 14, 30, 00), 1, null);

            //Assert
            act.ShouldThrow<InvalidParameterException>(@$"The argument ""project"" was not provided.");
        }

        [Fact]
        public void When_Creating_ToDo_With_Valid_Properties()
        {
            //Arrange & Act
            var newToDo = new ToDo("Task 1", "", new DateTime(2025, 02, 25, 14, 30, 00), 1, new Project("New Project"));

            //Assert
            newToDo.Id.ShouldBeOfType<int>();
            newToDo.Title.ShouldBe(newToDo.Title);
            newToDo.Description.ShouldBe(newToDo.Description);
            newToDo.DueDate.ShouldBe(newToDo.DueDate);
            newToDo.Priority.ShouldBe(newToDo.Priority);
            newToDo.Project.ShouldBe(newToDo.Project);
        }

        [Fact]
        public void When_Updating_ToDo_With_UpdateToDo_Updates_Successfully()
        {
            //Arrange
            var existingToDo = new ToDo("Task 1", "", new DateTime(2024, 12, 12, 12, 45, 00), 2, new Project("Project 1"), false);
            var newTitle = "Task 2";
            var newDescription = "Task 2 is to accomplish xyz.";
            var newDueDate = new DateTime(2025, 02, 25, 17, 00, 00);
            var newPriority = 1;
            var newIsCompleted = true;
            var newProject = new Project("Project 2");

            //Act
            existingToDo.UpdateToDo(newTitle, newDescription, newDueDate, newPriority, newProject, newIsCompleted);

            //Assert
            existingToDo.Title.ShouldBe(newTitle);
            existingToDo.Description.ShouldBe(newDescription);
            existingToDo.DueDate.ShouldBe(newDueDate);
            existingToDo.Priority.ShouldBe(newPriority);
            existingToDo.Project.ShouldBe(newProject);
            existingToDo.IsCompleted.ShouldBe(newIsCompleted);
        }
    }
}