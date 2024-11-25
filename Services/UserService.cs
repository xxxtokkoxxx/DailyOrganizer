
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private const int IncorectId = -1;

    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> RegisterUser(string username, string password)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Name == username))
            return false;

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        UserModel user = new UserModel
        {
            Name = username,
            Password = passwordHash
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<UserModel> VerifyPasswordAndGetuser(string username, string password)
    {
        UserModel? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == username);

        if (user == null)
            return null;

        bool isVerified = BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!isVerified)
            return null;

        return user;
    }

    public int GetUserId(ClaimsPrincipal user)
    {
        string? userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        bool isParsed = int.TryParse(userId, out int id);

        if(!isParsed)
            id = IncorectId;

        return id;
    }
}
