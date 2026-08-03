import { OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { LayoutService } from './service/app.layout.service';

@Component({
    selector: 'app-menu',
    templateUrl: './app.menu.component.html'
})
export class AppMenuComponent implements OnInit {

    model: any[] = [];

    constructor(public layoutService: LayoutService) { }

    ngOnInit() {
        this.model = [
            {
                label: 'Gestión Ágil',
                items: [
                    { label: 'Proyectos', icon: 'pi pi-fw pi-briefcase', routerLink: ['/projects'] },
                    { label: 'Tablero Kanban', icon: 'pi pi-fw pi-th-large', routerLink: ['/board/1'] }
                ]
            }
        ];
    }
}
