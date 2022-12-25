namespace API.Helpers;

// Class that contains the pagination parameters that we will receive  from the user.
public class UserParams
{
    // max amount of items that we're ever going to return from a request
    private const int MaxPageSize = 50;

    // page size that we're going to recieve from client
    private int _pageSize = 10;

    // Full property to set the page size so it doesn't be greater than MaxPageSize
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public int PageNumber { get; set; } = 1;
}