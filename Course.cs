namespace CanvasAPI;

public class Course : CanvasObject
{
    public string _ID { get; set; }
    public string ID { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; }

    public string Link => $"{Client.BaseAddress}/courses/{_ID}";
}