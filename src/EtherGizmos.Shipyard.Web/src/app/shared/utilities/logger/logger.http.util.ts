import { HttpClient } from "@angular/common/http";
import { inject, Injectable, InjectionToken } from "@angular/core";
import { LogEvent, LogLevel, LogSink } from "./logger.util";

export interface HttpBatchLogSinkOptions {
  /** Endpoint that accepts POST of LogEntry[] */
  endpoint: string;

  /** Additional headers to pass to the endpoint */
  headers?: Record<string, string>;

  /** Max number of entries per batch before immediate flush (default: 20) */
  maxBatchSize?: number;

  /** Max time to wait before flushing (ms) even if batch isn’t full (default: 2000) */
  maxIntervalMs?: number;

  /** Hard cap on buffered entries; oldest are dropped beyond this (default: 2000) */
  maxQueueSize?: number;

  /** Flush immediately when levels match these (default: ["Error","Fatal"]) */
  immediateFlushLevels?: Array<"Error" | "Fatal" | "Warning" | "Information" | "Debug" | "Verbose" | "None">;
}

export const HTTP_BATCH_LOG_SINK_OPTIONS =
  new InjectionToken<HttpBatchLogSinkOptions>('HTTP_BATCH_LOG_SINK_OPTIONS');

interface LogEntry {
  timestamp: string;
  severity: string,
  sourceContext?: string,
  message: string,
  exception?: {
    message: string,
    stackTrace?: string,
  },
  properties: unknown,
}

@Injectable({
  providedIn: "root"
})
export class HttpBatchLogSink implements LogSink {
  private readonly options: Required<HttpBatchLogSinkOptions>;
  private readonly buffer: LogEntry[] = [];
  private timerId: ReturnType<typeof setTimeout> | null = null;
  private inflight = false;
  private retryDelayMs = 0;

  constructor(
    private readonly http: HttpClient
  ) {
    const options = inject(HTTP_BATCH_LOG_SINK_OPTIONS);
    this.options = {
      endpoint: options.endpoint,
      headers: options.headers ?? {},
      maxBatchSize: options.maxBatchSize ?? 20,
      maxIntervalMs: options.maxIntervalMs ?? 2000,
      maxQueueSize: options.maxQueueSize ?? 2000,
      immediateFlushLevels: options.immediateFlushLevels ?? ["Error", "Fatal"],
    };

    // Flush on tab close/backgrounding
    if (typeof window !== "undefined") {
      window.addEventListener("beforeunload", () => this.flush());
      document.addEventListener("visibilitychange", () => {
        if (document.visibilityState === "hidden") this.flush();
      });
      addEventListener("pagehide", () => this.flush());
    }
  }

  log(event: LogEvent): void {
    const entry = this.mapToLogEntry(event);

    // Enforce queue cap: drop oldest if needed
    if (this.buffer.length >= this.options.maxQueueSize) {
      this.buffer.splice(0, this.buffer.length - this.options.maxQueueSize + 1);
    }
    this.buffer.push(entry);

    // Immediate flush for important levels
    if (this.shouldImmediateFlush(event.level) || this.buffer.length >= this.options.maxBatchSize) {
      this.flush();
      return;
    }

    // Flush events after the timer expires
    if (this.timerId === null) {
      this.timerId = setTimeout(() => {
        this.timerId = null;
        this.flush();
      }, this.options.maxIntervalMs);
    }
  }

  /** Flush the buffer */
  private flush(): void {
    if (this.buffer.length === 0) return;

    // Avoid parallel posts
    if (this.inflight) return;

    // If we trigger a flush early, no need for the timer to go off
    if (this.timerId) {
      clearTimeout(this.timerId);
      this.timerId = null;
    }

    // Pull a batch of logs off the buffer to send
    const batch = this.buffer.splice(0, this.options.maxBatchSize);

    this.inflight = true;
    this.http.post(this.options.endpoint, batch, { headers: this.options.headers, keepalive: true }).subscribe({
      next: () => {
        this.inflight = false;
        this.retryDelayMs = 0; // reset backoff

        // If more remain, schedule immediate-or-timed flush
        if (this.buffer.length >= this.options.maxBatchSize) {
          this.flush();
        } else if (this.buffer.length > 0 && this.timerId === null) {
          this.timerId = setTimeout(() => {
            this.timerId = null;
            this.flush();
          }, this.options.maxIntervalMs);
        }
      },
      error: () => {
        this.inflight = false;

        // Re-queue the failed batch at the front
        this.buffer.unshift(...batch);

        // Exponential backoff (capped)
        this.retryDelayMs = Math.min(this.retryDelayMs === 0 ? 1000 : this.retryDelayMs * 2, 30000);

        if (this.timerId) clearTimeout(this.timerId);
        this.timerId = setTimeout(() => {
          this.timerId = null;
          this.flush();
        }, this.retryDelayMs);
      },
    });
  }

  private shouldImmediateFlush(level: LogLevel): boolean {
    const name = this.mapSeverity(level);
    return this.options.immediateFlushLevels.includes(name as any);
  }

  private mapToLogEntry(event: LogEvent): LogEntry {
    return {
      timestamp: event.timestamp?.toISOString(),
      severity: this.mapSeverity(event.level),
      sourceContext: event.sourceContext,
      message: event.message,
      exception: this.mapException(event.exception),
      properties: this.safeSerialize(event.properties),
    };
  }

  private mapSeverity(level: LogLevel): string {
    switch (level) {
      case LogLevel.Verbose: return "Verbose";
      case LogLevel.Debug: return "Debug";
      case LogLevel.Information: return "Information";
      case LogLevel.Warning: return "Warning";
      case LogLevel.Error: return "Error";
      case LogLevel.Fatal: return "Fatal";
      default: return "None";
    }
  }

  private mapException(ex: any): { message: string; stackTrace?: string } | undefined {
    if (!ex) return undefined;
    try {
      if (typeof ex === "string") return { message: ex };
      if (ex instanceof Error) return { message: ex.message ?? String(ex), stackTrace: ex.stack ?? undefined };
      const msg = ex.message ?? ex.toString?.() ?? JSON.stringify(ex);
      const stack = ex.stack ?? undefined;
      return { message: String(msg), stackTrace: typeof stack === "string" ? stack : undefined };
    } catch {
      return { message: "Unknown exception" };
    }
  }

  private safeSerialize(obj: Record<string, any>): unknown {
    if (!obj) return undefined;
    try {
      return JSON.parse(JSON.stringify(obj));
    } catch {
      return { _serializationError: true };
    }
  }
}
