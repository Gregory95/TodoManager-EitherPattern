
using GKTodoManager.Application.Abstractions;
using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GKTodoManager.API.Controllers;



[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(IAuthService service) : ControllerBase
{
    private readonly IAuthService _service = service;

    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> GetAccessTokenAsync([FromBody] LoginRequestDto request)
    {
        var result = await _service.GetAccessTokenAsync(request);

        if (result.IsSuccess)
        {
            return Ok(result.Success);
        }

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> LoginAsync([FromBody] LoginRequestDto request)
    {
        var result = await _service.LoginUserAsync(request);

        if (result.IsSuccess)
        {
            return Ok(result.Success);
        }

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> RegisterAsync([FromBody] RegisterUserRequestDto request)
    {
        var result = await _service.RegisterNewUserAsync(request);

        if (result.IsSuccess)
        {
            return Ok(result.Success);
        }

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }
}
