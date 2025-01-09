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
public class TaskController(ITaskService taskService) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;

    [HttpPost]
    [ProducesResponseType(typeof(TaskResponseDto), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> CreateTaskAsync([FromBody] CreateTaskRequestDto request, CancellationToken ct)
    {
        var result = await _taskService.CreateTaskAsync(request, ct);

        if (result.IsSuccess) return Ok(result.Success);

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }

    [HttpPut]
    [ProducesResponseType(typeof(TaskResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> UpdateTaskAsync([FromBody] UpdateTaskRequestDto request, CancellationToken ct)
    {
        var result = await _taskService.UpdateTaskAsync(request, ct);

        if (result.IsSuccess) return Ok(result.Success);

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }

    [HttpPatch("group")]
    [ProducesResponseType(typeof(TaskResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> UpdateTaskGroupAsync([FromBody] UpdateTaskGroupDto request, CancellationToken ct)
    {
        var result = await _taskService.UpdateTaskGroupAsync(request, ct);

        if (result.IsSuccess) return Ok(result.Success);

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }

    [HttpPatch("status")]
    [ProducesResponseType(typeof(TaskResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> UpdateTaskStatusAsync([FromBody] UpdateTaskStatusDto request, CancellationToken ct)
    {
        var result = await _taskService.UpdateTaskStatusAsync(request, ct);

        if (result.IsSuccess) return Ok(result.Success);

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> GetTaskByIdAsync(int id, CancellationToken ct)
    {
        var result = await _taskService.GetTaskByIdAsync(id, ct);

        if (result.IsSuccess) return Ok(result.Success);

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> GetTasksAsync(CancellationToken ct)
    {
        var result = await _taskService.GetTasksAsync(ct);

        if (result.IsSuccess) return Ok(result.Success);

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> DeleteTaskAsync(int id, CancellationToken ct)
    {
        var result = await _taskService.DeleteTaskAsync(id, ct);

        if (result.IsSuccess) return Ok(result.Success);

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }

    [HttpGet("group/{groupId}")]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<ActionResult> GetGrouppedTasksAsync(int groupId, CancellationToken ct)
    {
        var result = await _taskService.GetGrouppedTasksAsync(groupId, ct);

        if (result.IsSuccess) return Ok(result.Success);

        return result.Error.Status switch
        {
            HttpStatusCode.NotFound => NotFound(result.Error),
            HttpStatusCode.Unauthorized => Unauthorized(result.Error),
            HttpStatusCode.Conflict => Conflict(result.Error),
            _ => BadRequest(result.Error),
        };
    }
}
