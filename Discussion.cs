namespace CanvasAPI;

public class Discussion : CanvasObject
{
    public string _CourseID { get; set; }
    public string _ID { get; set; }
    public string ID { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public User Author { get; set; }
    public DateTime CreatedAt { get; set; }

    public string Link => $"{Client.BaseAddress}/courses/{_CourseID}/discussion_topics/{_ID}";
}