import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError(error => {
      // Si recibimos 401 (token expirado o inválido)
      if (error.status === 401) {
        // Limpia la sesión y redirige a login
        authService.logout();
      }

      // Re-lanza el error para que lo maneje el componente si es necesario
      return throwError(() => error);
    })
  );
};
