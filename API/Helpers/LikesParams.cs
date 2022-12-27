namespace API.Helpers;

// Class that will contain the likes and pagination 'that contain from inheritance' parameters that will be sent to the API in the query string from client.
public class LikesParams : PaginationParams
{
    public int UserId { get; set; }
    public string Predicate { get; set; }
}