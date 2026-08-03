import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { NotfoundComponent } from './demo/components/notfound/notfound.component';
import { AppLayoutComponent } from "./layout/app.layout.component";
import { authGuard } from './core/services/auth.guard';

@NgModule({
    imports: [
        RouterModule.forRoot([
            {
                path: '', component: AppLayoutComponent,
                canActivate: [authGuard],
                children: [
                    { path: '', redirectTo: 'projects', pathMatch: 'full' },
                    { path: 'projects', loadComponent: () => import('./features/projects/projects-list/projects-list.component').then(m => m.ProjectsListComponent) },
                    { path: 'board/:id', loadComponent: () => import('./features/board/board.component').then(m => m.BoardComponent) }
                ]
            },
            { path: 'auth', loadChildren: () => import('./demo/components/auth/auth.module').then(m => m.AuthModule) },
            { path: 'notfound', component: NotfoundComponent },
            { path: '**', redirectTo: '/notfound' },
        ], { scrollPositionRestoration: 'enabled', anchorScrolling: 'enabled', onSameUrlNavigation: 'reload' })
    ],
    exports: [RouterModule]
})
export class AppRoutingModule {
}
