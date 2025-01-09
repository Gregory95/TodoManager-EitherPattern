using GKTodoManager.Domain.Abstractions;
using GKTodoManager.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GKTodoManager.Infrastructure.Repositories;

public class TaskRepository(ApplicationDbContext context) : ITaskRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Domain.Entities.Task?> FindAsync(int Id, CancellationToken ct)
    {
        return await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id, ct);
    }
    
    public async Task<IEnumerable<Domain.Entities.Task>> FindAllAsync(CancellationToken ct)
    {
        return await _context.Tasks.AsNoTracking().ToListAsync(ct);
    }

    public async Task<Domain.Entities.Task> CreateAsync(CreateTaskRequestDto request, CancellationToken ct)
    {
        var newTask = await _context.AddAsync(new Domain.Entities.Task
        { 
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status
        }, ct);

        await _context.SaveChangesAsync(ct);

        return newTask.Entity;
    }

    public async Task<int> DeleteAsync(int taskId, CancellationToken ct)
    {
        var task = await _context.Tasks.FirstAsync(x => x.Id == taskId, ct);

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(ct);

        return taskId;
    }

    public async Task<Domain.Entities.Task> UpdateAsync(UpdateTaskRequestDto message, CancellationToken ct)
    {
        var checkTaskGroup = await _context.TaskGroups.AnyAsync(x => x.Id == message.TaskGroupId, ct);

        if (!checkTaskGroup) throw new Exception("Task group not exists");

        var task = await _context.Tasks.FirstAsync(x => x.Id == message.Id, ct);

        task.Name = message.Name;
        task.Description = message.Description;
        task.Status = message.Status;
        task.StartDate = message.StartDate;
        task.EndDate = message.EndDate;
        task.TaskGroupId = message.TaskGroupId;

        await _context.SaveChangesAsync(ct);

        return task;
    }

    public async Task<Domain.Entities.Task?> ChangeTaskStatusAsync(UpdateTaskStatusDto message, CancellationToken ct)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == message.Id, ct);

        if (task == null) return null;

        task.Status = message.Status;

        await _context.SaveChangesAsync(ct);

        return task;
    }

    public async Task<IEnumerable<Domain.Entities.Task>> FindGrouppedTasksAsync(int groupId, CancellationToken ct)
    {
        return await _context.Tasks.AsNoTracking()
                .Where(x => x.TaskGroupId == groupId)
                .ToListAsync(ct);
    }

    public async Task<Domain.Entities.Task?> ChangeTaskGroupAsync(UpdateTaskGroupDto message, CancellationToken ct)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == message.Id, ct);

        if (task == null) return null;

        task.TaskGroupId = message.TaskGroupId;

        await _context.SaveChangesAsync(ct);

        return task;
    }
}
