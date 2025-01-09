using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GKTodoManager.Domain.Base;

public class BaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    public string? ModifiedBy { get; set; }
    public string? CreatedBy { get; set; }
}
