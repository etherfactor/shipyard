import { TestBed } from "@angular/core/testing";
import { FormBuilder, Validators } from "@angular/forms";
import { Guid, generateGuid } from "../../types/guid/guid";
import { DefaultControlTypes, formFactoryForModel, getAllFormValues, getDirtyFormValues } from "./form.util";

const factory = formFactoryForModel<TestObject, DefaultControlTypes>(($form, model) => {
  return {
    id: [model.id, Validators.required],
    name: [model.name, Validators.required],
    description: [model.description],
    values: $form.nonNullable.array(model.values),
  };
});

describe('formFactoryFromModel', () => {
  let $form: FormBuilder;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    $form = TestBed.inject(FormBuilder);
  });

  it('should create a form that produces the same object', () => {
    const data = { id: generateGuid(), name: 'Test name', description: null, values: [] };
    const form = factory($form, data);

    expect(form.value).toEqual(jasmine.objectContaining(data));
  });
});

describe('getAllFormValues', () => {
  let $form: FormBuilder;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    $form = TestBed.inject(FormBuilder);
  });

  it('should return the full object', () => {
    const data = { id: generateGuid(), name: 'Test name', description: null, values: [] };
    const form = factory($form, data);

    expect(getAllFormValues(form)).toEqual(data);
  });

  it('should throw an error if the form is invalid', () => {
    const data = { id: generateGuid(), name: undefined!, description: null, values: [] };
    const form = factory($form, data);

    expect(() => getAllFormValues(form)).toThrowError();
  });
});

describe('getDirtyFormValues', () => {
  let $form: FormBuilder;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    $form = TestBed.inject(FormBuilder);
  });

  it('should return the partial object', () => {
    const data = { id: generateGuid(), name: 'Test name', description: null, values: [] };
    const form = factory($form, data);

    form.controls.name.markAsDirty();
    form.controls.values.markAsDirty();
    expect(getDirtyFormValues(form)).toEqual({ name: data.name, values: data.values });
  });

  it('should throw an error if the form is invalid', () => {
    const data = { id: generateGuid(), name: undefined!, description: null, values: [] };
    const form = factory($form, data);

    expect(() => getDirtyFormValues(form)).toThrowError();
  });
});

interface TestObject {
  id: Guid;
  name: string;
  description: string | null;
  values: string[];
}
