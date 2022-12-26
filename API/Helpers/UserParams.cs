namespace API.Helpers;

// Class that contains the pagination parameters that we will receive  from the user.
public class UserParams
{
    #region Pagination

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


    #endregion


    #region filtering

    // Get current username to don't displa it in the list of users
    public string CurrrentUserName { get; set; }

    //Get the gender to filter by it
    public string Gender { get; set; }

    // Get the Age to filter like From MinAge To MaxAge
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 150;

    #endregion
}