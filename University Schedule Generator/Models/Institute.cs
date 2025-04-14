using System.ComponentModel.DataAnnotations;

namespace LAB1_WEB_API;

public class Institute
{
    public int Id {get; set;}
    [Required]public string Name {get; set;}
    
    public int UniversityId {get; set;}
    public University University {get; set;}
}