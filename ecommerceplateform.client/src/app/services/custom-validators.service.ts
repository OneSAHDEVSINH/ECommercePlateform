import { Injectable } from '@angular/core';
import { ValidatorFn, AbstractControl } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class CustomValidatorsService {

  constructor() { }

  static lettersOnly(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (!control.value) {
        return null; // Don't validate empty values
      }

      const valid = /^[A-Za-z\s]+$/.test(control.value);
      return valid ? null : { 'lettersOnly': { value: control.value } };
    };
  }

  static noWhitespaceValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const isWhitespace = control.value && control.value.trim().length === 0;
      return isWhitespace ? { 'whitespace': true } : null;
    };
  }


}
