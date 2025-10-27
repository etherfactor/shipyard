import { TestBed } from '@angular/core/testing';
import { IteratePipe } from './iterate.pipe';

describe('IteratePipe', () => {
  let pipe: IteratePipe;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IteratePipe],
      providers: [IteratePipe]
    })
    .compileComponents();

    pipe = TestBed.inject(IteratePipe);
  });

  it('should create an instance', () => {
    expect(pipe).toBeTruthy();
  });

  it('should transform a number into an array', () => {
    const result = pipe.transform(10);
    expect(result.length).toBe(10);
    expect(result).toEqual([0, 1, 2, 3, 4, 5, 6, 7, 8, 9]);
  });
});
