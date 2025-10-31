import { Bound } from "./bound.util";

describe('Bound', () => {
  let test: TestClass;

  beforeEach(() => {
    test = new TestClass();
  });

  it('should reference the bound class when specified', () => {
    const callback = test.isBound;
    const value = callback();

    expect(value).toBe(test);
  });

  it('should not reference the bound class when not specified', () => {
    const callback = test.notBound;
    const value = callback();

    expect(value).not.toBe(test);
  });
});

class TestClass {

  @Bound isBound() {
    return this;
  }

  notBound() {
    return this;
  }
}
