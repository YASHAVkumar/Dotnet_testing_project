using Moq;
using testing_web;

namespace testing.web.tests;

public class UserServiceTests
{
    [Fact]
    public void GetUser_ExistingUser_ReturnsUser()
    {
        // Arrange
        var repositoryMock = new Mock<IUserRepository>();

        repositoryMock.Setup(x => x.SaveUserDetail("jhson","snow")).Returns(true);

        var service = new UserService(repositoryMock.Object);

        // Act
        var result = service.SaveUser("jhson","snow");

        // Assert
        Assert.True(result);

        var result1 = service.SaveUser("","snow");
        // Assert
        Assert.True(result);
    }
}
