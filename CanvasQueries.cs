namespace CanvasAPI;

public static class CanvasQueries
{
    public static string AllCourses => @"
query {
    allCourses {
        id
        _id
        name
    }
}
";
    
    public static string Course(string id) => $@"
query {{
    course(id: ""{id}"") {{
        id
        _id
        name
    }}
}}
";
    
    public static string AllDiscussions(string courseId) => $@"
query {{
    course(id: ""{courseId}"") {{
        _id
        discussionsConnection{{
            edges {{
                cursor
                node {{
                    
                }}
            }}
        }}
    }}
}}
";
    
    public static string CourseAnnouncements(string courseId, string? after = null, string? before = null, int? first = null,
        int? last = null)
    {
        string parameters = (after != null ? $"after: \"{after}\", " : "") + (before != null ? $"before: \"{before}\", " : "") + (first != null ? $"first: {first}, " : "");
        return $@"
query {{
    course(id: {courseId}) {{
        id
        _id
        discussionsConnection({parameters}, filter: {{isAnnouncement: true}}) {{
            edges {{
                cursor
                node {{
                    id
                }}
            }}
        }}
    }}
}}
";
    }

    public static string AllCoursesAnnouncements(string? after = null, string? before = null, int? first = null,
        int? last = null)
    {
        string parameters = (after != null ? $"after: \"{after}\", " : "") + 
                            (before != null ? $"before: \"{before}\", " : "") + 
                            (first != null ? $"first: {first}, " : "") + 
                            (last != null ? $"last: {last}, " : "");
        return $@"
query {{
    allCourses {{
        id
        _id
        discussionsConnection({parameters}, filter: {{isAnnouncement: true}}) {{
            nodes {{
                title
                createdAt
                id
                _id
                message
                author {{
                    firstName
                    lastName
                    name
                    email
                }}
            }}
        }}
    }}
}}
";
    }

    public static string Node(string typeName, string id, string fieldName) => $@"
query {{
  node(id: ""{id}"") {{
    ... on {typeName} {{
      {fieldName}
    }}
  }}
}}
";
    
    public static string ChildNode(string typeName, string id, string fieldName) => $@"
query {{
  node(id: ""{id}"") {{
    ... on {typeName} {{
        {fieldName} {{
            id
        }}
    }}
  }}
}}
";
}