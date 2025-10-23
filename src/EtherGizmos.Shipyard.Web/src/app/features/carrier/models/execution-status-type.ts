import { StatusTypeMetadata } from "../../package/models/status-type";

export enum ExecutionStatusType {
  Queued = "Queued",
  Running = "Running",
  Successful = "Successful",
  Failed = "Failed",
  TimedOut = "TimedOut",
  Cancelled = "Cancelled",
}

export function getExecutionStatusTypeMetadata(statusType: ExecutionStatusType): StatusTypeMetadata {
  switch (statusType) {
    case ExecutionStatusType.Queued:
      return { label: "Queued", icon: "bi-clock", color: "text-muted" };

    case ExecutionStatusType.Running:
      return { label: "Running", icon: "bi-play-circle", color: "text-info" };

    case ExecutionStatusType.Successful:
      return { label: "Successful", icon: "bi-check-circle", color: "text-success" };

    case ExecutionStatusType.Failed:
      return { label: "Failed", icon: "bi-x-circle", color: "text-danger" };

    case ExecutionStatusType.TimedOut:
      return { label: "Timed out", icon: "bi-hourglass-split", color: "text-warning" };

    case ExecutionStatusType.Cancelled:
      return { label: "Cancelled", icon: "bi-slash-circle", color: "text-muted" };

    default:
      return { label: "Unknown", icon: "bi-question-circle", color: "text-muted" };
  }
}
