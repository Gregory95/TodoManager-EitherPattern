using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GKTodoManager.Domain.Dtos;
using GKTodoManager.Domain.Base;
using GKTodoManager.Application.Abstractions;
using System.Net;

namespace GKTodoManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController(IAuthService service) : ControllerBase
{
    private readonly IAuthService _service = service;

    [HttpPut("unlock-user")]
    [ProducesResponseType(typeof(RegisterUserResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> UnlockUserAsync([FromBody] UsernameRequstDto request)
    {
        var result = await _service.UnlockUserAsync(request.Username);
  
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

    [HttpDelete("delete-user/{userName}")]
    [ProducesResponseType(typeof(RegisterUserResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> DeleteUserAsync(string userName)
    {
        var result = await _service.DeleteUserAsync(userName);

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
