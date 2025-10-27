import { Component, OnInit, Signal, computed, effect, inject, signal } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable, catchError, throwError } from "rxjs";
import { ZodType } from "zod";
import { NavbarAction } from "../../../../features/app/components/navbar-action/navbar-action.component";
import { NavbarActionService } from "../../../services/navbar-action/navbar-action.service";
import { Bound } from "../../../utilities/bound/bound.util";
import { TypedFormGroup, getDirtyFormValues } from "../../../utilities/form/form.util";

@Component({
  selector: 'app-editable',
  template: ''
})
export abstract class EditableComponent<TEntity, TKey> implements OnInit {

  protected readonly $navbarAction = inject(NavbarActionService);
  protected readonly $route = inject(ActivatedRoute);
  protected readonly $router = inject(Router);
  protected readonly keyParse: ZodType;

  id?: TKey;

  readonly isLoading = computed(() => this.isLoadingStack() > 0);
  private readonly isLoadingStack = signal(0);

  readonly isEditing = signal(false);
  readonly isNew = signal(true);

  entity: TEntity;
  form?: TypedFormGroup<TEntity>;

  constructor(
    keyParse: ZodType,
  ) {
    this.keyParse = keyParse;

    this.entity = {} as TEntity;

    effect(() => {
      const actions = this.actions();
      this.$navbarAction.setActions(actions);
    }, { allowSignalWrites: true });
  }

  protected abstract get actions(): Signal<NavbarAction[]>;

  protected abstract loadRecord(key: TKey): Observable<TEntity>

  protected abstract createEmptyRecord(): TEntity;

  protected abstract loadForm(entity: TEntity): TypedFormGroup<TEntity>;

  protected abstract createRecord(entity: Partial<TEntity>): Observable<TEntity>;

  protected abstract updateRecord(key: TKey, entity: Partial<TEntity>): Observable<TEntity>;

  protected abstract navigateToRecord(entity: TEntity): void;

  ngOnInit(): void {
    const testId = this.$route.snapshot.paramMap.get('id') as TKey;
    if (testId) {
      const parsedId = this.keyParse.parse(testId);
      this.id = parsedId;

      this.isEditing.set(false);
      this.isNew.set(false);
    } else {
      this.id = undefined;

      this.isEditing.set(true);
      this.isNew.set(true);
    }

    this.initialize();
  }

  private initialize(): void {
    if (this.id) {
      this.isLoadingStack.set(this.isLoadingStack() + 1);
      this.loadRecord(this.id).pipe(
        catchError(err => {
          this.isLoadingStack.set(this.isLoadingStack() - 1);
          return throwError(() => err);
        })
      ).subscribe(entity => {
        this.initializeForm(entity);
        this.isLoadingStack.set(this.isLoadingStack() - 1);
      });
    } else {
      const entity = this.createEmptyRecord();
      this.initializeForm(entity);
    }
  }

  private initializeForm(entity: TEntity): void {
    this.entity = entity;

    const form = this.loadForm(entity);

    if (this.isEditing()) {
      form.enable();
    } else {
      form.disable();
    }

    this.form = form;
  }

  @Bound edit(): void {
    this.isEditing.set(true);
    this.form?.enable();
  }

  @Bound cancel(): void {
    this.isEditing.set(false);
    this.form?.disable();

    this.initializeForm(this.entity);
  }

  @Bound save(): void {
    if (!this.form)
      return;

    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    const data = getDirtyFormValues(this.form);
    this.isEditing.set(false);

    if (this.isNew()) {
      this.isLoadingStack.set(this.isLoadingStack() + 1);
      this.createRecord(data).pipe(
        catchError(err => {
          this.isLoadingStack.set(this.isLoadingStack() - 1);
          return throwError(() => err);
        })
      ).subscribe(entity => {
        this.navigateToRecord(entity);
      });
    } else {
      if (!this.id)
        return;

      this.isLoadingStack.set(this.isLoadingStack() + 1);
      this.updateRecord(this.id, data).pipe(
        catchError(err => {
          this.isLoadingStack.set(this.isLoadingStack() - 1);
          return throwError(() => err);
        })
      ).subscribe(entity => {
        this.isLoadingStack.set(this.isLoadingStack() - 1);
        this.initializeForm(entity);
      });
    }
  }
}
