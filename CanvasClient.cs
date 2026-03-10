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

    public async Task<JToken?> RunQuery(string query)
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
        return Deserialize<Course[]>(json?["allCourses"]);
    }

    public async Task<Course?> GetCourse(string courseId)
    {
        JToken? json = await RunQuery(CanvasQueries.Course(courseId));
        return Deserialize<Course>(json?["course"]);
    }
    
    public async Task<T?> GetNodeField<T>(string typeName, string id, string fieldName)
    {
        JToken? json = await RunQuery(CanvasQueries.Node(typeName, id, fieldName));
        JToken? field = json?["node"]?[fieldName];
        return Deserialize<T>(field);
    }

    public async Task<T?> GetChildNode<T>(string typeName, string id, string fieldName)
    {
        JToken? json = await RunQuery(CanvasQueries.ChildNode(typeName, id, fieldName));
        JToken? field = json?["node"]?[fieldName];
        return Deserialize<T>(field);
    }

    public T? Deserialize<T>(JToken? json)
    {
        if(json == null) return default;
        T? obj = json.ToObject<T>();
        if(obj is Node canvasObject) InitialiseObject(canvasObject);
        if(obj is IEnumerable<Node> canvasObjects) InitialiseArray(canvasObjects);
        return obj;
    }
    
    private T? InitialiseObject<T>(T? obj) where T : Node
    {
        if(obj == null) return null;
        obj.Client = this;
        return obj;
    }
    
    private T? InitialiseArray<T>(T? objs) where T : IEnumerable<Node>
    {
        if (objs == null) return default;
        foreach (Node obj in objs)
        {
            InitialiseObject(obj);
        }
        return objs;
    }
}