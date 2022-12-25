// To contain the pagination information
export interface Pagination {
    // These properity must be typing as camelCase and the same names as I send it from API 
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

// To contain the members and the pagination information
// This class equal to PageList.cs in API
export class PaginatedResult<T> {
    // here this will be a list of members -> T = members[] ( but I make it as generic to be more reusable )
    result?: T;
    pagination?: Pagination;
}