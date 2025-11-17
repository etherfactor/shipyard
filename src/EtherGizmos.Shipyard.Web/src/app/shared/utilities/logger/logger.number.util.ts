import { Injectable } from "@angular/core";
import { LogDestructurer } from "./logger.util";

@Injectable({
  providedIn: 'root',
})
export class NumberDestructurer implements LogDestructurer {
  /**
   * Number of decimal places to retain.
   * Defaults to 3 (e.g., 69.300).
   */
  private readonly precision = 3;

  /**
   * Destructures numeric values by rounding them.
   * Leaves all other values unchanged.
   */
  destructure(value: any): any {
    if (typeof value === "number" && isFinite(value)) {
      // Round to configured precision
      const factor = Math.pow(10, this.precision);
      return Math.round(value * factor) / factor;
    }

    // Return unchanged if not a number
    return value;
  }
}
