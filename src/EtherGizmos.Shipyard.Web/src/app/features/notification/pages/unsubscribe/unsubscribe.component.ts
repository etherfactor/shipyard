import { Component, computed, inject, OnInit, signal } from "@angular/core";
import { ActivatedRoute, Router, RouterModule } from "@angular/router";
import { NotificationSubscriptionService } from "../../services/notification-subscription/notification-subscription.service";

@Component({
  selector: "app-unsubscribe",
  imports: [
    RouterModule,
  ],
  templateUrl: "./unsubscribe.component.html",
  styleUrl: "./unsubscribe.component.scss",
})
export class UnsubscribeComponent implements OnInit {
  private readonly $route = inject(ActivatedRoute);
  private readonly $router = inject(Router);
  private readonly $subscription = inject(NotificationSubscriptionService);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly key$$ = signal<string | undefined>(undefined);

  readonly isUnsubscribing$$ = computed(() => this.isUnsubscribingStack$$() > 0);
  private readonly isUnsubscribingStack$$ = signal(0);

  readonly isFailed$$ = signal(false);

  ngOnInit(): void {
    try {
      this.id$$.set(parseInt(this.$route.snapshot.queryParamMap.get("id")!));
    } catch { }

    this.key$$.set(this.$route.snapshot.queryParamMap.get("key") ?? undefined);
  }

  async unsubscribe() {
    this.isUnsubscribingStack$$.set(this.isUnsubscribingStack$$() + 1);
    let result = false;
    try {
      const id = this.id$$();
      const key = this.key$$();

      if (!id || !key) {
        throw new Error("Invalid query parameters");
      }

      result = await this.$subscription.unsubscribe(id, key);

      if (result) {
        this.$router.navigate(["/notifications/unsubscribed"], { replaceUrl: true });
      }
    } catch {
      result = false;
    } finally {
      this.isUnsubscribingStack$$.set(this.isUnsubscribingStack$$() - 1);
    }

    this.isFailed$$.set(!result);
  }
}
