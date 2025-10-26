import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ArtifactPreviewModalComponent } from './artifact-preview-modal.component';

describe('ArtifactPreviewModalComponent', () => {
  let component: ArtifactPreviewModalComponent;
  let fixture: ComponentFixture<ArtifactPreviewModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ArtifactPreviewModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ArtifactPreviewModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
