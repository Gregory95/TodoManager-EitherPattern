using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Enums;


namespace GKTodoManager.Domain.Entities;

public class TaskGroup : BaseModel
{
    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public ColorEnum Color { get; set; } = ColorEnum.Default;
}
