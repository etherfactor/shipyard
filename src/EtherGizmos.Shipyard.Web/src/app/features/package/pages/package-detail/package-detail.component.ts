import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DetailBoxComponent } from '../../../../shared/components/detail-box/detail-box.component';
import { DetailHeaderComponent } from '../../../../shared/components/detail-header/detail-header.component';
import { NavbarActionService } from '../../../../shared/services/navbar-action/navbar-action.service';
import { NavbarAction } from '../../../app/components/navbar-action/navbar-action.component';
import { Package } from '../../models/package';
import { getStatusTypeMetadata, StatusType } from '../../models/status-type';
import { PackageService } from '../../services/package/package.service';

@Component({
  selector: 'app-package-detail',
  imports: [
    CommonModule,
    DetailBoxComponent,
    DetailHeaderComponent,
  ],
  templateUrl: './package-detail.component.html',
  styleUrl: './package-detail.component.scss'
})
export class PackageDetailComponent implements OnInit {

  private readonly $package = inject(PackageService);
  private readonly $navbarAction = inject(NavbarActionService);
  private readonly $route = inject(ActivatedRoute);

  readonly id$$ = signal<number | undefined>(undefined);
  readonly package$$ = signal<Package | undefined>(undefined);

  readonly isLoading$$ = computed(() => this.isLoadingStack$$() > 0);
  private readonly isLoadingStack$$ = signal(0);

  readonly actions$$ = computed(() => {
    const actions: NavbarAction[] = [];

    if (!this.isLoading$$()) {
      actions.push({
        icon: "bi-arrow-repeat",
        label: "Repoll",
        callback: this.onRepoll,
      });
      actions.push({
        icon: "bi-pencil",
        label: "Edit",
        callback: this.onEdit,
      });
      actions.push({
        icon: "bi-trash",
        label: "Delete",
        callback: this.onDelete,
      });
    }

    return actions;
  });

  constructor() {
    effect(() => this.$navbarAction.setActions(this.actions$$()));
  }

  ngOnInit(): void {
    const id = parseInt(this.$route.snapshot.paramMap.get("packageId")!);
    this.id$$.set(id);

    this.load();
  }

  getStatusMetadata(statusType: StatusType) {
    return getStatusTypeMetadata(statusType);
  }

  private async load() {
    const id = this.id$$();
    if (!id)
      return;

    this.isLoadingStack$$.set(this.isLoadingStack$$() + 1);

    try {
      const exec = this.$package
        .get(id)
        .expand("carrier", e =>
          e.select("name")
        )
        .expand("trackingUpdates")
        .execute();
      const data = await exec.data;

      this.package$$.set(data);
    } finally {
      this.isLoadingStack$$.set(this.isLoadingStack$$() - 1);
    }
  }

  onRepoll() {

  }

  onEdit() {

  }

  onDelete() {

  }
}
