using System.ComponentModel.DataAnnotations;
using UserManagementAPI.Domain.Enums;

namespace UserManagementAPI.Application.DTOs
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public UserRole Role { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public UserStatus Status { get; set; }

        public string? Department { get; set; }
    }
}
