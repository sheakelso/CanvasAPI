namespace CanvasAPI;

public class User(string id) : Node("User", id)
{
    public async Task<string?> GetName() => await GetField<string>("name");
    public async Task<string?> GetAvatarUrl() => await GetField<string>("avatarUrl");
}