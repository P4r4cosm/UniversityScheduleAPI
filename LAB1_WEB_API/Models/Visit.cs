using System.ComponentModel.DataAnnotations.Schema;

namespace LAB1_WEB_API;

public class Visit
{
    public int Id { get; set; }
    
    public int StudentId { get; set; }
    public Student Student { get; set; }
    
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    
    public DateTime visitTime { get; set; }
}