using ExpenseTrackerApi.DTOs;

namespace ExpenseTrackerApi.Services;

public interface IAuthService
{
    string Login(LoginDto dto);
}