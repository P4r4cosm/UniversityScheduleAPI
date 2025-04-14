using System.ComponentModel.DataAnnotations;

namespace LAB1_WEB_API;

public class Course
{
    public int Id { get; set; }
    
    [Required]public string Name { get; set; }
    
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
    
    
    public int SpecialityId { get; set; }
    public Speciality Speciality { get; set; }

    [Required]public string Term{get; set;}
}