using Business;
using Entity.DTOs.Login;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserBusiness _userBusiness;  // Usamos la capa de Business

    public AuthController(UserBusiness userBusiness)
    {
        _userBusiness = userBusiness;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var user = await _userBusiness.ValidateCredentialsAsync(login.Email, login.Password);
        if (user == null)
            return Unauthorized("Credenciales inválidas.");

        return Ok(user);
    }
    
}
