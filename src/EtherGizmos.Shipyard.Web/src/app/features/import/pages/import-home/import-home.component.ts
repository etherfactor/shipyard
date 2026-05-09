import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { ReadonlyFormDirective } from '../../../../shared/directives/readonly-form/readonly-form.directive';
import { ImportSpec } from '../../models/import-spec';

@Component({
  selector: 'app-import-home',
  imports: [
    DetailBoxComponent,
    DetailHeaderComponent,
    FormsModule,
    NgSelectModule,
    ReadonlyFormDirective,
    RouterModule,
  ],
  templateUrl: './import-home.component.html',
  styleUrl: './import-home.component.scss',
})
export class ImportHomeComponent {
  sourceType = "file";
  document$$ = signal<ImportSpec | undefined>(undefined);

  //importedId: any;
  //sourceType$$ = signal("carrier");
  //sourceText$$ = signal("");
  //canValidate$$ = signal(true);
  //documentInfo$$ = signal({
  //  "kind": "carrier",
  //  "schemaVersion": 1,
  //  "metadata": { "exportedAt": "5/9/2026 5:32:26 PM +00:00" },
  //  "data": {
  //    "name": "USPS",
  //    "slug": "usps",
  //    "steps": [
  //      { "stepType": 1, "url": "https://tools.usps.com/go/TrackAction?tLabels={trackingNumber}" },
  //      { "stepType": 10, "selector": "span.tracking-number" },
  //      { "stepType": 20, "selector": "div.toggle-history-container" },
  //      { "stepType": 10, "selector": "span.tracking-number" },
  //      { "stepType": 1000, "script": "// USPS - History parsing script\n\n// ------------------\n// Utilities\n// ------------------\nconst clean = (s) =>\n  (s ?? \"\")\n    .replace(/\\u00A0/g, \" \")\n    .replace(/&nbsp;/g, \" \")\n    .replace(/\\s+/g, \" \")\n    .trim();\n\nconst safeText = (root, sel) => {\n  const el = root ? root.selectOne(sel) : selectOne(sel);\n  return el ? clean(el.text()) : \"\";\n};\n\nlet isFirstUpdate = true;\nlet firstLocation = null;\n\n// ------------------\n// Activity history\n// ------------------\n(() => {\n  const steps = selectAll(\"div.tb-step\");\n  console.log(`Found ${steps.length} history steps`);\n\n  for (const step of steps) {\n    const description = safeText(step, \".tb-status-detail\");\n    const location    = clean(safeText(step, \".tb-location\"));\n    const occurredRaw = safeText(step, \".tb-date\");\n\n    if (location && isFirstUpdate) {\n      firstLocation = location;\n      isFirstUpdate = false;\n    }\n\n    if (!description && !occurredRaw && !location) {\n      continue; // skip empty rows\n    }\n\n    // USPS usually provides a full date-time string already\n    // e.g., \"October 28, 2025, 3:45 pm\"\n    // Normalize commas like \", \" and ensure single spaces.\n    const occurredAtStr = occurredRaw.replace(/\\s*,\\s*/g, \", \").replace(/\\s+/g, \" \").trim();\n\n    let at = parseDate(occurredAtStr, location);\n\n    // Fallbacks: if the string is split like \"October 28, 2025\" + \"3:45 pm\" inside the same node\n    if (isNaN(new Date(at).getTime())) {\n      // Try stripping any leading words like \"On\", etc.\n      const simplified = occurredAtStr.replace(/^\\b(on|at|updated|arrived)\\b[:\\s]*/i, \"\").trim();\n      at = parseDate(simplified, location);\n    }\n\n    if (isNaN(new Date(at).getTime())) {\n      console.log(\"Could not parse occurredAt:\", occurredAtStr);\n      continue; // we need a valid timestamp\n    }\n\n    console.log(\"At:\", at);\n    console.log(\"Description:\", description);\n    console.log(\"Location:\", location);\n\n    recordEvent({\n      at: at,\n      description,\n      location,\n    });\n  }\n})();\n\n// ------------------\n// ETA (optional)\n// ------------------\n(() => {\n  const etaDay   = safeText(null, \"strong.date\");\n  const etaMonth = safeText(null, \"span.month_year > span:first-child\");\n  const hint     = safeText(null, \"span.month_year span.hint\");\n  const etaYear  = (() => {\n    // Some USPS layouts have two spans inside .month_year (month + year),\n    // some render the year text on the container. Try children first.\n    const container = selectOne(\"span.month_year\");\n    if (!container) return \"\";\n    const all = clean(container.text());\n    console.log(all);\n    return clean(all.replace(etaMonth, \"\"));\n  })().replace(hint, \"\").trim();\n  let etaTime  = safeText(null, \"strong.time\").replace(hint, \"\").trim();\n\n  if (etaDay || etaMonth || etaYear || etaTime) {\n    // Normalize common prefixes like \"by\", \"Before\", etc.\n    etaTime = etaTime.replace(/\\b(by|before|no later than)\\b\\s*/i, \"\").trim();\n\n    const etaStr = `${etaMonth} ${etaDay}, ${etaYear} ${etaTime}`.trim();\n    console.log(\"ETA text:\", etaStr);\n\n    let eta = parseDate(etaStr, firstLocation);\n\n    // If parsing without time fails, try date only; if still bad, skip\n    if (isNaN(new Date(eta).getTime())) {\n      const dateOnly = `${etaMonth} ${etaDay}, ${etaYear}`.trim();\n      eta = parseDate(dateOnly, firstLocation);\n    }\n\n    if (!isNaN(new Date(eta).getTime())) {\n      console.log(\"ETA:\", eta);\n      setEta(eta);\n    } else {\n      console.log(\"Could not parse ETA:\", etaStr);\n    }\n  } else {\n    console.log(\"ETA elements not found (this is fine if no estimated delivery is shown).\");\n  }\n})();\n" }
  //    ],
  //    "rules": [
  //      { "pattern": "(?i)Delivered", "statusType": 100, "priority": 1, "isActive": true },
  //      { "pattern": "(?i)No Authorized Recipient Available", "statusType": -10, "priority": 2, "isActive": true },
  //      { "pattern": "(?i)Out for Delivery", "statusType": 20, "priority": 3, "isActive": true },
  //      { "pattern": "(?i)USPS in possession of item", "statusType": 10, "priority": 7, "isActive": true },
  //      { "pattern": "(?i)Departed Post Office", "statusType": 10, "priority": 7, "isActive": true },
  //      { "pattern": "(?i)Arrived at USPS", "statusType": 10, "priority": 7, "isActive": true },
  //      { "pattern": "(?i)Departed USPS", "statusType": 10, "priority": 7, "isActive": true },
  //      { "pattern": "(?i)In Transit to Next Facility", "statusType": 10, "priority": 7, "isActive": true },
  //      { "pattern": "(?i)Arrived at Post Office", "statusType": 10, "priority": 7, "isActive": true },
  //      { "pattern": "(?i)Accepted at USPS", "statusType": 10, "priority": 7, "isActive": true },
  //      { "pattern": "(?i)Shipment Received, Package Acceptance Pending", "statusType": 10, "priority": 7, "isActive": true },
  //      { "pattern": "(?i)In Transit, Arriving On Time", "statusType": 10, "priority": 7, "isActive": true },
  //      { "pattern": "(?i)Shipping Label Created", "statusType": 1, "priority": 10, "isActive": true },
  //      { "pattern": "(?i)USPS awaiting item", "statusType": 1, "priority": 10, "isActive": true },
  //      { "pattern": "(?i)USPS picked up item", "statusType": 10, "priority": 7, "isActive": true }
  //    ]
  //  }
  //});
  //documentStatusClass$$ = signal("success");
  //documentStatusIcon$$ = signal("bi-check-circle");
  //documentStatusMessage$$ = signal("message");
  //preview$$ = signal(true);
  //result$$ = signal({} as any);
  //canImport$$ = signal(true);

  //onFileSelected(a: any) { }
  //clearImport() { }
  //validateImport() { }
  //getStepSummary(a: any) { }
  //import() { }
}
