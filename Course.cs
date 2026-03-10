using Newtonsoft.Json.Linq;

namespace CanvasAPI;

public class Course(string id) : Node("Course", id)
{
    public string _Id { get; set; }

    public async Task<string?> GetName() => await GetField<string?>("name");
    public async Task<string?> GetImageUrl() => await GetField<string?>("imageUrl");

    public async Task<Dictionary<string, Discussion>?> GetDiscussions(string? after = null, string? before = null,
        int? first = null, int? last = null)
    {
        if (Client == null) return null;
        string parameters = (after != null ? $"after: \"{after}\", " : "") + 
                            (before != null ? $"before: \"{before}\", " : "") + 
                            (first != null ? $"first: {first}, " : "") + 
                            (last != null ? $"last: {last}, " : "");
        if(parameters.Length > 0) parameters = "(" + parameters + ")";

        string query = $@"
query {{
  node(id: ""{Id}"") {{
    ... on {TypeName} {{
        discussionsConnection{parameters} {{
            edges {{
                cursor
                node {{
                    id
                }}
            }}
        }}
    }}
  }}
}}";

        JToken? json = await Client.RunQuery(query);
        JToken? edges = json?["node"]?["discussionsConnection"]?["edges"];
        if (edges == null) return null;

        Dictionary<string, Discussion> result = new Dictionary<string, Discussion>();
        foreach (JToken edge in edges.Children())
        {
            string? cursor = edge["cursor"]?.ToString();
            Discussion? discussion = Client.Deserialize<Discussion>(edge["node"]);
            if(discussion == null || cursor == null) continue;
            result.Add(cursor, discussion);
        }
        return result;
    }
}