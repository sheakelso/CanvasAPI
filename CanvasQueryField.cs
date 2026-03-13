using System.Reflection;
using Newtonsoft.Json;

namespace CanvasAPI;

public class CanvasQueryField
{
    public enum CanvasOperation
    {
        Query,
        Mutation
    }
    
    public CanvasQueryField? Parent { get; private set; }
    public readonly string Name;
    public readonly CanvasOperation Operation;

    private List<CanvasQueryField> _fields = new List<CanvasQueryField>();
    public CanvasQueryField[] Fields => _fields.ToArray();
    
    public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();
    
    public string? OnType { get; set; }
    
    public CanvasQueryField(string name, CanvasOperation operation = CanvasOperation.Query)
    {
        Name = name;
        Operation = operation;
    }
    
    public CanvasQueryField(string name, IEnumerable<CanvasQueryField> fields, CanvasOperation operation = CanvasOperation.Query)
    {
        Name = name;
        AddFields(fields);
    }

    public static CanvasQueryField FromNode<T>(string name, CanvasOperation operation = CanvasOperation.Query)
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
        return new CanvasQueryField(name, fields, operation);
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

    public static string DictToString(Dictionary<string, object> dict)
    {
        string result = "";
        foreach (string paramater in dict.Keys)
        {
            object value = dict[paramater];
            string? strValue = JsonConvert.SerializeObject(value);
            if(value is Dictionary<string, object> subDict) strValue = "{" + DictToString(subDict) + "}";
            result += $"{paramater}: {strValue},";
        }
        return result;
    }

    public string ToString()
    {
        string query = Name;
        if (Parameters.Count > 0)
        {
            query += "(" + DictToString(Parameters) + ")";
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

        if (Parent == null) query = Enum.GetName(Operation)?.ToLower() + " { " + query + " }";
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