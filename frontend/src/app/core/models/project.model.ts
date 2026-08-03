export interface Project {
  id: number;
  nombre: string;
  descripcion: string;
  fechaInicio: Date | string;
  fechaFinPrevista?: Date | string;
  estado: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalRecords: number;
  page: number;
  pageSize: number;
}
