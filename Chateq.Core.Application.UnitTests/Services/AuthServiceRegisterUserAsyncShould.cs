using Chateq.Core.Application.Services;
using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Core.Domain.Interfaces.Services;
using Chateq.Core.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chateq.Core.Application.UnitTests.Services;

public class AuthServiceRegisterUserAsyncShould
{
    [Fact]
    public async Task RegisterUserAsync_UserIsNotNull_ThrowsInvalidOperationException()
    {
        var user = new User("test", "password");

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>())).Returns(Task.FromResult(user)!);
        userRepositoryMock.Setup(r => r.AddUserAsync(user)).Returns(Task.CompletedTask);
        var authService = new AuthService(userRepositoryMock.Object, new Mock<IJwtService>().Object,
            new Mock<ILogger<AuthService>>().Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => authService.RegisterUserAsync(new RegisterUserDto()));
    }

    [Fact]
    public async Task RegisterUserAsync_UserIsNotNull_ThrowsExceptionWithCorrectMessage()
    {
        var user = new User("test", "password");

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>())).Returns(Task.FromResult(user)!);
        userRepositoryMock.Setup(r => r.AddUserAsync(user)).Returns(Task.CompletedTask);
        var authService = new AuthService(userRepositoryMock.Object, new Mock<IJwtService>().Object,
            new Mock<ILogger<AuthService>>().Object);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                authService.RegisterUserAsync(new RegisterUserDto()));
        Assert.Equal("User with this username already exists.", exception.Message);
    }

    [Fact]
    public async Task RegisterUserAsync_UnexpectedError_ThrowsInvalidProgramException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<User>(null!)!);
        userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>())).Throws(new Exception());
        var authService = new AuthService(userRepositoryMock.Object, new Mock<IJwtService>().Object,
            new Mock<ILogger<AuthService>>().Object);

        await Assert.ThrowsAsync<InvalidProgramException>(() => authService.RegisterUserAsync(new RegisterUserDto()));
    }
}