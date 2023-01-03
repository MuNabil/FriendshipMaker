namespace API.SignalR;

[Authorize]
public class PresenceHub : Hub
{
    private readonly PresenceTracker _tracker;
    public PresenceHub(PresenceTracker tracker)
    {
        _tracker = tracker;
    }
    public override async Task OnConnectedAsync()
    {
        bool isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

        // If this is the first connection to this user then isOnline will be true 
        // then we will send that user is online to all listening clients
        if (isOnline)
        {
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
        }

        var currentUsers = await _tracker.GetOnlineUsers();
        // To send all online users to the caller that listen to the event "GetOnlineUsers" now
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

        if (isOffline) // this will be true if this user has no connection
        {
            // To send that this caller is become offline now to other clients that listen to this event
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
        }

        await base.OnDisconnectedAsync(exception);
    }

}