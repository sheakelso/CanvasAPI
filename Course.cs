using Newtonsoft.Json.Linq;

namespace CanvasAPI;

public class Course(string id) : Node(id)
{
    [DefaultNodeProperty] public string _id { get; set; }
    [DefaultNodeProperty] public string name { get; set; }
    [DefaultNodeProperty] public string courseCode { get; set; }
    [DefaultNodeProperty] public string imageUrl { get; set; }

    public string Link => $"{Client?.BaseAddress}/courses/{_id}";
    
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
    ... on {Node.GetTypeName<Course>()} {{
        discussionsConnection{parameters} {{
            edges {{
                cursor
                node {{
                    _id
                    id
                    title
                    postedAt
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
            discussion.Course = this;
            result.Add(cursor, discussion);
        }
        return result;
    }
}