using ExpenseTrackerApi.DTOs;
using ExpenseTrackerApi.Services;
using Microsoft.Extensions.Configuration;

namespace ExpenseTrackerApi.Tests;

public class AuthServiceTests
{
    [Fact]
    public void Login_ShouldReturnToken_WhenCredentialsAreCorrect()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Username"] = "admin",
                ["Jwt:Password"] = "1234",
                ["Jwt:Key"] = "my-super-secret-key-123456789-ABCD"
            })
            .Build();

        var service = new AuthService(configuration);

        var dto = new LoginDto
        {
            Username = "admin",
            Password = "1234"
        };

        var token = service.Login(dto);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void Login_ShouldThrow_WhenCredentialsAreIncorrect()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Username"] = "admin",
                ["Jwt:Password"] = "1234",
                ["Jwt:Key"] = "my-super-secret-key-123456789-ABCD"
            })
            .Build();

        var service = new AuthService(configuration);

        var dto = new LoginDto
        {
            Username = "admin",
            Password = "wrong-password"
        };

        Assert.Throws<UnauthorizedAccessException>(
            () => service.Login(dto));
    }

    [Fact]
    public void Login_ShouldThrow_WhenUsernameIsIncorrect()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Username"] = "admin",
                ["Jwt:Password"] = "1234",
                ["Jwt:Key"] = "my-super-secret-key-123456789-ABCD"
            })
            .Build();

        var service = new AuthService(configuration);

        var dto = new LoginDto
        {
            Username = "wrong-user",
            Password = "1234"
        };

        Assert.Throws<UnauthorizedAccessException>(
            () => service.Login(dto));
    }

    [Fact]
    public void Login_ShouldCreateTokenWithCorrectClaims()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Username"] = "admin",
                ["Jwt:Password"] = "1234",
                ["Jwt:Key"] = "my-super-secret-key-123456789-ABCD"
            })
            .Build();

        var service = new AuthService(configuration);

        var dto = new LoginDto
        {
            Username = "admin",
            Password = "1234"
        };

        var token = service.Login(dto);

        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal("admin", jwt.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Name).Value);
        Assert.Equal("Admin", jwt.Claims.First(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value);
    }

    [Fact]
    public void Login_ShouldSetExpiration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Username"] = "admin",
                ["Jwt:Password"] = "1234",
                ["Jwt:Key"] = "my-super-secret-key-123456789-ABCD"
            })
            .Build();

        var service = new AuthService(configuration);

        var dto = new LoginDto
        {
            Username = "admin",
            Password = "1234"
        };

        var token = service.Login(dto);

        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }
}