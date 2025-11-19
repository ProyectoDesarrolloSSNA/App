
export interface CityDto {
  name?: string;
  country?: string;
  latitude: number;
  longitude: number;
}

export interface CreateUpdateDestinoDto {
  nombre: string;
  pais: string;
  descripcion?: string;
}

export interface DestinoDto {
  id?: string;
  nombre?: string;
  pais?: string;
  descripcion?: string;
}
