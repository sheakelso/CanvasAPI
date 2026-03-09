using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CanvasAPI;

public class CanvasClient
{
    public readonly string Endpoint;
    public readonly string BaseAddress;
    public readonly HttpClient HttpClient;

    public CanvasClient(string baseAddress, string apiKey)
    {
        BaseAddress = baseAddress;
        if(BaseAddress.EndsWith("/")) BaseAddress = BaseAddress.Substring(0, BaseAddress.Length - 1);
        Endpoint = $"{baseAddress}/api/graphql";
        
        HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    private async Task<JToken?> RunQuery(string query)
    {
        Dictionary<string, string> queryParams = new Dictionary<string, string>();
        queryParams.Add("query", query);

        StringContent content = new StringContent(JsonConvert.SerializeObject(queryParams));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        
        HttpResponseMessage response = await HttpClient.PostAsync(Endpoint, content);

        try
        {
            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            if (json.TryGetValue("errors", out var errors))
            {
                Console.WriteLine("Error in query: ");
                foreach (JToken error in errors)
                {
                    Console.WriteLine(error["message"]);
                }
            }

            return json["data"];
        }
        catch (JsonReaderException e)
        {
            Console.WriteLine("Response was not valid json: ");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
        
        return null;
    }

    public async Task<Course[]?> GetAllCourses()
    {
        JToken? json = await RunQuery(CanvasQueries.AllCourses);
        return Deserialise<Course[]>(json?["allCourses"]);
    }

    public async Task<Course?> GetCourse(string id)
    {
        JToken? json = await RunQuery(CanvasQueries.Course(id));
        return Deserialise<Course>(json?["course"]);
    }

    public async Task<Discussion[]?> GetAllDiscussions(string courseId)
    {
        JToken? json = await RunQuery(CanvasQueries.AllDiscussions(courseId));
        string? _courseID = json?["course"]?["_id"]?.ToString();
        JToken? nodes = json?["course"]?["discussionsConnection"]?["nodes"];
        if (nodes == null || _courseID == null) return null;

        foreach (JToken node in nodes.Children())
        {
            node["_courseID"] = JToken.Parse($"\"{_courseID}\"");
        }

        return Deserialise<Discussion[]>(nodes);
    }
    
    public async Task<Discussion[]?> GetAllAnnouncements(string courseId)
    {
        JToken? json = await RunQuery(CanvasQueries.AllAnnouncements(courseId));
        string? _courseID = json?["course"]?["_id"]?.ToString();
        JToken? nodes = json?["course"]?["discussionsConnection"]?["nodes"];
        if (nodes == null || _courseID == null) return null;

        foreach (JToken node in nodes.Children())
        {
            node["_courseID"] = JToken.Parse($"\"{_courseID}\"");
        }
        
        return Deserialise<Discussion[]>(nodes);
    }

    private T? Deserialise<T>(JToken? json)
    {
        if(json == null) return default;
        T? obj = json.ToObject<T>();
        if(obj is CanvasObject canvasObject) InitialiseObject(canvasObject);
        if(obj is IEnumerable<CanvasObject> canvasObjects) InitialiseArray(canvasObjects);
        return obj;
    }
    
    private T? InitialiseObject<T>(T? obj) where T : CanvasObject
    {
        if(obj == null) return null;
        obj.Client = this;
        return obj;
    }
    
    private T? InitialiseArray<T>(T? objs) where T : IEnumerable<CanvasObject>
    {
        if (objs == null) return default;
        foreach (CanvasObject obj in objs)
        {
            InitialiseObject(obj);
        }
        return objs;
    }
}