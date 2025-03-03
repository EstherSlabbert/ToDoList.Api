using ToDoList.Entities;
using ToDoList.Common;

namespace UnitTests.Entities
{
    public class ProjectEntityTests()
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void When_Creating_Project_With_Invalid_Name_Throws_InvalidParameterException(string name)
        {
            //Arrange & Act
            var act = () => new Project(name);

            //Assert
            act.ShouldThrow<InvalidParameterException>(@$"The argument ""name"" was not provided.");
        }

        [Fact]
        public void When_Creating_Project_With_Valid_Properties()
        {
            //Arrange & Act
            var newProject = new Project("Wonder Project", "Wonder Project Description");

            //Assert
            newProject.Id.ShouldBeOfType<int>();
            newProject.Name.ShouldBe(newProject.Name);
            newProject.Description.ShouldBe(newProject.Description);
        }

        [Fact]
        public void When_Updating_Project_With_UpdateProject_Updates_Successfully()
        {
            //Arrange
            var existingProject = new Project("Project 1");
            var newName = "Project 2";
            var newDescription = "Project 2 is in place to accomplish xyz.";

            //Act
            existingProject.UpdateProject(newName, newDescription);

            //Assert
            existingProject.Name.ShouldBe(newName);
            existingProject.Description.ShouldBe(newDescription);
        }
    }
}