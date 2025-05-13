//@CodeCopy
import moment from 'moment';
import { map } from 'rxjs/operators';

export function JSONDateConverter(target: any, key: any, descriptor: any) {
  const originalMethod = descriptor.value;
  descriptor.value = function () {
    return originalMethod
      .apply(this)
      .pipe(map((obj) => Object.assign({}, obj, stringToDate(obj))));
  };
  return descriptor;
}

const isDate = (s: any) => moment(s, moment.ISO_8601, true).isValid();

export function stringToDate(obj: any) {
  // performance!!
  Object.keys(obj)
    .filter(
      (key) =>
        obj[key] &&
        typeof obj[key] === 'string' &&
        obj[key].length >= 19 &&
        obj[key][10] === 'T' &&
        isDate(obj[key])
    )
    .map((key) => {
      obj[key] = moment(obj[key]).toDate();
    });

  Object.keys(obj)
    .filter((key) => obj[key] && typeof obj[key] === 'object')
    .map((key) => {
      stringToDate(obj[key]);
    });
}

export function arrayToDate(arr: any[]) {
  arr.forEach((elem) => {
    stringToDate(elem);
  });
}
