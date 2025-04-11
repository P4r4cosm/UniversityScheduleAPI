namespace LAB1_WEB_API;

public class Schedule
{
    public int Id {get; set;}
    
    public int LectureId {get; set;}
    public Lecture Lecture {get; set;}
    
    public int GroupId {get; set;}
    public Group Group {get; set;}
    
    public DateTime StartTime {get; set;}
    public DateTime EndTime {get; set;}
}