import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, throwError } from "rxjs";
import { ToastService } from "../../services/toast/toast.service";

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const $toast = inject(ToastService);

  return next(req).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse) {
        let body: object | string = err.error;
        if (typeof body === "string") {
          try {
            body = JSON.parse(err.error);
          } catch { }
        }

        $toast.show({
          header: getErrorHeader(err),
          body: isODataErrorResponse(body)
            ? getODataErrorBody(body)
            : getFallbackErrorBody(err),
          theme: "danger",
        });
      }

      return throwError(() => err);
    }),
  );
};

function isODataErrorResponse(value: unknown): value is ODataErrorResponse {
  if (!value || typeof value !== "object") {
    return false;
  }

  const error = (value as Record<string, unknown>)["error"];

  if (!error || typeof error !== "object") {
    return false;
  }

  return typeof (error as Record<string, unknown>)["message"] === "string";
}

function getODataErrorBody(response: ODataErrorResponse): string {
  const { error } = response;

  if (!error.details?.length) {
    return error.message;
  }

  const details = error.details
    .map(x => x.target ? `${x.target}: ${x.message}` : x.message)
    .join("\n");

  return `${error.message}\n\n${details}`;
}

function getFallbackErrorBody(err: HttpErrorResponse): string {
  if (typeof err.error === "string" && err.error.trim()) {
    return err.error;
  }

  return err.message || "The request could not be completed.";
}

function getErrorHeader(err: HttpErrorResponse): string {
  switch (err.status) {
    case 0:
      return "Connection Error";
    case 400:
      return "Invalid Request";
    case 401:
      return "Unauthorized";
    case 403:
      return "Forbidden";
    case 404:
      return "Not Found";
    case 409:
      return "Conflict";
    case 422:
      return "Validation Error";
    case 500:
      return "Server Error";
    default:
      return "Request Failed";
  }
}
interface ODataErrorResponse {
  error: ODataError;
}

interface ODataError {
  code: string;
  target?: string;
  message: string;
  details?: ODataErrorDetail[];
}

interface ODataErrorDetail {
  code: string;
  target?: string;
  message: string;
}
