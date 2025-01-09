using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace GKTodoManager.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? PasswordModifiedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool HasFirstLogin { get; set; } = false;

        [ForeignKey("UserId")]
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName, string description) : base(roleName)
        {
            this.Description = description;
        }

        public Boolean IsDeleted { get; set; }
        public string Description { get; set; }
        public bool? IsAdmin { get; set; }
        public DateTime Created { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    }

    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }

    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public virtual ApplicationRole Role { get; set; }
    }

    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}