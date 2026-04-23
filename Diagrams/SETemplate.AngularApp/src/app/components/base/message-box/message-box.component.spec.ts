//@BaseCode
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { of, Subject } from 'rxjs';
import { MessageBoxComponent } from './message-box.component';

describe('MessageBoxComponent', () => {
  let component: MessageBoxComponent;
  let fixture: ComponentFixture<MessageBoxComponent>;

  const translateServiceMock = {
    currentLang: 'de',
    onLangChange: new Subject<{ lang: string }>(),
    use: jasmine.createSpy('use').and.returnValue(of({}))
  };

  const activeModalMock = {
    close: jasmine.createSpy('close'),
    dismiss: jasmine.createSpy('dismiss')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MessageBoxComponent],
      providers: [
        { provide: TranslateService, useValue: translateServiceMock },
        { provide: NgbActiveModal, useValue: activeModalMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MessageBoxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
