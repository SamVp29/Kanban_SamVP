import { Task } from './task.model';

export interface Column {
  id: number;
  nombre: string;
  proyectoId: number;
  orden: number;
  tareas?: Task[]; // Local property to hold tasks for UI
}
