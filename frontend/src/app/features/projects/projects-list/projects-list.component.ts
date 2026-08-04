import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ProjectService } from '../../../core/services/project.service';
import { Project } from '../../../core/models/project.model';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DropdownModule } from 'primeng/dropdown';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { FormsModule } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-projects-list',
  standalone: true,
  imports: [
    CommonModule, 
    TableModule, 
    ButtonModule, 
    RippleModule, 
    InputTextModule, 
    InputTextareaModule,
    DropdownModule,
    DialogModule, 
    ConfirmDialogModule,
    FormsModule,
    ToastModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './projects-list.component.html',
  styleUrls: ['./projects-list.component.scss']
})
export class ProjectsListComponent implements OnInit {
  projects: Project[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  
  // Modal state
  projectDialog: boolean = false;
  project: Partial<Project> = {};
  submitted: boolean = false;
  
  estados = [
    { label: 'Activo', value: 'Activo' },
    { label: 'Pausado', value: 'Pausado' },
    { label: 'Completado', value: 'Completado' }
  ];

  // Filtering & Pagination
  lastTableEvent?: TableLazyLoadEvent;
  globalFilter: string = '';

  constructor(
    private projectService: ProjectService,
    private router: Router,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  ngOnInit(): void {
    // Initial load is handled by the table's onLazyLoad event
  }

  loadProjects(event: TableLazyLoadEvent) {
    this.lastTableEvent = event;
    this.loading = true;
    
    const page = (event.first ?? 0) / (event.rows ?? 10) + 1;
    const pageSize = event.rows ?? 10;
    const filter = this.globalFilter;

    this.projectService.getProjects(page, pageSize, filter).subscribe({
      next: (res) => {
        this.projects = res.items;
        this.totalRecords = res.totalRecords;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los proyectos' });
      }
    });
  }
  
  onSearch() {
    if (this.lastTableEvent) {
      this.lastTableEvent.first = 0; // Reset to page 1
      this.loadProjects(this.lastTableEvent);
    }
  }

  openNew() {
    this.project = {
      nombre: '',
      descripcion: '',
      fechaInicio: new Date().toISOString().split('T')[0],
      estado: 'Activo'
    };
    this.submitted = false;
    this.projectDialog = true;
  }

  editProject(p: Project) {
    this.project = { ...p };
    // Format date for input type="date"
    if (this.project.fechaInicio) {
      this.project.fechaInicio = new Date(this.project.fechaInicio).toISOString().split('T')[0];
    }
    if (this.project.fechaFinPrevista) {
      this.project.fechaFinPrevista = new Date(this.project.fechaFinPrevista).toISOString().split('T')[0];
    }
    this.projectDialog = true;
  }

  deleteProject(p: Project) {
    this.confirmationService.confirm({
      message: `¿Estás seguro de eliminar el proyecto "${p.nombre}"?`,
      header: 'Confirmar Eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, eliminar',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.projectService.deleteProject(p.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Proyecto eliminado' });
            if (this.lastTableEvent) this.loadProjects(this.lastTableEvent);
          },
          error: () => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar el proyecto' });
          }
        });
      }
    });
  }

  saveProject() {
    this.submitted = true;
    if (!this.project.nombre?.trim()) return;

    if (this.project.id) {
      this.projectService.updateProject(this.project.id, this.project).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Proyecto actualizado' });
          this.projectDialog = false;
          if (this.lastTableEvent) this.loadProjects(this.lastTableEvent);
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error al actualizar' })
      });
    } else {
      this.projectService.createProject(this.project).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Éxito', detail: 'Proyecto creado' });
          this.projectDialog = false;
          if (this.lastTableEvent) this.loadProjects(this.lastTableEvent);
        },
        error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error al crear' })
      });
    }
  }

  openBoard(project: Project): void {
    this.router.navigate(['/board', project.id]);
  }
}
