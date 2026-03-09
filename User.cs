namespace CanvasAPI;

public class User : CanvasObject
{
    public string AvatarUrl { get; set; }
    public string? Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Name { get; set; }
}