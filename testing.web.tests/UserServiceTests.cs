using Moq;
using testing_web;

namespace testing.web.tests;
public class UserServiceTests
{
    [Fact]
    public void SaveUser_givingExpectedResult()
    {
        //Arrange
        var repositoryObj = new Mock<IUserRepository>();

        repositoryObj.Setup(repo => repo.SaveUserDetail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
        var service = new UserService(repositoryObj.Object);


        //IUserRepository realRepo = new UserRepository();
        //var service = new UserService(realRepo);
        //Act
        //Assert
       
         var result = service.SaveUser("", "pass");
        
        Assert.False(result);

        repositoryObj.Verify(
        x => x.SaveUserDetail(It.IsAny<string>(), It.IsAny<string>()),Times.Never);

    }

    [Fact]
    public async Task GetUserById_ReturnsExpectedResult()
    {
        //Arrange
        var reposeMoq= new Mock<IUserRepository>();
        var ob = new User()
        {
            Id = 1,
            Name = "Jhon snow",
            Age = 0,
            Date = DateTime.Now,
            IsActive = true,
        };
        reposeMoq.Setup(repo => repo.GetUserByid(1)).
         ReturnsAsync(ob);

        var service = new UserService(reposeMoq.Object);

        //Act 
        var result = await service.GetUserById(1);
        //Assert
        Assert.Equal("Jhon snow", result.Name);

        Assert.Null(result);
        
        reposeMoq.Verify(
            x => x.GetUserByid(1),
            Times.Once);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetUserById(0));

    }
}
