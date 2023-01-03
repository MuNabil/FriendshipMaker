namespace API.SignalR;

// Class to track the user that currently connected
public class PresenceTracker
{
    // Dictionary to store key: username, value: connectionId 
    // Maybe there is a user that will connect from diffrient devices so this user will have more than one connectionId
    public static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

    // Method to add a user connection to the dictionary and return true only if the user is not in the dictionary
    public Task<bool> UserConnected(string username, string connectionId)
    {
        bool isOnline = false;

        // Lock the dictionary because maybe many clients try to use it at same time and the dictionary is not a thread safe resource
        lock (OnlineUsers)
        {
            // If the user is already connected then add this new connectionId to his list
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, new List<string> { connectionId });
                isOnline = true;
            }
        }
        return Task.FromResult(isOnline);
    }

    // Method to remove a user connection from the dictionary
    public Task<bool> UserDisconnected(string username, string connectionId)
    {
        bool isOffline = false;

        // lock it at every point so it can only do one thing at a time
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

            OnlineUsers[username].Remove(connectionId);

            // If there is no more connection to this user then remove it from the dictionary and now he is actully offline
            if (OnlineUsers[username].Count == 0)
            {
                OnlineUsers.Remove(username);
                isOffline = true;
            }
        }
        return Task.FromResult(isOffline);
    }

    // Method to return all users username that is currently connected
    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.OrderBy(d => d.Key).Select(d => d.Key).ToArray();
        }

        return Task.FromResult(onlineUsers);
    }

    // For notifying a users when they recieved a new messages
    public Task<List<string>> GetConnectionForUser(string username)
    {
        List<string> connectionIds;
        lock (OnlineUsers)
        {
            connectionIds = OnlineUsers.GetValueOrDefault(username);
        }
        return Task.FromResult(connectionIds);

    }
}