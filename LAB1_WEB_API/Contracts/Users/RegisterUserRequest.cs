using System.ComponentModel.DataAnnotations;

namespace LAB1_WEB_API.Contracts.Users;

public record RegisterUserRequest(
    [Required] string Name,
    [Required] string Password);