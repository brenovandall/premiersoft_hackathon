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

export interface HospitalListDto {
  id: number;
  nome: string;
  cidade: string;
  uf: string;
}

export interface HospitalDetailsDto {
  id: number;
  nome: string;
  cidade: string;
  estado: string;
  uf: string;
  leitosTotal: number;
  leitosOcupados: number;
  medicosAlocados: number;
  taxaOcupacao: number;
  rankingRegional: number;
  tipoHospital: string;
}

export interface HospitalSpecialtyDto {
  especialidade: string;
  numeroMedicos: number;
  numeroPacientes: number;
}

export interface PatientDemographicDto {
  ageGroup: string;
  male: number;
  female: number;
  total: number;
}

export interface DoctorSpecialtyStatsDto {
  specialty: string;
  doctors: number;
  patients: number;
  doctorPatientRatio: number;
}

export interface DoctorSearchDto {
  id: number;
  name: string;
  specialty: string;
  hospitals: string[];
  city: string;
  state: string;
}

export interface PatientStatsDto {
  totalPatients: number;
  malePatients: number;
  femalePatients: number;
  malePercentage: number;
  femalePercentage: number;
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