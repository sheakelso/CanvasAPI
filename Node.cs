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
}