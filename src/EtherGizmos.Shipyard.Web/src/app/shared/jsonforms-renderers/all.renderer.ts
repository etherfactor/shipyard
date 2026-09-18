import { JsonFormsRendererRegistryEntry } from "@jsonforms/core";
import { BooleanControlRenderer, booleanControlTester } from "./boolean.renderer";
import { DateControlRenderer, dateControlTester } from "./date-renderer";

export const bootstrapRenderers: JsonFormsRendererRegistryEntry[] = [
  {
    tester: booleanControlTester,
    renderer: BooleanControlRenderer,
  },
  {
    tester: dateControlTester,
    renderer: DateControlRenderer,
  },
];
