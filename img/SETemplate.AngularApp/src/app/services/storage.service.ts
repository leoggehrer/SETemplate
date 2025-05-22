//@CodeCopy
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StorageService {
  constructor() { }

  setData(key: string, data: any) {
    const jsonData = JSON.stringify(data);
    localStorage.setItem(key, jsonData);
  }

  getData(key: string) {
    const item = localStorage.getItem(key);

    if (item) {
      return JSON.parse(item);
    } else {
      return null;
    }
  }

  remove(key: string) {
    localStorage.removeItem(key);
  }
}
