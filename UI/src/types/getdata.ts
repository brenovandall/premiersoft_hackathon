// Tipos para os dados da API GetData

export interface EstadoDto {
  id: string;
  codigo_uf: number;
  uf: string;
  nome: string;
  latitude: number;
  longitude: number;
  regiao: string;
}

export interface MunicipioDto {
  id: string;
  codigo_ibge: string;
  nome: string;
  latitude: number;
  longitude: number;
  capital: boolean;
  codigo_uf: string;
  siafi_id: number;
  ddd: number;
  fuso_horario: string;
  populacao: number;
  erros: string;
}

export interface PacienteDto {
  id: string;
  codigo: string;
  cpf: string;
  genero: string;
  nome_completo: string;
  convenio: boolean;
  municipio: string;
  cid10: string;
  descricaoCid10: string;
}

export interface MedicoDto {
  id: string;
  codigo: string;
  nome_completo: string;
  especialidade: string;
  municipio: string;
}

export interface HospitalDto {
  id: string;
  codigo: string;
  nome: string;
  bairro: string;
  cidade: string;
  leitos_totais: number;
}

export interface HealthResponse {
  status: string;
  timestamp: string;
}

// Tipos para filtros e parâmetros
export interface ApiFilters {
  page?: number;
  pageSize?: number;
  search?: string;
}

export interface EstadoFilters extends ApiFilters {
  regiao?: string;
}

export interface MunicipioFilters extends ApiFilters {
  codigo_uf?: string;
  capital?: boolean;
}

export interface PacienteFilters extends ApiFilters {
  genero?: string;
  convenio?: boolean;
  municipio?: string;
}

export interface MedicoFilters extends ApiFilters {
  especialidade?: string;
  municipio?: string;
}

export interface HospitalFilters extends ApiFilters {
  cidade?: string;
}