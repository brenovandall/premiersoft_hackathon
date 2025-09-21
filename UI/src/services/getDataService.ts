import { 
  EstadoDto, 
  MunicipioDto, 
  PacienteDto, 
  MedicoDto, 
  HospitalDto,
  HospitalListDto,
  HospitalDetailsDto,
  HospitalSpecialtyDto,
  PatientDemographicDto,
  DoctorSpecialtyStatsDto,
  DoctorSearchDto,
  PatientStatsDto,
  HealthResponse 
} from '@/types/getdata';// Configuração da API GetData
const GETDATA_API_CONFIG = {
  baseUrl: import.meta.env.VITE_API_BASE_URL || 'http://localhost:6000/v1/ImportFiles',
  timeout: 30000, // 30 segundos
};

/**
 * Classe de erro personalizada para a API
 */
export class GetDataApiError extends Error {
  constructor(
    public status: number,
    message: string,
    public details?: any
  ) {
    super(message);
    this.name = 'GetDataApiError';
  }
}

/**
 * Função helper para fazer requisições HTTP
 */
async function apiRequest<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  const url = `${GETDATA_API_CONFIG.baseUrl}${endpoint}`;
  
  try {
    const response = await fetch(url, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
    });

    if (!response.ok) {
      let errorMessage = `HTTP ${response.status}: ${response.statusText}`;
      let errorDetails = null;

      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorMessage;
        errorDetails = errorData;
      } catch {
        // Ignore JSON parsing errors for error responses
      }

      throw new GetDataApiError(response.status, errorMessage, errorDetails);
    }

    return await response.json();
  } catch (error) {
    if (error instanceof GetDataApiError) {
      throw error;
    }
    
    // Network or other errors
    throw new GetDataApiError(0, `Erro de conexão: ${error instanceof Error ? error.message : 'Erro desconhecido'}`);
  }
}

/**
 * Serviço para consumir a API GetData
 */
export class GetDataService {
  
  /**
   * Verifica se a API está funcionando
   */
  static async healthCheck(): Promise<HealthResponse> {
    return apiRequest<HealthResponse>('/health');
  }

  /**
   * Busca todos os estados
   */
  static async getEstados(): Promise<EstadoDto[]> {
    return apiRequest<EstadoDto[]>('/estados');
  }

  /**
   * Busca todos os municípios
   */
  static async getMunicipios(): Promise<MunicipioDto[]> {
    return apiRequest<MunicipioDto[]>('/municipios');
  }

  /**
   * Busca municípios por estado
   */
  static async getMunicipiosByEstado(codigoUf: string): Promise<MunicipioDto[]> {
    return apiRequest<MunicipioDto[]>(`/municipios/estado/${encodeURIComponent(codigoUf)}`);
  }

  /**
   * Busca todos os pacientes
   */
  static async getPacientes(): Promise<PacienteDto[]> {
    return apiRequest<PacienteDto[]>('/pacientes');
  }

  /**
   * Busca todos os médicos
   */
  static async getMedicos(): Promise<MedicoDto[]> {
    return apiRequest<MedicoDto[]>('/medicos');
  }

  /**
   * Busca todos os hospitais
   */
  static async getHospitais(): Promise<HospitalDto[]> {
    return apiRequest<HospitalDto[]>('/hospitais');
  }

  /**
   * Busca lista de hospitais para seleção
   */
  static async getHospitalsList(): Promise<HospitalListDto[]> {
    return apiRequest<HospitalListDto[]>('/hospitais/list');
  }

  /**
   * Busca detalhes específicos de um hospital
   */
  static async getHospitalDetails(hospitalId: number): Promise<HospitalDetailsDto> {
    return apiRequest<HospitalDetailsDto>(`/hospitais/${hospitalId}/details`);
  }

  /**
   * Busca especialidades médicas de um hospital específico
   */
  static async getHospitalSpecialties(hospitalId: number): Promise<HospitalSpecialtyDto[]> {
    return apiRequest<HospitalSpecialtyDto[]>(`/hospitais/${hospitalId}/especialidades`);
  }

  /**
   * Busca dados demográficos de pacientes
   */
  static async getPatientDemographics(): Promise<PatientDemographicDto[]> {
    return apiRequest<PatientDemographicDto[]>('/pacientes/demographics');
  }

  /**
   * Busca estatísticas gerais de pacientes
   */
  static async getPatientStats(): Promise<PatientStatsDto> {
    return apiRequest<PatientStatsDto>('/pacientes/stats');
  }

  /**
   * Busca estatísticas de médicos por especialidade
   */
  static async getDoctorSpecialtyStats(): Promise<DoctorSpecialtyStatsDto[]> {
    return apiRequest<DoctorSpecialtyStatsDto[]>('/medicos/specialty-stats');
  }

  /**
   * Busca médicos por nome e especialidade
   */
  static async searchDoctors(searchTerm?: string, specialty?: string): Promise<DoctorSearchDto[]> {
    const params = new URLSearchParams();
    if (searchTerm) params.append('searchTerm', searchTerm);
    if (specialty) params.append('specialty', specialty);
    
    const queryString = params.toString();
    const endpoint = queryString ? `/medicos/search?${queryString}` : '/medicos/search';
    
    return apiRequest<DoctorSearchDto[]>(endpoint);
  }
}

/**
 * Hook personalizado para facilitar o uso do serviço com React
 */
export const useGetDataService = () => {
  return {
    healthCheck: GetDataService.healthCheck,
    getEstados: GetDataService.getEstados,
    getMunicipios: GetDataService.getMunicipios,
    getMunicipiosByEstado: GetDataService.getMunicipiosByEstado,
    getPacientes: GetDataService.getPacientes,
    getMedicos: GetDataService.getMedicos,
    getHospitais: GetDataService.getHospitais,
    getHospitalsList: GetDataService.getHospitalsList,
    getHospitalDetails: GetDataService.getHospitalDetails,
    getHospitalSpecialties: GetDataService.getHospitalSpecialties,
    getPatientDemographics: GetDataService.getPatientDemographics,
    getPatientStats: GetDataService.getPatientStats,
    getDoctorSpecialtyStats: GetDataService.getDoctorSpecialtyStats,
    searchDoctors: GetDataService.searchDoctors,
  };
};