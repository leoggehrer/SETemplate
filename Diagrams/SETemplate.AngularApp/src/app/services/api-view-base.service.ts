//@CodeCopy
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { IQueryParams } from '@app/models/base/i-query-params';
import { IViewModel } from '../models/i-view-model';
import { IApiViewBaseService } from './i-api-view-base.service';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { arrayToDate } from '@app/converter/date-converter';

/**
 * Abstract base service for API operations.
 * Provides common CRUD operations for entities implementing the IKey interface.
 * @template T - The type of the entity extending IKey.
 */
export abstract class ApiViewBaseService<T extends IViewModel> implements IApiViewBaseService<T> {
  /**
   * Constructor for ApiBaseService.
   * @param http - The HttpClient instance for making HTTP requests.
   * @param ENDPOINT_URL - The base URL of the API endpoint.
   */
  constructor(
    public http: HttpClient,
    protected ENDPOINT_URL: string) {
  }

  /**
   * Retrieves the count of entities from the API.
   * @returns An Observable of the count as a number.
   */
  public getCount(): Observable<number> {
    return this.http.get<number>(`${this.ENDPOINT_URL}/count`).pipe(
      map((response: number) => {
        return response;
      })
    );
  }

  /**
   * Retrieves all entities from the API.
   * @returns An Observable of an array of entities.
   */
  public getAll(): Observable<T[]> {
    return this.http.get<T[]>(`${this.ENDPOINT_URL}`).pipe(
      map((response: T[]) => {
        arrayToDate(response);
        return response;
      })
    );
  }

  /**
   * Executes a query against the API endpoint and retrieves an array of results.
   * @param queryParams - The parameters to be sent with the query request.
   * @returns An observable that emits an array of results of type `T`.
   */
  public query(queryParams: IQueryParams): Observable<T[]> {
    return this.http.post<T[]>(`${this.ENDPOINT_URL}/query`, queryParams)
      .pipe(
        map((response: T[]) => {
          arrayToDate(response);
          return response;
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Error executing query:', error);
          throw error;
        })
      );
  }
}
