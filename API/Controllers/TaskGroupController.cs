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
public class TaskGroupController(ITaskGroupService taskGroupService) : ControllerBase
{
    private readonly ITaskGroupService _taskGroupService = taskGroupService;

    [HttpPost]
    [ProducesResponseType(typeof(TaskGroupResponseDto), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> CreateTaskGroupAsync([FromBody] CreateTaskGroupRequestDto request, CancellationToken ct)
    {
        var result = await _taskGroupService.CreateTaskGroupAsync(request, ct);

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

    [HttpPut]
    [ProducesResponseType(typeof(TaskGroupResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> UpdateTaskGroupAsync([FromBody] UpdateTaskGroupRequestDto request, CancellationToken ct)
    {
        var result = await _taskGroupService.UpdateTaskGroupAsync(request, ct);

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

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskGroupResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> GetTaskGroupByIdAsync(int id, CancellationToken ct)
    {
        var result = await _taskGroupService.GetTaskGroupByIdAsync(id, ct);

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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskGroupResponseDto>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> GetTaskGroupsAsync(CancellationToken ct)
    {
        var result = await _taskGroupService.GetTaskGroupsAsync(ct);

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

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(TaskGroupResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> DeleteTaskGroupAsync(int id, CancellationToken ct)
    {
        var result = await _taskGroupService.DeleteTaskGroupAsync(id, ct);

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
