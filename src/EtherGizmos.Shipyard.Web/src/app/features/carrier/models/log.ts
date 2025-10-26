import { z } from "zod";
import { DateTimeZ } from "../../../shared/types/datetime/datetime";

export const LogZ = z.object({
  "@t": DateTimeZ,
  "@m": z.string().nullish(),
  "{OriginalFormat}": z.string().nullish(),
  "@i": z.string(),
  "@l": z.string().nullish(),
}).passthrough().transform(data => {
  const properties: Record<string, any> = { ...data };
  delete properties["@t"];
  delete properties["@m"];
  delete properties["{OriginalFormat}"];
  delete properties["@i"];
  delete properties["@l"];

  const output = {
    timestamp: data["@t"],
    level: data["@l"] ?? "Information",
    message: data["{OriginalFormat}"] ?? data["@m"] ?? "",
    id: data["@i"],
    properties,
  };

  if (data["{OriginalFormat}"]) {
    // Handle escaped braces like Serilog: "{{" -> "{" and "}}" -> "}"
    const OPEN = "\uFFF0";
    const CLOSE = "\uFFF1";
    let tmpl = output.message.replaceAll("{{", OPEN).replaceAll("}}", CLOSE);

    // {token}, {path.to.value}, {@obj}, {num:0.00}, {$literal}
    tmpl = tmpl.replace(/{([^{}]+)}/g, (_m, raw: string) => {
      let token = raw.trim();

      let destructure = false; // {@obj}
      let literal = false;     // {$x} (we'll just String(x))
      if (token.startsWith("@")) { destructure = true; token = token.slice(1); }
      else if (token.startsWith("$")) { literal = true; token = token.slice(1); }

      const val = properties[token];
      if (val === undefined) return `{${raw}}`;

      if (destructure) return safeJson(val);
      return String(val);
    });

    // restore escaped braces
    output.message = tmpl.replaceAll(OPEN, "{").replaceAll(CLOSE, "}");
  }

  return output;
});

const safeJson = (v: any) => {
  try { return JSON.stringify(v); } catch { return String(v); }
};

export interface Log extends z.infer<typeof LogZ> { }
