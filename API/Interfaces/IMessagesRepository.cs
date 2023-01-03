namespace API.Interfaces;

public interface IMessagesRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessageAsync(int id);
    Task<PagedList<MessageDto>> GetMessagesforUserAsync(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessagesThreadAsync(string currentUsername, string recipientUsername);
    Task<bool> SaveAllAsync();

    // For SignalR
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection> GetConnection(string connectionId);
    Task<Group> GetMessageGroup(string groupName);
    Task<Group> GetGroupForConnection(string connectionId);
}