import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BoardComponent } from './board.component';
import { Task } from '../../core/models/task.model';
import { KanbanService } from '../../core/services/kanban.service';
import { ActivatedRoute } from '@angular/router';
import { MessageService, ConfirmationService } from 'primeng/api';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('BoardComponent', () => {
  let component: BoardComponent;
  let fixture: ComponentFixture<BoardComponent>;
  let kanbanServiceMock: any;

  beforeEach(async () => {
    kanbanServiceMock = {
      getUsuarios: jasmine.createSpy('getUsuarios').and.returnValue(of([])),
      getColumns: jasmine.createSpy('getColumns').and.returnValue(of([])),
      getTasksByColumn: jasmine.createSpy('getTasksByColumn').and.returnValue(of([])),
      startConnection: jasmine.createSpy('startConnection'),
      stopConnection: jasmine.createSpy('stopConnection'),
      boardUpdated$: of()
    };

    await TestBed.configureTestingModule({
      imports: [BoardComponent],
      providers: [
        { provide: KanbanService, useValue: kanbanServiceMock },
        { provide: ActivatedRoute, useValue: { paramMap: of({ get: () => '1' }) } },
        MessageService,
        ConfirmationService
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(BoardComponent);
    component = fixture.componentInstance;
  });

  it('debe filtrar las tareas por texto de búsqueda en título y descripción', () => {
    const tareas: Task[] = [
      { id: 1, titulo: 'Fix bug en login', descripcion: 'Revisar interceptor', prioridad: 'Alta', responsableId: 1, columnaId: 1, orden: 1, fechaCreacion: new Date() },
      { id: 2, titulo: 'Diseñar reporte PDF', descripcion: 'Usar QuestPDF', prioridad: 'Media', responsableId: 2, columnaId: 1, orden: 2, fechaCreacion: new Date() }
    ];

    component.searchText = 'QuestPDF';
    const resultado = component.getFilteredTasks(tareas);

    expect(resultado.length).toBe(1);
    expect(resultado[0].titulo).toBe('Diseñar reporte PDF');
  });

  it('debe filtrar las tareas por prioridad seleccionada', () => {
    const tareas: Task[] = [
      { id: 1, titulo: 'Fix bug', descripcion: 'Desc 1', prioridad: 'Alta', columnaId: 1, orden: 1, fechaCreacion: new Date() },
      { id: 2, titulo: 'Refactor', descripcion: 'Desc 2', prioridad: 'Baja', columnaId: 1, orden: 2, fechaCreacion: new Date() }
    ];

    component.selectedPriority = 'Alta';
    const resultado = component.getFilteredTasks(tareas);

    expect(resultado.length).toBe(1);
    expect(resultado[0].titulo).toBe('Fix bug');
  });
});
