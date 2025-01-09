using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Dtos;

namespace GKTodoManager.Application.Abstractions;

public interface ITaskGroupService
{
    Task<Either<TaskGroupResponseDto, ErrorResponse>> GetTaskGroupByIdAsync(int Id, CancellationToken ct);
    Task<Either<IEnumerable<TaskGroupResponseDto>, ErrorResponse>> GetTaskGroupsAsync(CancellationToken ct);
    Task<Either<TaskGroupResponseDto, ErrorResponse>> CreateTaskGroupAsync(CreateTaskGroupRequestDto message, CancellationToken ct);
    Task<Either<int, ErrorResponse>> DeleteTaskGroupAsync(int Id, CancellationToken ct);
    Task<Either<TaskGroupResponseDto, ErrorResponse>> UpdateTaskGroupAsync(UpdateTaskGroupRequestDto message, CancellationToken ct);
}
