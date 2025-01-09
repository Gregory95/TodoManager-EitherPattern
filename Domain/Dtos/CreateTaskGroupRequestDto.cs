using GKTodoManager.Domain.Enums;

namespace GKTodoManager.Domain.Dtos;

public class CreateTaskGroupRequestDto
{
    public required string Name { get; set; }

    public string Description { get; set; }

    public ColorEnum Color { get; set; }
}
