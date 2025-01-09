using AutoMapper;
using GKTodoManager.Application.Abstractions;
using GKTodoManager.Domain.Abstractions;
using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Dtos;
using System.Net;

namespace GKTodoManager.Application.Services;

public class TaskService(ITaskRepository taskRepository, IMapper mapper, ITaskGroupRepository taskGroupRepository) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly ITaskGroupRepository _taskGroupRepository = taskGroupRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<Either<TaskResponseDto, ErrorResponse>> CreateTaskAsync(CreateTaskRequestDto message, CancellationToken ct)
    {
        try
        {
            var newTask = await _taskRepository.CreateAsync(message, ct);

            return Either<TaskResponseDto, ErrorResponse>.Succeeded(_mapper.Map<TaskResponseDto>(newTask));
        }
        catch(Exception ex)
        {
            return Either<TaskResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "Task creation failed.",
                Description = ex.Message,
                Status = HttpStatusCode.BadRequest
            });
        }        
    }

    public async Task<Either<int, ErrorResponse>> DeleteTaskAsync(int Id, CancellationToken ct)
    {
        try
        {
            var task = await _taskRepository.FindAsync(Id, ct);

            if (task == null)
            {
                return Either<int, ErrorResponse>.Failure(new ErrorResponse
                {
                    Title = "Task not found",
                    Description = $"Task {Id} not found in the system.",
                    Status = HttpStatusCode.NotFound
                });
            }

            var deletedTask = await _taskRepository.DeleteAsync(Id, ct);

            return Either<int, ErrorResponse>.Succeeded(deletedTask);
        }
        catch (Exception ex)
        {
            return Either<int, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "System could not delete task.",
                Description = ex.Message,
                Status = HttpStatusCode.BadRequest
            });
        }
    }

    public async Task<Either<TaskResponseDto, ErrorResponse>> GetTaskByIdAsync(int Id, CancellationToken ct)
    {
        var task = await _taskRepository.FindAsync(Id, ct);

        if (task == null)
        {
            return Either<TaskResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "Task not found",
                Description = $"Task {Id} not found in the system.",
                Status = HttpStatusCode.NotFound
            });
        }

        return Either<TaskResponseDto, ErrorResponse>.Succeeded(_mapper.Map<TaskResponseDto>(task));
    }

    public async Task<Either<IEnumerable<TaskResponseDto>, ErrorResponse>> GetTasksAsync(CancellationToken ct)
    {
        var tasksList = await _taskRepository.FindAllAsync(ct);

        return Either<IEnumerable<TaskResponseDto>, ErrorResponse>
            .Succeeded(_mapper.Map<IEnumerable<TaskResponseDto>>(tasksList));
    }

    public async Task<Either<TaskResponseDto, ErrorResponse>> UpdateTaskAsync(UpdateTaskRequestDto message, CancellationToken ct)
    {
        try
        {
            var task = await _taskRepository.FindAsync(message.Id, ct);

            if (task == null)
            {
                return Either<TaskResponseDto, ErrorResponse>.Failure(new ErrorResponse
                {
                    Title = "Task not found",
                    Description = $"Task {message.Id} not found in the system.",
                    Status = HttpStatusCode.NotFound
                });
            }

            var updatedTask = await _taskRepository.UpdateAsync(message, ct);

            return Either<TaskResponseDto, ErrorResponse>.Succeeded(_mapper.Map<TaskResponseDto>(updatedTask));
        }
        catch (Exception ex)
        {
            return Either<TaskResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "System could not update task.",
                Description = ex.Message,
                Status = HttpStatusCode.BadRequest
            });
        }
    }

    public async Task<Either<TaskResponseDto, ErrorResponse>> UpdateTaskStatusAsync(UpdateTaskStatusDto message, CancellationToken ct)
    {
        try
        {
            var task = await _taskRepository.ChangeTaskStatusAsync(message, ct);

            if (task == null)
            {
                return Either<TaskResponseDto, ErrorResponse>.Failure(new ErrorResponse
                {
                    Title = "Task not found",
                    Description = $"Task {message.Id} not found in the system.",
                    Status = HttpStatusCode.NotFound
                });
            }

            return Either<TaskResponseDto, ErrorResponse>.Succeeded(_mapper.Map<TaskResponseDto>(task));
        }
        catch (Exception ex)
        {
            return Either<TaskResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "System could not update task.",
                Description = ex.Message,
                Status = HttpStatusCode.BadRequest
            });
        }
    }

    public async Task<Either<TaskResponseDto, ErrorResponse>> UpdateTaskGroupAsync(UpdateTaskGroupDto message, CancellationToken ct)
    {
        try
        {
            if (message.TaskGroupId != null 
                && await _taskGroupRepository.FindAsync(message.TaskGroupId ?? 0, ct) == null)
            {
                return Either<TaskResponseDto, ErrorResponse>.Failure(new ErrorResponse
                {
                    Title = "Task group not found",
                    Description = $"Task group {message.TaskGroupId} not found in the system.",
                    Status = HttpStatusCode.NotFound
                });
            }

            var task = await _taskRepository.ChangeTaskGroupAsync(message, ct);

            if (task == null)
            {
                return Either<TaskResponseDto, ErrorResponse>.Failure(new ErrorResponse
                {
                    Title = "Task not found",
                    Description = $"Task {message.Id} not found in the system.",
                    Status = HttpStatusCode.NotFound
                });
            }

            return Either<TaskResponseDto, ErrorResponse>.Succeeded(_mapper.Map<TaskResponseDto>(task));
        }
        catch (Exception ex)
        {
            return Either<TaskResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "System could not update task.",
                Description = ex.Message,
                Status = HttpStatusCode.BadRequest
            });
        }
    }

    public async Task<Either<IEnumerable<TaskResponseDto>, ErrorResponse>> GetGrouppedTasksAsync(int groupId, CancellationToken ct)
    {
        var grouppedTasks = await _taskRepository.FindGrouppedTasksAsync(groupId, ct);

        return Either<IEnumerable<TaskResponseDto>, ErrorResponse>
            .Succeeded(_mapper.Map<IEnumerable<TaskResponseDto>>(grouppedTasks));
    }
}