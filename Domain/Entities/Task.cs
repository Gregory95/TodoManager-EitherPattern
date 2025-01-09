using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;


namespace GKTodoManager.Domain.Entities;

public class Task : BaseModel
{
    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public TaskStatusEnum Status { get; set; }

    [ForeignKey(nameof(TaskGroupId))]
    public int? TaskGroupId { get; set; }

    public TaskGroup TaskGroupNavigation { get; set; }
}
