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
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
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
    ConfirmDialogModule,
    InputTextModule,
    InputTextareaModule,
    FormsModule,
    ToastModule,
    RippleModule,
    DropdownModule,
    TagModule,
    TooltipModule
  ],
  providers: [MessageService, ConfirmationService],
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

  // Filters & State
  searchText = '';
  selectedPriority: string | null = null;
  selectedResponsableId: number | null = null;
  connectedUsers = 1;

  priorities = [
    { label: 'Alta', value: 'Alta' },
    { label: 'Media', value: 'Media' },
    { label: 'Baja', value: 'Baja' }
  ];

  filterPriorities = [
    { label: 'Todas las prioridades', value: null },
    { label: 'Alta', value: 'Alta' },
    { label: 'Media', value: 'Media' },
    { label: 'Baja', value: 'Baja' }
  ];

  usuarios: any[] = [];
  filterUsuarios: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private kanbanService: KanbanService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) { }

  ngOnInit(): void {
    this.kanbanService.getUsuarios().subscribe({
      next: (users) => {
        this.usuarios = users.map(u => ({ label: u.nombre, value: u.id }));
        this.filterUsuarios = [
          { label: 'Todos los responsables', value: null },
          ...this.usuarios
        ];
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

        this.subs.add(
          this.kanbanService.connectedUsers$.subscribe(count => {
            this.connectedUsers = count;
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

  clearFilters() {
    this.searchText = '';
    this.selectedPriority = null;
    this.selectedResponsableId = null;
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

  getFilteredTasks(tasks: Task[]): Task[] {
    if (!tasks) return [];
    return tasks.filter(t => {
      const matchText = !this.searchText || 
        t.titulo.toLowerCase().includes(this.searchText.toLowerCase()) || 
        t.descripcion.toLowerCase().includes(this.searchText.toLowerCase());
      
      const matchPriority = !this.selectedPriority || t.prioridad === this.selectedPriority;
      const matchResponsable = !this.selectedResponsableId || t.responsableId === this.selectedResponsableId;

      return matchText && matchPriority && matchResponsable;
    });
  }

  moveColumnLeft(index: number) {
    if (index <= 0) return;
    const current = this.columns[index];
    const prev = this.columns[index - 1];
    
    const prevPrevOrder = index - 2 >= 0 ? this.columns[index - 2].orden : 0;
    const newOrder = (prevPrevOrder + prev.orden) / 2;
    
    current.orden = newOrder;
    this.columns.sort((a, b) => a.orden - b.orden);

    this.kanbanService.updateColumnOrder(current.id, newOrder).subscribe({
      error: () => this.loadBoard()
    });
  }

  moveColumnRight(index: number) {
    if (index >= this.columns.length - 1) return;
    const current = this.columns[index];
    const next = this.columns[index + 1];
    
    const nextNextOrder = index + 2 < this.columns.length ? this.columns[index + 2].orden : next.orden + 65536;
    const newOrder = (next.orden + nextNextOrder) / 2;

    current.orden = newOrder;
    this.columns.sort((a, b) => a.orden - b.orden);

    this.kanbanService.updateColumnOrder(current.id, newOrder).subscribe({
      error: () => this.loadBoard()
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

    this.confirmationService.confirm({
      message: `¿Estás seguro de eliminar la columna "${col.nombre}"?`,
      header: 'Confirmar Eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, eliminar',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.kanbanService.deleteColumn(col.id).subscribe({
          next: () => this.loadBoard(),
          error: (err) => this.messageService.add({severity:'error', summary: 'Error', detail: err.error?.message || 'Error al eliminar'})
        });
      }
    });
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
    this.confirmationService.confirm({
      message: `¿Estás seguro de eliminar la tarea "${task.titulo}"?`,
      header: 'Confirmar Eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, eliminar',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.kanbanService.deleteTask(task.id).subscribe(() => {
          this.loadBoard();
        });
      }
    });
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
    this.kanbanService.exportBoardToPdf(this.projectId, this.selectedPriority, this.selectedResponsableId, this.searchText).subscribe({
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
    this.kanbanService.exportBoardToExcel(this.projectId, this.selectedPriority, this.selectedResponsableId, this.searchText).subscribe({
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
