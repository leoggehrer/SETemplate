//@CodeCopy
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenericEditComponent } from './generic-edit.component';
import { IKey } from '@app/models/i-key';

describe('GenericEditComponent', () => {
  let component: GenericEditComponent<IKey>;
  let fixture: ComponentFixture<GenericEditComponent<IKey>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GenericEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GenericEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
