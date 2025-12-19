import { TestBed } from '@angular/core/testing';

import { Cities } from './cities';

describe('Cities', () => {
  let service: Cities;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Cities);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
