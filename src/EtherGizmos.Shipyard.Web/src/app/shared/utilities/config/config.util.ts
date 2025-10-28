import { InjectionToken } from "@angular/core";
import { z } from "zod";

const AppConfigZ = z.object({
  production: z.boolean(),
  resourceServer: z.string(),
  oauth: z.object({
    authority: z.string(),
    clientId: z.string(),
    scope: z.string(),
  }),
  version: z.string(),
});

export interface AppConfig extends z.infer<typeof AppConfigZ> { }

export const APP_CONFIG = new InjectionToken<AppConfig>("config.json");

export async function fetchConfig() {
  const result = await fetch("/assets/config.json");
  const json = await result.json();
  const config = AppConfigZ.parse(json);

  return config;
}
