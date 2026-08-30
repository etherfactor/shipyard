import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JsonSchemaAutoFormComponent } from './json-schema-auto-form.component';

describe('JsonSchemaAutoFormComponent', () => {
  let component: JsonSchemaAutoFormComponent;
  let fixture: ComponentFixture<JsonSchemaAutoFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JsonSchemaAutoFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JsonSchemaAutoFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
