namespace CanvasAPI;

public static class CanvasQueries
{
    public static string AllCourses => @"
query {
    allCourses {
        name
        id
        _id
        imageUrl
    }
}
";
    
    public static string Course(string id) => $@"
query {{
    course(id: ""{id}"") {{
        name
        id
        _id
        imageUrl
    }}
}}
";
    
    public static string AllDiscussions(string courseId) => $@"
query {{
    course(id: ""{courseId}"") {{
        _id
        discussionsConnection{{
            nodes {{
                title
                createdAt
                id
                _id
                message
                author
            }}
        }}
    }}
}}
";
    
    public static string AllAnnouncements(string courseId) => $@"
query {{
    course(id: ""{courseId}"") {{
        _id
        discussionsConnection(filter: {{isAnnouncement: true}}) {{
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