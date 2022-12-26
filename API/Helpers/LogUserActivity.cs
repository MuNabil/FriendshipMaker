namespace API.Helpers;

// The ActionFilter allow us to do something even before the request is executing or after the request is executed.
public class LogUserActivity : IAsyncActionFilter
{
    // (context) give us the abelity to access the action that executing before
    // (next) give us the abelity to access the action that executed 'after'
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // To get the context after the action has been executed
        var resultContext = await next();

        // To check if the user that make the request is not authenticated (mean is he/she send a invalid token)
        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        // Get the user id so that I can update the lastActive proprity in it
        var userId = resultContext.HttpContext.User.GetUserId();

        // To get access to the repository (with service located pattern) without inject it in ctor
        var repository = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();

        // Get the user to change the lastActive property in it
        var user = await repository.GetUserByIdAsync(userId);
        user.LastActive = DateTime.Now;
        await repository.SaveAllAsync();
    }
}