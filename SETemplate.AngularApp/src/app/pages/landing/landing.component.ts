//@AiCode
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent {
  features = [
    { icon: 'bi-shield-check', titleKey: 'LANDING.FEATURES.SECURE.TITLE', textKey: 'LANDING.FEATURES.SECURE.TEXT' },
    { icon: 'bi-lightning-charge', titleKey: 'LANDING.FEATURES.FAST.TITLE', textKey: 'LANDING.FEATURES.FAST.TEXT' },
    { icon: 'bi-code-slash', titleKey: 'LANDING.FEATURES.MODERN.TITLE', textKey: 'LANDING.FEATURES.MODERN.TEXT' },
    { icon: 'bi-phone', titleKey: 'LANDING.FEATURES.RESPONSIVE.TITLE', textKey: 'LANDING.FEATURES.RESPONSIVE.TEXT' },
    { icon: 'bi-gear', titleKey: 'LANDING.FEATURES.FLEXIBLE.TITLE', textKey: 'LANDING.FEATURES.FLEXIBLE.TEXT' },
    { icon: 'bi-globe', titleKey: 'LANDING.FEATURES.I18N.TITLE', textKey: 'LANDING.FEATURES.I18N.TEXT' }
  ];
}
