using System.Reflection;

namespace CanvasAPI;

public class CanvasQueryField
{
    public CanvasQueryField? Parent { get; private set; }
    public readonly string Name;

    private List<CanvasQueryField> _fields = new List<CanvasQueryField>();
    public CanvasQueryField[] Fields => _fields.ToArray();
    
    public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
    
    public string? OnType { get; set; }

    public static CanvasQueryField FromNode<T>(string name)
    {
        List<CanvasQueryField> fields = new List<CanvasQueryField>();
        foreach (PropertyInfo property in typeof(T).GetProperties())
        {
            foreach (CustomAttributeData attributeData in property.CustomAttributes)
            {
                if (attributeData.AttributeType == typeof(DefaultNodePropertyAttribute))
                {
                    fields.Add(new CanvasQueryField(property.Name));
                }
            }
        }
        fields.Add(new CanvasQueryField("id"));
        return new CanvasQueryField(name, fields);
    }
    
    public CanvasQueryField(string name)
    {
        Name = name;
    }
    
    public CanvasQueryField(string name, IEnumerable<CanvasQueryField> fields)
    {
        Name = name;
        AddFields(fields);
    }

    public void AddField(CanvasQueryField field)
    {
        field.Parent = this;
        _fields.Add(field);
    }

    public void AddFields(IEnumerable<CanvasQueryField> fields)
    {
        foreach (CanvasQueryField field in fields) AddField(field);
    }

    public string ToString()
    {
        string query = Name;
        if (Parameters.Count > 0)
        {
            query += "(";
            foreach (string paramater in Parameters.Keys)
            {
                query += $"{paramater}: \"{Parameters[paramater]}\",";
            }
            query += ")";
        }
        
        if (_fields.Count > 0)
        {
            query += " {";

            if (OnType != null) query += " ... on " + OnType + " {";
            foreach (CanvasQueryField field in _fields)
            {
                query += " " + field.ToString();
            }
            if (OnType != null) query += " }";
            
            query += " }";
        }

        if (Parent == null) query = "query { " + query + " }";
        return query;
    }

    public CanvasQueryField On<T>()
    {
        OnType = Node.GetTypeName<T>();
        return this;
    }

    public CanvasQueryField WithParameter(string name, object value)
    {
        Parameters.Add(name, value);
        return this;
    }

    public CanvasQueryField WithField(string name)
    {
        AddField(new CanvasQueryField(name));
        return this;
    }

    public CanvasQueryField WithField(CanvasQueryField field)
    {
        AddField(field);
        return this;
    }
}