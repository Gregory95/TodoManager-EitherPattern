using AutoMapper;
using GKTodoManager.Application.Abstractions;
using GKTodoManager.Domain.Abstractions;
using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Dtos;
using System.Net;


namespace GKTodoManager.Application.Services;

public class TaskGroupService(ITaskGroupRepository taskGroupRepo, IMapper mapper) : ITaskGroupService
{
    private readonly ITaskGroupRepository _taskGroupRepo = taskGroupRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<Either<TaskGroupResponseDto, ErrorResponse>> CreateTaskGroupAsync(CreateTaskGroupRequestDto message, CancellationToken ct)
    {
        try
        {
            var newTaskGroup = await _taskGroupRepo.CreateAsync(message, ct);

            return Either<TaskGroupResponseDto, ErrorResponse>.Succeeded(_mapper.Map<TaskGroupResponseDto>(newTaskGroup));
        }
        catch (Exception ex)
        {
            return Either<TaskGroupResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "Task group creation failed.",
                Description = ex.Message,
                Status = HttpStatusCode.BadRequest
            });
        }
    }

    public async Task<Either<int, ErrorResponse>> DeleteTaskGroupAsync(int Id, CancellationToken ct)
    {
        try
        {
            var taskGroup = await _taskGroupRepo.FindAsync(Id, ct);

            if (taskGroup == null)
            {
                return Either<int, ErrorResponse>.Failure(new ErrorResponse
                {
                    Title = "Task group not found",
                    Description = $"Task group with Id {Id} not found in the system.",
                    Status = HttpStatusCode.NotFound
                });
            }

            var deletedTaskGroup = await _taskGroupRepo.DeleteAsync(Id, ct);

            return Either<int, ErrorResponse>.Succeeded(deletedTaskGroup);
        }
        catch (Exception ex)
        {
            return Either<int, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "System could not delete task group.",
                Description = ex.Message,
                Status = HttpStatusCode.BadRequest
            });
        }
    }

    public async Task<Either<TaskGroupResponseDto, ErrorResponse>> GetTaskGroupByIdAsync(int Id, CancellationToken ct)
    {
        var taskGroup = await _taskGroupRepo.FindAsync(Id, ct);

        if (taskGroup == null)
        {
            return Either<TaskGroupResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "Task group not found",
                Description = $"Task group with Id {Id} not found in the system.",
                Status = HttpStatusCode.NotFound
            });
        }

        return Either<TaskGroupResponseDto, ErrorResponse>.Succeeded(_mapper.Map<TaskGroupResponseDto>(taskGroup));
    }

    public async Task<Either<IEnumerable<TaskGroupResponseDto>, ErrorResponse>> GetTaskGroupsAsync(CancellationToken ct)
    {
        var taskGroupsList = await _taskGroupRepo.FindAllAsync(ct);

        return Either<IEnumerable<TaskGroupResponseDto>, ErrorResponse>
            .Succeeded(_mapper.Map<IEnumerable<TaskGroupResponseDto>>(taskGroupsList));
    }

    public async Task<Either<TaskGroupResponseDto, ErrorResponse>> UpdateTaskGroupAsync(UpdateTaskGroupRequestDto message, CancellationToken ct)
    {
        try
        {
            var taskGroup = await _taskGroupRepo.FindAsync(message.Id, ct);

            if (taskGroup == null)
            {
                return Either<TaskGroupResponseDto, ErrorResponse>.Failure(new ErrorResponse
                {
                    Title = "Task group not found",
                    Description = $"Task group with Id {message.Id} not found in the system.",
                    Status = HttpStatusCode.NotFound
                });
            }

            var updatedTaskGroup = await _taskGroupRepo.UpdateAsync(message, ct);

            return Either<TaskGroupResponseDto, ErrorResponse>.Succeeded(_mapper.Map<TaskGroupResponseDto>(updatedTaskGroup));
        }
        catch (Exception ex)
        {
            return Either<TaskGroupResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "System could not update task group.",
                Description = ex.Message,
                Status = HttpStatusCode.BadRequest
            });
        }
    }
}
