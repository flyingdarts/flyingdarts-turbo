import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-user-sign-up',
  templateUrl: './user-sign-up.component.html',
  imports: [ReactiveFormsModule, CommonModule],
  standalone: true,
})
export class UserSignUpComponent {
  signUpForm: FormGroup;

  constructor(private readonly formBuilder: FormBuilder) {
    this.signUpForm = this.formBuilder.group({
      userName: ['', [Validators.required, Validators.minLength(3)]],
    });
  }

  onSubmit(): void {
    if (this.signUpForm.valid) {
      const userName = this.signUpForm.get('userName')?.value;
      sessionStorage.setItem('userName', userName);
    }
  }
}
