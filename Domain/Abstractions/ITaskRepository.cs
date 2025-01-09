using GKTodoManager.Domain.Dtos;

namespace GKTodoManager.Domain.Abstractions
{
    public interface ITaskRepository
    {
        Task<Entities.Task?> FindAsync(int Id, CancellationToken ct);
        Task<IEnumerable<Entities.Task>> FindAllAsync(CancellationToken ct);
        Task<IEnumerable<Entities.Task>> FindGrouppedTasksAsync(int groupId, CancellationToken ct);
        Task<Entities.Task> CreateAsync(CreateTaskRequestDto request, CancellationToken ct);
        Task<Entities.Task> UpdateAsync(UpdateTaskRequestDto request, CancellationToken ct);
        Task<int> DeleteAsync(int taskId, CancellationToken ct);
        Task<Entities.Task?> ChangeTaskStatusAsync(UpdateTaskStatusDto message, CancellationToken ct);
        Task<Entities.Task?> ChangeTaskGroupAsync(UpdateTaskGroupDto message, CancellationToken ct);
    }
}
