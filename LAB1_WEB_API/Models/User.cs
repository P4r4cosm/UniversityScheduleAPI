using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LAB1_WEB_API;

public class User
{
    public int Id { get; set; }

    [Required]public string Name { get; set; } = "";
    
    [Required]public string PasswordHash { get; set; } = "";
}