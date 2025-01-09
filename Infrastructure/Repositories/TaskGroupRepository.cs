using GKTodoManager.Domain.Abstractions;
using GKTodoManager.Domain.Dtos;
using GKTodoManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace GKTodoManager.Infrastructure.Repositories;

public class TaskGroupRepository(ApplicationDbContext context) : ITaskGroupRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<TaskGroup> CreateAsync(CreateTaskGroupRequestDto request, CancellationToken ct)
    {
        var newTaskGroup = await _context.AddAsync(new TaskGroup
        {
            Name = request.Name,
            Description = request.Description,
            Color = request.Color
        }, ct);

        await _context.SaveChangesAsync(ct);

        return newTaskGroup.Entity;
    }

    public async Task<int> DeleteAsync(int taskGroupId, CancellationToken ct)
    {
        var taskGroup = await _context.TaskGroups.FirstAsync(x => x.Id == taskGroupId, ct);

        _context.TaskGroups.Remove(taskGroup);
        await _context.SaveChangesAsync(ct);

        return taskGroupId;
    }

    public async Task<IEnumerable<TaskGroup>> FindAllAsync(CancellationToken ct)
    {
        return await _context.TaskGroups.AsNoTracking().ToListAsync(ct);
    }

    public async Task<TaskGroup?> FindAsync(int Id, CancellationToken ct)
    {
        return await _context.TaskGroups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id, ct);
    }

    public async Task<TaskGroup> UpdateAsync(UpdateTaskGroupRequestDto request, CancellationToken ct)
    {
        var taskGroup = await _context.TaskGroups.FirstAsync(x => x.Id == request.Id, ct);

        taskGroup.Name = request.Name;
        taskGroup.Description = request.Description;
        taskGroup.Color = request.Color;

        await _context.SaveChangesAsync(ct);

        return taskGroup;
    }
}
