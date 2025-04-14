namespace LAB1_WEB_API;

public class MaterialElastic
{
    public int Id {get; set;}
    
    public string Name {get; set;}
    public string Content {get; set;}
    public int LectureId {get; set;}
    public Lecture Lecture {get; set;}
}