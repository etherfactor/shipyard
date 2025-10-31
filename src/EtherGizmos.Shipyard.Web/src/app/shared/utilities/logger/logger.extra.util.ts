import { Injectable } from "@angular/core";
import { LogEnricher, LogEvent, LogLevel, LogSink } from "./logger.util";

@Injectable({
  providedIn: "root"
})
export class ConsoleSink implements LogSink {
  log(event: LogEvent): void {
    const chunks: any[] = [];

    let lastIndex = 0;

    const tokenRegex = /{(?<destructure>@?)(?<property>[A-Za-z0-9_]+)}/g;
    let match: RegExpExecArray | null;
    while ((match = tokenRegex.exec(event.message)) !== null) {
      if (!match.groups)
        continue;

      const { property } = match.groups;
      chunks.push(event.message.substring(lastIndex, match.index).trim());
      chunks.push(event.properties[property]);

      lastIndex += match.index + match[0].length;
    }

    chunks.push(event.message.substring(lastIndex).trim());

    if (event.exception) {
      chunks.push(event.exception);
    }

    switch (event.level) {
      case (LogLevel.Fatal):
      case (LogLevel.Error):
        console.error(...chunks);
        break;

      case (LogLevel.Warning):
        console.warn(...chunks);
        break;

      case (LogLevel.Information):
        console.info(...chunks);
        break;

      case (LogLevel.Debug):
      case (LogLevel.Verbose):
        console.debug(...chunks);
        break;
    }
  }
}

@Injectable({
  providedIn: "root"
})
export class SourceContextEnricher implements LogEnricher {
  enrich(event: LogEvent): Record<string, any> {
    return { SourceContext: event.sourceContext };
  }
}
