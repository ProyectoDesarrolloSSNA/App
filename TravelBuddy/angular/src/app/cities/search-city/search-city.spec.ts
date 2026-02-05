import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchCity } from './search-city';

describe('SearchCity', () => {
  let component: SearchCity;
  let fixture: ComponentFixture<SearchCity>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchCity]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SearchCity);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
