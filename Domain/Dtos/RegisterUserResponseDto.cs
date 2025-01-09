using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GKTodoManager.Domain.Dtos;

public class RegisterUserResponseDto
{
    public string Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; } = null;
    public string Username { get; set; }
    public string NormalizedUserName { get; set; }
    public string Email { get; set; }
    public string NormalizedEmail { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [NotMapped]
    public string FullName
    {
        get
        {
            return FirstName + " " + LastName;
        }
    }
}

public class UserEmailVM
{
    [Required]
    public string Email { get; set; }
}

public class OtpVerificationVM
{
    [Required]
    [MaxLength(6)]
    public string Otp { get; set; }
    [Required]
    public string Email { get; set; }
}