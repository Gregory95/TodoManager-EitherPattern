using GKTodoManager.Domain.Enums;

namespace GKTodoManager.Domain.Dtos;

public class CreateTaskRequestDto
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public required DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TaskStatusEnum Status { get; set; } = TaskStatusEnum.New;
    public int? TaskGroupId { get; set; }
}
