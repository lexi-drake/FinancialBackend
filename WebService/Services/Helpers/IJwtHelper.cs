namespace WebService
{
    public interface IJwtHelper
    {
        Token CreateToken(string userId, string role);
        string GetUserIdFromToken(string jwt);
        string GetRoleFromToken(string jwt);
    }
}