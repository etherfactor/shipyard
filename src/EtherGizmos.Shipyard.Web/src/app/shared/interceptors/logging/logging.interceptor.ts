import { HttpEventType, HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { of, switchMap, tap } from 'rxjs';
import { Logger } from '../../utilities/logger/logger.util';

export const loggingInterceptor: HttpInterceptorFn = (req, next) => {
  const logger = inject(Logger).forContext("HttpClient");

  let start: number;
  return of(undefined).pipe(
    tap(() => {
      start = performance.now();
      logger.information(
        "Starting HTTP {Method} request to {Url}. Headers: {HeaderCount}, Body: {HasBody}, BodyLength: {BodyLength}",
        req.method,
        req.url,
        req.headers.keys().length,
        !!req.body,
        (typeof req.body === "object" ? JSON.stringify(req.body) : String(req.body)).length,
      );
    }),
    switchMap(() => next(req)),
    tap(event => {
      if (event.type === HttpEventType.Response && event instanceof HttpResponse) {
        const end = performance.now();
        logger.information(
          "Finished HTTP {Method} request to {Url} with status {StatusCode} in {ElapsedMilliseconds}ms. ResponseBodyLength: {BodyLength}",
          req.method,
          req.url,
          event.status,
          end - start,
          event.body ? JSON.stringify(event.body).length : 0,
        );
      }
    }),
  );
};
