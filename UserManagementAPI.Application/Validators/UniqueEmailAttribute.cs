using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using UserManagementAPI.Application.Interfaces;

namespace UserManagementAPI.Application.Validators
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string email)
                return ValidationResult.Success;

            var userRepository = validationContext.GetService<IUserRepository>();
            if (userRepository == null)
                return ValidationResult.Success;

            var existingUser = userRepository.GetByEmailAsync(email).Result;
            if (existingUser != null)
                return new ValidationResult("Email address is already in use.");

            return ValidationResult.Success;
        }
    }
}
