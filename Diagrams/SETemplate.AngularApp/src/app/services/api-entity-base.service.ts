//@CodeCopy
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { IdType, IKeyModel } from '@app/models/i-key-model';
import { IQueryParams } from '@app/models/base/i-query-params';
import { IApiEntityBaseService } from './i-api-entity-base.service';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { arrayToDate, stringToDate } from '@app/converter/date-converter';

/**
 * Abstract base service for API operations.
 * Provides common CRUD operations for entities implementing the IKey interface.
 * @template T - The type of the entity extending IKey.
 */
export abstract class ApiEntityBaseService<T extends IKeyModel> implements IApiEntityBaseService<T> {
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
   * Retrieves a single entity by its ID.
   * @param id - The ID of the entity to retrieve.
   * @returns An Observable of the entity.
   */
  public getById(id: IdType): Observable<T> {
    return this.http.get<T>(`${this.ENDPOINT_URL}/${id}`)
      .pipe(
        map((response: T) => {
          stringToDate(response);
          return response;
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Error retrieving entity by ID:', error);
          throw error;
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

  /**
   * Creates a new entity in the API.
   * @param dataItem - The entity to create.
   * @returns An Observable of the created entity.
   */
  public create<T extends IKeyModel>(dataItem: T): Observable<T> {
    return this.http
      .post<T>(`${this.ENDPOINT_URL}`, dataItem)
      .pipe(
        map((response: T) => {
          stringToDate(response);
          return response;
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Error creating entity:', error);
          throw error;
        })
      );
  }

  /**
   * Updates an existing entity in the API.
   * @param dataItem - The entity to update.
   * @returns An Observable of the updated entity.
   */
  public update<T extends IKeyModel>(dataItem: T): Observable<T> {
    return this.http
      .put<T>(`${this.ENDPOINT_URL}/${dataItem.id}`, dataItem)
      .pipe(
        map((response: T) => {
          stringToDate(response);
          return response;
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Error updating entity:', error);
          throw error;
        })
      );
  }

  /**
   * Deletes an entity from the API.
   * @param dataItem - The entity to delete.
   * @returns An Observable of the HTTP response.
   */
  public delete(dataItem: IKeyModel) {
    const options = {
      headers: {},
      body: dataItem,
    };

    return this.http
      .delete(`${this.ENDPOINT_URL}/${dataItem.id}`, options)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          console.error('Error deleting entity:', error);
          throw error;
        })
      );
  }

  /**
   * Deletes an entity by its ID from the API.
   * @param id - The ID of the entity to delete.
   * @returns An Observable of the HTTP response.
   */
  public deleteById(id: IdType) {
    const options = {
      headers: {}
    };

    return this.http
      .delete(`${this.ENDPOINT_URL}/${id}`, options)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          console.error('Error deleting entity by ID:', error);
          throw error;
        })
      );
  }
}
