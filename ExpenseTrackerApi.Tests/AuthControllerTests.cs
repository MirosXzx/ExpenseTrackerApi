using ExpenseTrackerApi.Controllers;
using ExpenseTrackerApi.DTOs;
using ExpenseTrackerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerApi.Tests;

public class AuthControllerTests
{
    [Fact]
    public void Login_ShouldReturnOk_WhenCredentialsAreCorrect()
    {
        var service = new FakeAuthService();
        var controller = new AuthController(service);

        var dto = new LoginDto
        {
            Username = "admin",
            Password = "1234"
        };

        var result = controller.Login(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void Login_ShouldReturnUnauthorized_WhenCredentialsAreIncorrect()
    {
        var service = new FakeAuthService();
        var controller = new AuthController(service);

        var dto = new LoginDto
        {
            Username = "wrong",
            Password = "wrong"
        };

        var result = controller.Login(dto);

        Assert.IsType<UnauthorizedResult>(result);
    }

    private class FakeAuthService : IAuthService
    {
        public string Login(LoginDto dto)
        {
            if (dto.Username != "admin" || dto.Password != "1234")
            {
                throw new UnauthorizedAccessException();
            }

            return "test-token";
        }
    }
}