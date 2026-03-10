namespace CanvasAPI;

public class Discussion(string id) : Node("Discussion", id)
{
    public async Task<string?> GetTitle() => await GetField<string>("title");
    public async Task<User?> GetAuthor() => await GetField<User>("author");
    public async Task<DateTimeOffset> GetPostedAt() => await GetField<DateTimeOffset>("postedAt");
    public async Task<string?> GetMessage() => await GetField<string>("message");
}