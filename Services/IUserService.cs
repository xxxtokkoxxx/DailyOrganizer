using System.Security.Claims;
using System.Threading.Tasks;

public interface IUserService
{
    Task<bool> RegisterUser(string username, string password);
    Task<UserModel> VerifyPasswordAndGetuser(string username, string password);
    int GetUserId(ClaimsPrincipal user);
}