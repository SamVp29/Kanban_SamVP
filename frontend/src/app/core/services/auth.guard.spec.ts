import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { authGuard } from './auth.guard';
import { AuthService } from './auth.service';
import { BehaviorSubject } from 'rxjs';

describe('AuthGuard', () => {
  let authServiceMock: any;
  let routerMock: any;
  let currentUserSubject: BehaviorSubject<any>;

  beforeEach(() => {
    currentUserSubject = new BehaviorSubject<any>(null);
    authServiceMock = {
      currentUserValue: null
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    });
  });

  it('debe bloquear la navegación y redirigir a /auth/login si no existe sesión', () => {
    authServiceMock.currentUserValue = null;

    const result = TestBed.runInInjectionContext(() => authGuard({} as any, {} as any));

    expect(result).toBeFalse();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/auth/login']);
  });

  it('debe permitir la navegación si el usuario tiene sesión activa', () => {
    authServiceMock.currentUserValue = { token: 'jwt-token-valido', email: 'admin@kanban.com' };

    const result = TestBed.runInInjectionContext(() => authGuard({} as any, {} as any));

    expect(result).toBeTrue();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});
