import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-user-profile',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-profile.component.html',
  standalone: true,
})
export class UserProfileComponent {
  @Input({ required: true }) userName!: string;
  @Output() submitUserName = new EventEmitter<string>();

  public userProfileForm: FormGroup;

  constructor(private readonly formBuilder: FormBuilder) {
    this.userProfileForm = this.formBuilder.group({
      userName: [
        this.userName,
        [
          Validators.required,
          Validators.minLength(3),
          this.validateDifferenceFromInitial(this.userName),
        ],
      ],
    });
  }

  validateDifferenceFromInitial(initialValue: string | undefined) {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!initialValue) {
        return null;
      }
      const currentValue = control.value;
      return currentValue !== initialValue ? null : { notDifferent: true };
    };
  }

  onSubmit(): void {
    if (this.userProfileForm.valid) {
      const userName = this.userProfileForm.get('userName')?.value;

      this.submitUserName.emit(userName);
    }
  }
}
