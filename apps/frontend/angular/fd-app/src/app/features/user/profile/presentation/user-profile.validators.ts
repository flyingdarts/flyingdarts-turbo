import { AbstractControl, ValidationErrors } from '@angular/forms';

export class UserProfileValidators {
  static validateDifferenceFromInitial(initialValue: string | undefined) {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!initialValue) {
        return null;
      }
      const currentValue = control.value;
      return currentValue !== initialValue ? null : { notDifferent: true };
    };
  }
}
