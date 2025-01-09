using GKTodoManager.Domain.Dtos;
using GKTodoManager.Domain.Entities;

namespace GKTodoManager.Domain.Abstractions;

public interface ITaskGroupRepository
{
    Task<TaskGroup?> FindAsync(int Id, CancellationToken ct);
    Task<IEnumerable<TaskGroup>> FindAllAsync(CancellationToken ct);
    Task<TaskGroup> CreateAsync(CreateTaskGroupRequestDto request, CancellationToken ct);
    Task<TaskGroup> UpdateAsync(UpdateTaskGroupRequestDto request, CancellationToken ct);
    Task<int> DeleteAsync(int taskId, CancellationToken ct);
}
