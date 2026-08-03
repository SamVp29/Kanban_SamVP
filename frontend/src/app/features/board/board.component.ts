import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { KanbanService } from '../../core/services/kanban.service';
import { Column } from '../../core/models/column.model';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Task } from '../../core/models/task.model';
import { Subscription, forkJoin, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { FormsModule } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { RippleModule } from 'primeng/ripple';
import { DropdownModule } from 'primeng/dropdown';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [
    CommonModule, 
    DragDropModule, 
    ButtonModule, 
    CardModule, 
    DialogModule, 
    InputTextModule,
    InputTextareaModule,
    FormsModule,
    ToastModule,
    RippleModule,
    DropdownModule,
    TagModule,
    TooltipModule
  ],
  providers: [MessageService],
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss']
})
export class BoardComponent implements OnInit, OnDestroy {
  columns: Column[] = [];
  projectId!: number;
  private subs = new Subscription();
  loading = true;

  // Dialogs
  taskDialog = false;
  colDialog = false;
  
  // Forms
  taskForm: Partial<Task> = {};
  colForm: Partial<Column> = {};
  submitted = false;

  priorities = [
    { label: 'Alta', value: 'Alta' },
    { label: 'Media', value: 'Media' },
    { label: 'Baja', value: 'Baja' }
  ];

  usuarios: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private kanbanService: KanbanService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.kanbanService.getUsuarios().subscribe({
      next: (users) => {
        this.usuarios = users.map(u => ({ label: u.nombre, value: u.id }));
      }
    });

    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.projectId = +id;
        this.loadBoard();
        this.kanbanService.startConnection(this.projectId);
        
        this.subs.add(
          this.kanbanService.boardUpdated$.subscribe(() => {
             this.loadBoard(); 
          })
        );
      }
    });
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
    if (this.projectId) {
      this.kanbanService.stopConnection(this.projectId);
    }
  }

  loadBoard(): void {
    this.loading = true;
    this.kanbanService.getColumns(this.projectId).pipe(
      switchMap(columns => {
        if (columns.length === 0) return of([]);
        const taskRequests = columns.map(c => 
          this.kanbanService.getTasksByColumn(c.id).pipe(
            map(tasks => {
              c.tareas = tasks.sort((a, b) => a.orden - b.orden);
              return c;
            })
          )
        );
        return forkJoin(taskRequests);
      })
    ).subscribe({
      next: (cols) => {
        this.columns = cols.sort((a, b) => a.orden - b.orden);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.messageService.add({severity:'error', summary: 'Error', detail: 'No se pudo cargar el tablero'});
      }
    });
  }

  // --- Drag & Drop ---
  drop(event: CdkDragDrop<Task[]>, targetColumnId: number) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      this.updateTaskOrderOptimistic(event.container.data, targetColumnId);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
      this.updateTaskOrderOptimistic(event.container.data, targetColumnId);
    }
  }

  updateTaskOrderOptimistic(tasks: Task[], columnId: number) {
    // Calculates simple lex order for MVP
    // Send to backend
    tasks.forEach((task, index) => {
        const order = (index + 1) * 65536;
        if (task.columnaId !== columnId || task.orden !== order) {
            task.columnaId = columnId;
            task.orden = order;
            this.kanbanService.updateTaskColumn(task.id, columnId, order).subscribe({
                error: () => this.loadBoard() // Revert on failure
            });
        }
    });
  }

  // --- Column CRUD ---
  openNewColumn() {
    this.colForm = { proyectoId: this.projectId, nombre: '' };
    this.submitted = false;
    this.colDialog = true;
  }

  saveColumn() {
    this.submitted = true;
    if (!this.colForm.nombre?.trim()) return;

    if (this.colForm.id) {
      this.kanbanService.updateColumn(this.colForm.id, this.colForm.nombre).subscribe(() => {
        this.colDialog = false;
        // SignalR will refresh us
      });
    } else {
      this.kanbanService.createColumn(this.colForm).subscribe(() => {
        this.colDialog = false;
      });
    }
  }

  editColumn(col: Column) {
    this.colForm = { ...col };
    this.colDialog = true;
  }

  deleteColumn(col: Column) {
    if (col.tareas && col.tareas.length > 0) {
      this.messageService.add({severity:'warn', summary: 'Advertencia', detail: 'No se puede eliminar una columna con tareas.'});
      return;
    }
    if (confirm(`¿Eliminar la columna ${col.nombre}?`)) {
      this.kanbanService.deleteColumn(col.id).subscribe({
        next: () => this.loadBoard(), // Wait for SignalR or reload
        error: (err) => this.messageService.add({severity:'error', summary: 'Error', detail: err.error?.message || 'Error al eliminar'})
      });
    }
  }

  // --- Task CRUD ---
  openNewTask(columnId: number) {
    this.taskForm = { columnaId: columnId, titulo: '', descripcion: '', prioridad: 'Media' };
    this.submitted = false;
    this.taskDialog = true;
  }

  editTask(task: Task) {
    this.taskForm = { ...task };
    this.taskDialog = true;
  }

  deleteTask(task: Task) {
    if (confirm(`¿Eliminar la tarea ${task.titulo}?`)) {
      this.kanbanService.deleteTask(task.id).subscribe(() => {
        this.loadBoard();
      });
    }
  }

  saveTask() {
    this.submitted = true;
    if (!this.taskForm.titulo?.trim()) return;

    if (this.taskForm.id) {
      this.kanbanService.updateTask(this.taskForm.id, this.taskForm).subscribe(() => {
        this.taskDialog = false;
      });
    } else {
      this.kanbanService.createTask(this.taskForm).subscribe(() => {
        this.taskDialog = false;
      });
    }
  }

  getPrioritySeverity(priority: string) {
    switch (priority?.toLowerCase()) {
        case 'alta': return 'danger';
        case 'media': return 'warning';
        case 'baja': return 'success';
        default: return 'info';
    }
  }

  getResponsableName(id?: number): string {
    if (!id) return '';
    const user = this.usuarios.find(u => u.value === id);
    return user ? user.label : 'Asignado';
  }

  exportPdf() {
    this.kanbanService.exportBoardToPdf(this.projectId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Tablero_${this.projectId}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => this.messageService.add({severity:'error', summary: 'Error', detail: 'No se pudo exportar el PDF'})
    });
  }

  exportExcel() {
    this.kanbanService.exportBoardToExcel(this.projectId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Tablero_${this.projectId}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => this.messageService.add({severity:'error', summary: 'Error', detail: 'No se pudo exportar a Excel'})
    });
  }
}
