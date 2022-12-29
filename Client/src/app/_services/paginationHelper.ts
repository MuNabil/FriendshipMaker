import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../_models/pagination";

export function GetPaginatedResult<T>(url: string, params: HttpParams, http: HttpClient) {
    // To contain the response body and the response header { result(bode): (membr[]) , pagination(header): pagination information}
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

    // Using this observe syntax to get the all 'response' not just the body
    return http.get<T>(url, { observe: 'response', params }).pipe(
        map(response => {

            // Take the response body that containing the actual data
            paginatedResult.result = response.body;

            // Take the pagination informations
            if (response.headers.get('Pagination') !== null) {
                paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
            }

            return paginatedResult;
        })
    );
}

export function GetPaginationHeaders(pageNumber: number, pageSize: number) {
    //To add the parameters in the query string
    let params = new HttpParams();

    // To add the parameters that I want to with params query string to the endpoint
    params = params.append('pageNumber', pageNumber.toString()); // toString() because I wanna send it as a string in the query string

    params = params.append('pageSize', pageSize.toString());

    return params;
}