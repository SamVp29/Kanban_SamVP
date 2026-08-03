export interface Task {
  id: number;
  titulo: string;
  descripcion: string;
  prioridad: string;
  columnaId: number;
  responsableId?: number;
  orden: number;
  fechaCreacion: Date;
}
