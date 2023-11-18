import { ComponentFixture, TestBed } from '@angular/core/testing';

import { X01StatsComponent } from './x01-stats.component';

describe('X01StatsComponent', () => {
  let component: X01StatsComponent;
  let fixture: ComponentFixture<X01StatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ X01StatsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(X01StatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
