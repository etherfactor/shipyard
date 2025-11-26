import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PackageExecutionListComponent } from './package-execution-list.component';

describe('PackageExecutionListComponent', () => {
  let component: PackageExecutionListComponent;
  let fixture: ComponentFixture<PackageExecutionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PackageExecutionListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PackageExecutionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
