namespace HenguelSistemas.Application.Interfaces;

public interface IWaitlistService
{
    Task<(bool Success, string Message)> RegisterAsync(string name, string email, bool consent);
    Task<int> GetCountAsync();
}