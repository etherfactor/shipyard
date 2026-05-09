import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { buildUrl } from '@ethergizmos/odata-fluent-client/dist/src/utils/http';
import { catchError, firstValueFrom, of } from 'rxjs';
import { APP_CONFIG } from '../../../../shared/utilities/config/config.util';
import { ImportResultZ } from '../../models/import-result';

@Injectable({
  providedIn: 'root',
})
export class ImportService {
  private readonly config = inject(APP_CONFIG);
  private readonly $http = inject(HttpClient);

  async import(content: string, contentType = "application/yaml") {
    const url = buildUrl(this.config.resourceServer, "api", "v1", `import`);
    const result = await firstValueFrom(this.$http.post(url, content, {
      headers: {
        "Content-Type": contentType,
      },
    }).pipe(
      catchError((err: HttpErrorResponse) => {
        return of(err.error);
      }),
    ));
    return ImportResultZ.parse(result);
  }

  async verify(content: string, contentType = "application/yaml") {
    const url = buildUrl(this.config.resourceServer, "api", "v1", `import`, `verify`);
    const result = await firstValueFrom(this.$http.post(url, content, {
      headers: {
        "Content-Type": contentType,
      },
    }).pipe(
      catchError((err: HttpErrorResponse) => {
        return of(err.error);
      }),
    ));
    return ImportResultZ.parse(result);
  }
}
