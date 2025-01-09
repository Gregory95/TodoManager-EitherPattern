using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Enums;

namespace GKTodoManager.Domain.Dtos;

public class TaskGroupResponseDto : BaseDto
{    public string Name { get; set; }
    public string Description { get; set; }
    public ColorEnum Color { get; set; }
}
