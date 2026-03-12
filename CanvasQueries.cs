namespace CanvasAPI;

public static class CanvasQueries
{
    public static CanvasQueryField AllCourses() => CanvasQueryField.FromNode<Course>("allCourses");
    public static CanvasQueryField Node<T>(string id) where T : Node => CanvasQueryField.FromNode<T>("node").WithParameter("id", id).On<T>();

    public static CanvasQueryField NodeField<TP, TC>(string id, string fieldName) where TP : Node
    {
        CanvasQueryField field;
        if (typeof(TC).IsAssignableTo(typeof(Node))) field = CanvasQueryField.FromNode<TC>(fieldName);
        else field = new CanvasQueryField(fieldName);
        
        return new CanvasQueryField("node").WithParameter("id", id).WithField(field).On<TP>();
    }
}