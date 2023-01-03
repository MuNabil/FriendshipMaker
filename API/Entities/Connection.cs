namespace API.Entities;

public class Connection
{
    public Connection() // Empty ctor because required by EF (when I create another nonempty one)
    {
    }

    public Connection(string connectionId, string username) // To easly scan its properities
    {
        ConnectionId = connectionId;
        Username = username;
    }

    public string ConnectionId { get; set; }
    public string Username { get; set; }
}