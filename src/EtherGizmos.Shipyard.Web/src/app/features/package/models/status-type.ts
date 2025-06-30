export enum StatusType {
  Unknown = "Unknown",
  Waiting = "Waiting",
  InTransit = "InTransit",
  OutForDelivery = "OutForDelivery",
  Delivered = "Delivered",
  FailedAttempt = "FailedAttempt",
  Returned = "Returned",
  Expired = "Expired",
}

interface StatusTypeMetadata {
  label: string;
  icon: string;
  color: string;
}

export function getStatusTypeMetadata(statusType: StatusType): StatusTypeMetadata {
  switch (statusType) {
    case StatusType.Delivered:
      return { label: "Delivered", icon: "bi-check-circle", color: "text-success" };

    case StatusType.Expired:
      return { label: "Expired", icon: "bi-slash-circle", color: "text-muted" };

    case StatusType.FailedAttempt:
      return { label: "Failed attempt", icon: "bi-x-circle", color: "text-danger" };

    case StatusType.InTransit:
      return { label: "In transit", icon: "bi-truck", color: "text-info" };

    case StatusType.OutForDelivery:
      return { label: "Out for delivery", icon: "bi-box-seam", color: "text-warning" };

    case StatusType.Returned:
      return { label: "Returned", icon: "bi-arrow-counterclockwise", color: "text-danger" };

    case StatusType.Unknown:
      return { label: "Unknown", icon: "bi-question-circle", color: "text-muted" };

    case StatusType.Waiting:
      return { label: "Waiting for pickup", icon: "bi-clock", color: "text-muted" };
  }
}
