using System.ComponentModel.DataAnnotations;

namespace LAB1_WEB_API.Contracts.Users;

public record LoginUserRequest(
    [Required] string Name,
    [Required] string Password);