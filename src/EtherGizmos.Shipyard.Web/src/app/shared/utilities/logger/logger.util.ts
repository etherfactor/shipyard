import { inject, Provider, Type } from "@angular/core";

export function provideLogger<Constructors extends readonly Type<any>[]>(
  deps: Constructors,
  factory: (...instances: InstancesOf<Constructors>) => Logger
): Provider {
  return {
    provide: Logger,
    useFactory: () => {
      const instances = deps.map(d => inject(d)) as unknown as InstancesOf<Constructors>;
      return factory(...instances);
    },
  };
}

type InstancesOf<T extends readonly Type<any>[]> = {
  [K in keyof T]: T[K] extends Type<infer R> ? R : never
};

/**
 * The severity level of the log event.
 */
export enum LogLevel {
  None = "None",
  Verbose = "Verbose",
  Debug = "Debug",
  Information = "Information",
  Warning = "Warning",
  Error = "Error",
  Fatal = "Fatal",
}

const logLevelMap: Record<LogLevel, number> = {
  None: 0,
  Verbose: 1,
  Debug: 2,
  Information: 3,
  Warning: 4,
  Error: 5,
  Fatal: 6,
};

function levelToOrder(level: LogLevel) {
  return logLevelMap[level];
}

/**
 * Represents a message or event being logged, with extended properties.
 */
export interface LogEvent {
  timestamp: Date;
  level: LogLevel;
  sourceContext?: string;
  message: string;
  exception?: any;
  properties: Record<string, any>;
}

/**
 * Provides options for configuring a logger.
 */
export interface LoggerOptions {
  sinks: LogSink[];
  enrichers: LogEnricher[];
  filters: LogFilter[];
  destructurers: LogDestructurer[];
  minimumLevel: LogLevel;
}

/**
 * Outputs a log event.
 */
export interface LogSink {
  /**
   * Output a log event.
   * @param event The log event.
   */
  log(event: LogEvent): void;
}

/**
 * Adds extended properties to a log event.
 */
export interface LogEnricher {
  /**
   * Add extended properties to a log event.
   * @param event The log event.
   */
  enrich(event: LogEvent): Record<string, any>;
}

/**
 * Destructures properties of a log event.
 */
export interface LogDestructurer {
  /**
   * Destructures a property of a log event.
   * @param value The property to destructure. If not handling this value, return it as-is.
   */
  destructure(value: any): any;
}

/**
 * Filters out log events.
 */
export interface LogFilter {
  /**
   * Determines if a log event should be filtered.
   * @param event The log event.
   */
  filter(event: LogEvent): boolean;
}

type SinkInput = LogSink | (new () => LogSink) | ((event: LogEvent) => void);
type EnrichInput = LogEnricher | (new () => LogEnricher) | ((event: LogEvent) => Record<string, any>);
type FilterInput = LogFilter | (new () => LogFilter) | ((event: LogEvent) => boolean);
type DestructureInput = LogDestructurer | (new () => LogDestructurer) | ((value: any) => any);

/**
 * Configures a {@link Logger} instance.
 */
export class LoggerConfiguration {

  private readonly options: LoggerOptions = {
    destructurers: [],
    enrichers: [],
    filters: [],
    sinks: [],
    minimumLevel: LogLevel.Verbose,
  };

  /**
   * Configures the sinks to which logs will be written.
   */
  readonly writeTo = {
    /**
     * Adds a sink to the logger.
     * @param sink The sink to add.
     * @returns The builder.
    */
    sink: (sink: SinkInput): this => {
      let sinkInstance: LogSink;
      if (typeof sink === "function") {
        if ("prototype" in sink && sink.prototype && "constructor" in sink.prototype) {
          sinkInstance = new (sink as new () => LogSink)();
        } else {
          sinkInstance = { log: sink as (event: LogEvent) => void };
        }
      } else {
        sinkInstance = sink;
      }
      this.options.sinks.push(sinkInstance);
      return this;
    },
  };

  /**
   * Configures the enrichers that will add or modify log properties.
   */
  readonly enrich = {
    /**
     * Adds an enricher to the logger.
     * @param enricher The enricher to add.
     * @returns The builder.
    */
    with: (enricher: EnrichInput): this => {
      let enricherInstance: LogEnricher;
      if (typeof enricher === "function") {
        if ("prototype" in enricher && enricher.prototype && "constructor" in enricher.prototype) {
          enricherInstance = new (enricher as new () => LogEnricher)();
        } else {
          enricherInstance = { enrich: enricher as (event: LogEvent) => Record<string, any> };
        }
      } else {
        enricherInstance = enricher;
      }
      this.options.enrichers.push(enricherInstance);
      return this;
    },
  };

  /**
   * Configures the filters that will omit certain logs.
   */
  readonly filter = {
    /**
     * Adds a filter to the logger.
     * @param filter The filter to add.
     * @returns The builder.
    */
    with: (filter: FilterInput): this => {
      let filterInstance: LogFilter;
      if (typeof filter === "function") {
        if ("prototype" in filter && filter.prototype && "constructor" in filter.prototype) {
          filterInstance = new (filter as new () => LogFilter)();
        } else {
          filterInstance = { filter: filter as (event: LogEvent) => boolean };
        }
      } else {
        filterInstance = filter;
      }
      this.options.filters.push(filterInstance);
      return this;
    },
  };

  /**
   * Configures the destructurers that will process log properties.
   */
  readonly destructure = {
    /**
     * Adds a destructurer to the logger.
     * @param destructurer The destructurer to add.
     * @returns The builder.
    */
    with: (destructurer: DestructureInput): this => {
      let destructurerInstance: LogDestructurer;
      if (typeof destructurer === "function") {
        if ("prototype" in destructurer && destructurer.prototype && "constructor" in destructurer.prototype) {
          destructurerInstance = new (destructurer as new () => LogDestructurer)();
        } else {
          destructurerInstance = { destructure: destructurer as (value: any) => any };
        }
      } else {
        destructurerInstance = destructurer;
      }
      this.options.destructurers.push(destructurerInstance);
      return this;
    },
  };

  /**
   * Configures the minimum level of the logger. The most restrictive filter applies.
   */
  readonly minimumLevel = {
    /**
     * Sets the minimum level of the logger to the specified level.
     * @returns The builder.
    */
    set: (level: LogLevel): this => {
      this.setMinimumLevel(level);
      return this;
    },
    /**
     * Sets the minimum level of the logger to {@link LogLevel.Verbose}.
     * @returns The builder.
     */
    verbose: (): this => {
      this.setMinimumLevel(LogLevel.Verbose);
      return this;
    },
    /**
     * Sets the minimum level of the logger to {@link LogLevel.Debug}.
     * @returns The builder.
     */
    debug: (): this => {
      this.setMinimumLevel(LogLevel.Debug);
      return this;
    },
    /**
     * Sets the minimum level of the logger to {@link LogLevel.Information}.
     * @returns The builder.
     */
    information: (): this => {
      this.setMinimumLevel(LogLevel.Information);
      return this;
    },
    /**
     * Sets the minimum level of the logger to {@link LogLevel.Warning}.
     * @returns The builder.
     */
    warning: (): this => {
      this.setMinimumLevel(LogLevel.Warning);
      return this;
    },
    /**
     * Sets the minimum level of the logger to {@link LogLevel.Error}.
     * @returns The builder.
     */
    error: (): this => {
      this.setMinimumLevel(LogLevel.Error);
      return this;
    },
    /**
     * Sets the minimum level of the logger to {@link LogLevel.Fatal}.
     * @returns The builder.
     */
    fatal: (): this => {
      this.setMinimumLevel(LogLevel.Fatal);
      return this;
    },
  };

  /**
   * Builds the logger.
   * @returns The constructed logger instance.
   */
  createLogger(): Logger {
    return new Logger(this.options);
  }

  private setMinimumLevel(level: LogLevel) {
    if (levelToOrder(this.options.minimumLevel) < levelToOrder(level)) {
      this.options.minimumLevel = level;
    }
  }
}

/**
 * Provides means of logging events and messages.
 */
export class Logger {

  private readonly options: LoggerOptions;
  private readonly context?: string;

  constructor(
    options: LoggerOptions,
    context?: string,
  ) {
    //Duplicate the options so they cannot be changed later
    this.options = { ...options };
    this.context = context;
  }

  /**
   * Creates a new logger, for the specified source context.
   * @param context The new context.
   * @returns The new logger.
   */
  forContext(context: string) {
    return new Logger(this.options, context);
  }

  /**
   * Logs a message at the specified severity level.
   * @param level The severity.
   * @param ex The exception.
   * @param message The message template. Use {Parameter} syntax to set basic values and {@Parameter} syntax for destructured complex values.
   * @param properties The properties to fill in the message template {Parameter} instances, in order from left-to-right.
   */
  log(level: LogLevel, ex: Error, message: string, ...properties: any[]): void;
  log(level: LogLevel, message: string, ...properties: any[]): void;
  log(level: LogLevel, ...params: any[]) {
    if (levelToOrder(level) < levelToOrder(this.options.minimumLevel))
      return;

    //Create a new log event
    const event: LogEvent = {
      timestamp: new Date(),
      level: level,
      sourceContext: this.context,
      properties: {},
    } as LogEvent;

    //Extract the actual arguments based on the call signature
    let properties: any[];
    if (typeof params[0] === "string") {
      event.message = params[0];
      properties = params.slice(1);
    } else {
      event.exception = params[0];
      event.message = params[1];
      properties = params.slice(2);
    }

    //Apply all the enrichers
    for (const enricher of this.options.enrichers) {
      try {
        const newProperties = enricher.enrich(event);
        event.properties = { ...event.properties, ...newProperties };
      } catch (ex) {
        console.error("Log enricher failed", ex);
      }
    }

    //Apply all the filters
    for (const filter of this.options.filters) {
      try {
        if (!filter.filter(event))
          return;
      } catch (ex) {
        console.error("Log filter failed", ex);
      }
    }

    //Extract the properties from the message template
    event.properties = { ...event.properties, ...extractProperties(event.message, properties) };

    //Apply all the destructurers
    for (const key of Object.keys(event.properties)) {
      event.properties[key] = this.options.destructurers.reduce((value, destructurer) => {
        try {
          return destructurer.destructure(value);
        } catch (ex) {
          console.error("Log destructure failed", ex);
          return value;
        }
      }, event.properties[key]);
    }

    //Output the log to all the sinks
    for (const sink of this.options.sinks) {
      try {
        sink.log(event);
      } catch (ex) {
        console.error("Log sink failed", ex);
      }
    }
  }

  /**
   * Logs a verbose message.
   * @param ex The exception.
   * @param message The message template. Use {Parameter} syntax to set basic values and {@Parameter} syntax for destructured complex values.
   * @param properties The properties to fill in the message template {Parameter} instances, in order from left-to-right.
   */
  verbose(ex: Error, message: string, ...properties: any[]): void;
  verbose(message: string, ...properties: any[]): void;
  verbose(...params: any[]) {
    this.log(LogLevel.Verbose, ...(params as [string, ...any[]]));
  }

  /**
   * Logs a debug message.
   * @param ex The exception.
   * @param message The message template. Use {Parameter} syntax to set basic values and {@Parameter} syntax for destructured complex values.
   * @param properties The properties to fill in the message template {Parameter} instances, in order from left-to-right.
   */
  debug(ex: Error, message: string, ...properties: any[]): void;
  debug(message: string, ...properties: any[]): void;
  debug(...params: any[]) {
    this.log(LogLevel.Debug, ...(params as [string, ...any[]]));
  }

  /**
   * Logs an information message.
   * @param ex The exception.
   * @param message The message template. Use {Parameter} syntax to set basic values and {@Parameter} syntax for destructured complex values.
   * @param properties The properties to fill in the message template {Parameter} instances, in order from left-to-right.
   */
  information(ex: Error, message: string, ...properties: any[]): void;
  information(message: string, ...properties: any[]): void;
  information(...params: any[]) {
    this.log(LogLevel.Information, ...(params as [string, ...any[]]));
  }

  /**
   * Logs a warning message.
   * @param ex The exception.
   * @param message The message template. Use {Parameter} syntax to set basic values and {@Parameter} syntax for destructured complex values.
   * @param properties The properties to fill in the message template {Parameter} instances, in order from left-to-right.
   */
  warning(ex: Error, message: string, ...properties: any[]): void;
  warning(message: string, ...properties: any[]): void;
  warning(...params: any[]) {
    this.log(LogLevel.Warning, ...(params as [string, ...any[]]));
  }

  /**
   * Logs an error message.
   * @param ex The exception.
   * @param message The message template. Use {Parameter} syntax to set basic values and {@Parameter} syntax for destructured complex values.
   * @param properties The properties to fill in the message template {Parameter} instances, in order from left-to-right.
   */
  error(ex: Error, message: string, ...properties: any[]): void;
  error(message: string, ...properties: any[]): void;
  error(...params: any[]) {
    this.log(LogLevel.Error, ...(params as [string, ...any[]]));
  }

  /**
   * Logs a fatal message.
   * @param ex The exception.
   * @param message The message template. Use {Parameter} syntax to set basic values and {@Parameter} syntax for destructured complex values.
   * @param properties The properties to fill in the message template {Parameter} instances, in order from left-to-right.
   */
  fatal(ex: Error, message: string, ...properties: any[]): void;
  fatal(message: string, ...properties: any[]): void;
  fatal(...params: any[]) {
    this.log(LogLevel.Fatal, ...(params as [string, ...any[]]));
  }
}

function extractProperties(message: string, properties: any[]) {
  //Create a mapping of property names to values
  const propertyMap: Record<string, any> = {};

  //Track which properties we've processed
  let argIndex = 0;

  //Extract property names by regex
  const tokenRegex = /{(?<destructure>@?)(?<property>[A-Za-z0-9_]+)}/g;
  let match: RegExpExecArray | null;
  while ((match = tokenRegex.exec(message)) !== null) {
    if (!match.groups)
      continue;

    const { destructure, property } = match.groups;

    //If we already have the property, just ignore it; we don't want to increment the index or set the property twice
    if (property in propertyMap)
      continue;

    if (argIndex < properties.length) {
      //Property has a match
      const value = properties[argIndex];
      if (destructure === "@") {
        propertyMap[property] = value;
      } else {
        if (isSimpleValue(value)) {
          //If the value is simple, just store it as-is so it formats more nicely in the console
          propertyMap[property] = value;
        } else {
          //If the value is a complex object, store its string implementation since we don't want to deal with an object
          propertyMap[property] = value.toString();
        }
      }
    } else {
      //Property does not have a match
      propertyMap[property] = "<missing>";
    }

    argIndex++;
  }

  if (argIndex < properties.length) {
    //Property does not have a match
    propertyMap["_unmapped"] = properties.slice(argIndex);
  }

  return propertyMap;
}

function isSimpleValue(value: any) {
  return typeof value === "boolean"
    || typeof value === "number"
    || typeof value === "string"
    || value === null
    || value === undefined;
}
