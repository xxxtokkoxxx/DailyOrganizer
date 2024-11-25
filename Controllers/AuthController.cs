using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    //POST: api/Auth
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        UserModel user = await _userService.VerifyPasswordAndGetuser(model.Username, model.Password);

        if (user == null)
        {

            return Unauthorized(new { message = "Invalid credentials" });
        }

        string token = GenerateJwtToken(user.Id.ToString());
        return Ok(new { token });
    }


    // POST: api/Auth
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("register")]
    public async Task<ActionResult<UserModel>> RegisterUser(UserModel user)
    {
        if (await _userService.RegisterUser(user.Name, user.Password))
        {
            return Ok("Registration successful");
        }
        return BadRequest("User already exists");
    }

    private string GenerateJwtToken(string userId)
    {
        string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
            new Claim("issuedAt", DateTime.UtcNow.ToString()), 
            new Claim("randomNonce", new Random().Next(100000, 999999).ToString())
        }),
            Expires = DateTime.UtcNow.AddHours(20),
            SigningCredentials = creds
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}