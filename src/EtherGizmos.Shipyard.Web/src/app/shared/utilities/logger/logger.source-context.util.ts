import { Injectable } from "@angular/core";
import { LogEnricher, LogEvent } from "./logger.util";

@Injectable({
  providedIn: "root"
})
export class SourceContextEnricher implements LogEnricher {
  enrich(event: LogEvent): Record<string, any> {
    return { SourceContext: event.sourceContext };
  }
}
