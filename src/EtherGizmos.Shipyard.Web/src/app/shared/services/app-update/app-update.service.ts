import { inject, Injectable } from '@angular/core';
import { SwUpdate } from '@angular/service-worker';
import { Subscription } from 'rxjs';
import { Logger } from '../../utilities/logger/logger.util';
import { ToastService } from '../toast/toast.service';

@Injectable({
  providedIn: 'root',
})
export class AppUpdateService {

  private readonly $logger = inject(Logger).forContext("AppUpdateService");
  private readonly $swUpdate = inject(SwUpdate);
  private readonly $toast = inject(ToastService);

  private subscription?: Subscription;

  start() {
    this.subscription?.unsubscribe();

    this.subscription = this.$swUpdate.versionUpdates.subscribe(evt => {
      switch (evt.type) {
        case 'VERSION_DETECTED':
          this.$logger.information("Downloading new app version: {Version}",
            evt.version.hash);
          break;

        case 'VERSION_READY':
          this.$logger.information("Current app version: {OldVersion}; new app version ready for use: {Version}",
            evt.currentVersion.hash,
            evt.latestVersion.hash);

          this.$toast.show({
            header: "Update Available",
            body: "An update for Shipyard is available. Reload the application to apply the update.",
            actions: [
              {
                label: "Reload",
                color: "primary",
                execute: () => window.location.reload(),
              },
            ],
          });
          break;

        case 'VERSION_INSTALLATION_FAILED':
          this.$logger.error(new Error(evt.error),
            "Failed to install app version {Version}",
            evt.version.hash);
          break;

        case 'VERSION_FAILED':
          this.$logger.error(new Error(evt.error),
            "Version {Version} failed",
            evt.version.hash);
          break;
      }
    });

    this.$logger.information("Initialized the app update service");
  }
}
