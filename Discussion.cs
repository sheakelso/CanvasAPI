namespace CanvasAPI;

public class Discussion(string id) : Node(id)
{
    [DefaultNodeProperty] public string _id { get; set; }
    [DefaultNodeProperty] public string title { get; set; }
    [DefaultNodeProperty] public DateTimeOffset? postedAt{ get; set; }
    
    public Course? Course { get; set; }
    public string Link => $"{Course?.Link}/discussion_topics/{_id}";
    
    public async Task<User?> GetAuthor() => await GetField<Discussion, User>("author");
    public async Task<string?> GetMessage() => await GetField<Discussion, string>("message");

    public async Task<bool> SetReadState(bool read)
    {
        return await RunMutation("updateDiscussionReadState",
            new Dictionary<string, object> { { "discussionTopicId", _id }, { "read", read } });
    }
}