using System.Reflection;

namespace CanvasAPI;

public class DefaultNodePropertyAttribute : Attribute { }

public class NodeTypeNameAttribute(string name) : Attribute
{
    public readonly string TypeName = name;
}

public abstract class Node(string id)
{
    public static string GetTypeName<T>() => typeof(T).Name;
    public string Id { get; private set; } = id;
    
    private CanvasClient? _client;
    public CanvasClient? Client
    {
        get => _client;
        set
        {
            _client = value;
            InitialiseChildren();
        }
    }

    private void InitialiseChildren()
    {
        foreach (PropertyInfo property in GetType().GetProperties())
        {
            if (property.PropertyType.IsAssignableTo(typeof(Node)))
            {
                Node? child = (Node?)property.GetValue(this);
                if (child != null)
                {
                    child.Client = _client;
                }
            }
        }
    }

    public async Task<TC?> GetField<TP, TC>(string fieldName) where TP : Node
    {
        if (Client == null) return default;
        return await Client.GetNodeField<TP, TC>(Id, fieldName);
    }

    public async Task<bool> RunMutation(string name, Dictionary<string, object> input)
    {
        if (Client == null) return false;
        CanvasQueryField query = new CanvasQueryField(name, CanvasQueryField.CanvasOperation.Mutation);
        query.WithParameter("input", input);
        query.WithField(new CanvasQueryField("errors").WithField("message"));
        return await Client.RunMutation(query);
    }
}