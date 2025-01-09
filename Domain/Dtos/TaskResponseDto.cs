using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Enums;

namespace GKTodoManager.Domain.Dtos;
public class TaskResponseDto : BaseDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TaskStatusEnum Status { get; set; }
    public string StatusDesc 
    {
        get => Status.ToString();
        set => value = Status.ToString();
    }
    public int? TaskGroupId { get; set; }
}

