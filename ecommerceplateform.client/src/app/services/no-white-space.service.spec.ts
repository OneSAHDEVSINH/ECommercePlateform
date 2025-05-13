import { TestBed } from '@angular/core/testing';

import { NoWhiteSpaceService } from './no-white-space.service';

describe('NoWhiteSpaceService', () => {
  let service: NoWhiteSpaceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NoWhiteSpaceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
