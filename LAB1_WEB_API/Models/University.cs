using System.ComponentModel.DataAnnotations;

namespace LAB1_WEB_API;

public class University
{
    public int Id {get; set;}
    [Required] public string Name {get; set;}
}