import { Directive, HostListener } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';

@Directive({
  selector: 're-captcha',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: RecaptchaValueAccessorDirective,
      multi: true,
    },
  ],
})
export class RecaptchaValueAccessorDirective implements ControlValueAccessor {
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: any): void {
    // No need to implement this for reCAPTCHA
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    // No need to implement this for reCAPTCHA
  }

  @HostListener('resolved', ['$event'])
  onResolved(token: string) {
    this.onChange(token);
    this.onTouched();
  }
}
