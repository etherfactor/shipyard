import { Injectable } from "@angular/core";
import { LogEnricher, LogEvent, LogLevel, LogSink } from "./logger.util";

interface TemplateArgs {
  template: string;
  args: any[];
}

const isDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
const COLORS = {
  get FmtSystem() { return "#808080"; },

  get FmtNumber() { return isDark ? "#ff00ff" : "#800080"; },

  get FmtString() { return isDark ? "#00ffff" : "#00ced1"; },

  get TxtLight() { return "#ffffff"; },

  get TxtDark() { return "#111111"; },

  get BgFatal() { return isDark ? "#7a1c1c" : "#d63333"; },
  get TxtFatal() { return isDark ? "#ffffff" : "#ffffff"; },

  get BgError() { return isDark ? "#4a2024" : "#f8d7da"; },
  get TxtError() { return isDark ? "#f2a7af" : "#842029"; },

  get BgWarning() { return isDark ? "#3e3317" : "#fff3cd"; },
  get TxtWarning() { return isDark ? "#f6d173" : "#8a6d1b"; },

  get BgInformation() { return isDark ? "#16352b" : "#d9f2e4"; },
  get TxtInformation() { return isDark ? "#8edbb7" : "#1c6844"; },

  get BgDebug() { return isDark ? "#243248" : "#d7e3f8"; },
  get TxtDebug() { return isDark ? "#9fc3ff" : "#1b4f8a"; },

  get BgVerbose() { return isDark ? "#2e3338" : "#e7e9ec"; },
  get TxtVerbose() { return isDark ? "#cfd4da" : "#495057"; },
};

const LEVEL_COLORS: Record<LogLevel, { fg: string, bg: string }> = {
  [LogLevel.Fatal]: { fg: COLORS.TxtFatal, bg: COLORS.BgFatal },
  [LogLevel.Error]: { fg: COLORS.TxtError, bg: COLORS.BgError },
  [LogLevel.Warning]: { fg: COLORS.TxtWarning, bg: COLORS.BgWarning },
  [LogLevel.Information]: { fg: COLORS.TxtInformation, bg: COLORS.BgInformation },
  [LogLevel.Debug]: { fg: COLORS.TxtDebug, bg: COLORS.BgDebug },
  [LogLevel.Verbose]: { fg: COLORS.TxtVerbose, bg: COLORS.BgVerbose },
  [LogLevel.None]: { fg: "#111111", bg: "#ffffff" },
};

function levelToLabel(level: LogLevel): string {
  switch (level) {
    case LogLevel.Debug:
      return "DBG";

    case LogLevel.Error:
      return "ERR";

    case LogLevel.Fatal:
      return "FTL";

    case LogLevel.Information:
      return "INF";

    case LogLevel.Verbose:
      return "VRB";

    case LogLevel.Warning:
      return "WRN";

    default:
      return "???";
  }
}

function formatMessage(template: string, properties: Record<string, any>): TemplateArgs {
  const result: TemplateArgs = {
    template: "",
    args: [],
  };
  let lastIndex = 0;

  template = template.replace(/%/g, "%%");

  const tokenRegex = /{(?<destructure>@?)(?<property>[A-Za-z0-9_]+)}/g;
  let match: RegExpExecArray | null;
  while ((match = tokenRegex.exec(template)) !== null) {
    if (!match.groups)
      continue;

    const { property } = match.groups;
    result.template += template.substring(lastIndex, match.index);

    const formatted = formatValue(properties[property]);
    result.template += formatted.template;
    result.args = [...result.args, ...formatted.args];

    lastIndex = match.index + match[0].length;
  }

  result.template += template.substring(lastIndex);

  //if (event.exception) {
  //  chunks.push(event.exception);
  //}

  return result;
}

function formatValue(input: unknown): TemplateArgs {
  if (input === null || input === "undefined") {
    return colorValue(COLORS.FmtString, input);
  } else {
    switch (typeof input) {
      case "bigint":
      case "number":
        return colorValue(COLORS.FmtNumber, input);

      case "boolean":
      case "string":
        return colorValue(COLORS.FmtString, input);

      default:
        return {
          template: "%o",
          args: [input],
        };
    }
  }
}

function colorValue(color: string, input: any): TemplateArgs {
  let marker: string;
  if (input === null || input === undefined) {
    marker = "%s";
  } else {
    switch (typeof input) {
      case "bigint":
      case "boolean":
      case "number":
      case "string":
        marker = "%s";
        break;

      default:
        marker = "%o";
        break;
    }
  }

  return {
    template: `%c${marker}%c`,
    args: [`color:${color};`, input, "color:inherit;"],
  };
}

const RESET_CSS = "color:inherit;background:transparent;font-weight:inherit;padding:0;margin:0;border-radius:0";
function formatBadge(label: string, foreground: string, background: string): TemplateArgs {
  return {
    template: `%c${label}%c`,
    args: [
      `color:${foreground};background:${background};border-radius:4px;padding:0 6px;font-weight:600`,
      RESET_CSS,
    ],
  };
}

@Injectable({
  providedIn: "root"
})
export class ConsoleSink implements LogSink {
  log(event: LogEvent): void {
    const timestamp = event.timestamp.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false });
    const level = levelToLabel(event.level);

    const formatted = formatMessage(event.message, event.properties);

    const prefixTime = formatBadge(timestamp, COLORS.TxtLight, COLORS.FmtSystem);
    const prefixLevel = formatBadge(level, LEVEL_COLORS[event.level].fg, LEVEL_COLORS[event.level].bg);

    formatted.template = `${prefixTime.template} ${prefixLevel.template} ${formatted.template}`;
    formatted.args = [...prefixTime.args, ...prefixLevel.args, ...formatted.args];

    switch (event.level) {
      case (LogLevel.Fatal):
      case (LogLevel.Error):
        console.error(formatted.template, ...formatted.args);
        break;

      case (LogLevel.Warning):
        console.warn(formatted.template, ...formatted.args);
        break;

      case (LogLevel.Information):
        console.info(formatted.template, ...formatted.args);
        break;

      case (LogLevel.Debug):
      case (LogLevel.Verbose):
        console.debug(formatted.template, ...formatted.args);
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
