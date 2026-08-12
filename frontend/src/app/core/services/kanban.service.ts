import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Subject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Column } from '../models/column.model';
import { Task } from '../models/task.model';
import * as signalR from '@microsoft/signalr';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class KanbanService {
  private apiUrl = `${environment.apiUrl}`;
  private hubConnection: signalR.HubConnection | undefined;
  
  public boardUpdated$ = new Subject<void>();
  public connectedUsers$ = new BehaviorSubject<number>(1);

  constructor(private http: HttpClient, private authService: AuthService) { }

  // --- HTTP API Calls ---

  getColumns(projectId: number): Observable<Column[]> {
    return this.http.get<Column[]>(`${this.apiUrl}/Columnas/proyecto/${projectId}`);
  }

  getUsuarios(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Usuarios`);
  }
  
  createColumn(column: Partial<Column>): Observable<Column> {
    return this.http.post<Column>(`${this.apiUrl}/Columnas`, column);
  }

  updateColumn(id: number, nombre: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Columnas/${id}`, `"${nombre}"`, { headers: { 'Content-Type': 'application/json' } });
  }

  deleteColumn(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Columnas/${id}`);
  }

  updateColumnOrder(columnId: number, newOrder: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Columnas/mover`, {
      columnaId: columnId,
      nuevoOrden: newOrder
    });
  }

  getTasksByColumn(columnId: number): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/Tareas/columna/${columnId}`);
  }

  createTask(task: Partial<Task>): Observable<Task> {
    return this.http.post<Task>(`${this.apiUrl}/Tareas`, task);
  }

  updateTask(id: number, task: Partial<Task>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Tareas/${id}`, task);
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Tareas/${id}`);
  }

  updateTaskColumn(taskId: number, newColumnId: number, newOrder: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Tareas/mover`, {
      tareaId: taskId,
      nuevaColumnaId: newColumnId,
      nuevoOrden: newOrder
    });
  }

  exportBoardToPdf(projectId: number, prioridad?: string | null, responsableId?: number | null, texto?: string | null): Observable<Blob> {
    let params = `format=pdf`;
    if (prioridad) params += `&prioridad=${encodeURIComponent(prioridad)}`;
    if (responsableId) params += `&responsableId=${responsableId}`;
    if (texto) params += `&texto=${encodeURIComponent(texto)}`;
    return this.http.get(`${this.apiUrl}/Reportes/${projectId}?${params}`, { responseType: 'blob' });
  }

  exportBoardToExcel(projectId: number, prioridad?: string | null, responsableId?: number | null, texto?: string | null): Observable<Blob> {
    let params = `format=excel`;
    if (prioridad) params += `&prioridad=${encodeURIComponent(prioridad)}`;
    if (responsableId) params += `&responsableId=${responsableId}`;
    if (texto) params += `&texto=${encodeURIComponent(texto)}`;
    return this.http.get(`${this.apiUrl}/Reportes/${projectId}?${params}`, { responseType: 'blob' });
  }

  // --- SignalR Real-Time ---

  public startConnection(projectId: number): void {
    const token = this.authService.currentUserValue?.token;
    const hubUrl = this.apiUrl.replace(/\/api\/?$/, '') + '/hubs/kanban';
    
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => token || ''
      })
      .configureLogging(signalR.LogLevel.Warning)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.hubConnection?.invoke('JoinBoardGroup', projectId.toString());
      })
      .catch(err => console.error('Error while starting connection: ', err));

    this.hubConnection.on('BoardUpdated', () => {
      this.boardUpdated$.next();
    });

    this.hubConnection.on('ConnectedUsersChanged', (count: number) => {
      this.connectedUsers$.next(count);
    });
  }

  public stopConnection(projectId: number): void {
    if (this.hubConnection) {
      this.hubConnection.invoke('LeaveBoardGroup', projectId.toString())
        .then(() => this.hubConnection?.stop())
        .catch(err => console.error(err));
    }
  }
}
