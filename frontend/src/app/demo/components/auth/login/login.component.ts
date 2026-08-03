import { Component } from '@angular/core';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styles: [`
        :host ::ng-deep .pi-eye,
        :host ::ng-deep .pi-eye-slash {
            transform:scale(1.6);
            margin-right: 1rem;
            color: var(--primary-color) !important;
        }
    `]
})
export class LoginComponent {

    email!: string;
    password!: string;
    loading = false;
    errorMsg = '';

    constructor(
        public layoutService: LayoutService,
        private authService: AuthService,
        private router: Router
    ) { }

    onLogin() {
        if (!this.email || !this.password) return;

        this.loading = true;
        this.errorMsg = '';

        this.authService.login(this.email, this.password).subscribe({
            next: (res) => {
                this.router.navigate(['/']);
            },
            error: (err) => {
                this.loading = false;
                this.errorMsg = 'Credenciales inválidas o error en el servidor.';
                console.error(err);
            }
        });
    }
}
