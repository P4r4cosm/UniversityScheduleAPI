using System.ComponentModel.DataAnnotations;

namespace LAB1_WEB_API;

public class Department
{
    public int Id {get; set;}
    [Required] public string Name {get; set;}
    
    
    public int InstituteId {get; set;}
    public Institute Institute {get; set;}
}