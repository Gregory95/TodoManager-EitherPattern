using GKTodoManager.Domain.Enums;

namespace GKTodoManager.Domain.Dtos;

public class UpdateTaskGroupRequestDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ColorEnum Color { get; set; }
}
