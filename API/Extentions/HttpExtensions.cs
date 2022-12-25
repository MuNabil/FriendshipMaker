namespace API.Extentions;

public static class HttpExtensions
{
    // This method to add a pagination header to a http response.
    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
        var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

        // To make the response in camelCase
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        // To add the pagination to the response headers
        response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));

        // Add a CORS header into this header ( "Pagination" ) to make it available
        // In other words (to give explicit permissions for the client to read this header)
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
}