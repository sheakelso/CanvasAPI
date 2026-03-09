using System.Reflection;

namespace CanvasAPI;

public abstract class CanvasObject
{
    private CanvasClient _client;
    public CanvasClient Client
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
            if (property.PropertyType.IsAssignableTo(typeof(CanvasObject)))
            {
                CanvasObject? child = (CanvasObject?)property.GetValue(this);
                if (child != null)
                {
                    child.Client = _client;
                }
            }
        }
    }
}