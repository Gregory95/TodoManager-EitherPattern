using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Dtos;

namespace GKTodoManager.Application.Abstractions;

public interface ITaskService
{
    Task<Either<TaskResponseDto, ErrorResponse>> GetTaskByIdAsync(int Id, CancellationToken ct);
    Task<Either<IEnumerable<TaskResponseDto>, ErrorResponse>> GetTasksAsync(CancellationToken ct);
    Task<Either<IEnumerable<TaskResponseDto>, ErrorResponse>> GetGrouppedTasksAsync(int groupId, CancellationToken ct);
    Task<Either<TaskResponseDto, ErrorResponse>> CreateTaskAsync(CreateTaskRequestDto message, CancellationToken ct);
    Task<Either<int, ErrorResponse>> DeleteTaskAsync(int Id, CancellationToken ct);
    Task<Either<TaskResponseDto, ErrorResponse>> UpdateTaskAsync(UpdateTaskRequestDto message, CancellationToken ct);
    Task<Either<TaskResponseDto, ErrorResponse>> UpdateTaskStatusAsync(UpdateTaskStatusDto message, CancellationToken ct);
    Task<Either<TaskResponseDto, ErrorResponse>> UpdateTaskGroupAsync(UpdateTaskGroupDto message, CancellationToken ct);
}