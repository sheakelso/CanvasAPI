using System.Reflection;

namespace CanvasAPI;

public abstract class Node(string typeName, string id)
{
    public readonly string TypeName = typeName;
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

    public async Task<T?> GetField<T>(string fieldName)
    {
        if (Client == null) return default;
        if(typeof(T).IsAssignableTo(typeof(Node))) 
            return await Client.GetChildNode<T>(TypeName, Id, fieldName);
        return await Client.GetNodeField<T>(TypeName, Id, fieldName);
    }
}