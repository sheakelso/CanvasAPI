namespace CanvasAPI;

public class User(string id) : Node(id)
{
    [DefaultNodeProperty] public string name { get; set; }
    [DefaultNodeProperty] public string avatarUrl { get; set; }
}