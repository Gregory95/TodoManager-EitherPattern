using GKTodoManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GKTodoManager.Domain.Dtos;

public class UpdateTaskStatusDto
{
    public required int Id { get; set; }
    [Range(1, 6)]
    public required TaskStatusEnum Status { get; set; }
}
