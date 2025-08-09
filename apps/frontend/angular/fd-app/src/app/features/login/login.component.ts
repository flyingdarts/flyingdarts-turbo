import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { AuthressService } from 'src/app/services/authress.service';
import { AppStateActions } from 'src/app/state/app';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authressService: AuthressService,
    private readonly store: Store,
    private readonly router: Router
  ) {
    this.loginForm = this.formBuilder.group({
      acceptTerms: [false, Validators.requiredTrue],
    });
  }
  async ngOnInit(): Promise<void> {
    if (await this.authressService.isLoggedIn()) {
      this.router.navigate(['']);
    }
    this.store.dispatch(AppStateActions.setLoading({ loading: false }));
  }
  async onSubmit(): Promise<void> {
    if (this.loginForm.valid) {
      this.store.dispatch(AppStateActions.setLoading({ loading: true }));
      await this.authressService.authenticate();
    }
  }
}
