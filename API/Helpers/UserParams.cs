namespace API.Helpers;

// Class that contains the parameters that we will receive  from the user.
public class UserParams : PaginationParams
{
    #region filtering

    // Get current username to don't displa it in the list of users
    public string CurrrentUserName { get; set; }

    //Get the gender to filter by it
    public string Gender { get; set; }

    // Get the Age to filter like From MinAge To MaxAge
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 150;

    #endregion

    // For Sorting
    public string OrderBy { get; set; } = "lastActive";
}