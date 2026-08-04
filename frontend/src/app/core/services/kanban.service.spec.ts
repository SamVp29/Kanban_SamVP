import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { KanbanService } from './kanban.service';
import { AuthService } from './auth.service';

describe('KanbanService', () => {
  let service: KanbanService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    const authServiceMock = { currentUserValue: { token: 'mock-token' } };

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        KanbanService,
        { provide: AuthService, useValue: authServiceMock }
      ]
    });

    service = TestBed.inject(KanbanService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debe enviar una petición PUT a /Tareas/mover al reordenar o mover una tarea', () => {
    const tareaId = 5;
    const nuevaColumnaId = 2;
    const nuevoOrden = 196608;

    service.updateTaskColumn(tareaId, nuevaColumnaId, nuevoOrden).subscribe();

    const req = httpMock.expectOne(req => req.url.endsWith('/Tareas/mover'));
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({
      tareaId: tareaId,
      nuevaColumnaId: nuevaColumnaId,
      nuevoOrden: nuevoOrden
    });

    req.flush(null);
  });
});
