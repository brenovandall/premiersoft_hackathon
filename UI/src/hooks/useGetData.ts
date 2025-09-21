import { useState, useEffect, useCallback } from 'react';
import { GetDataService, GetDataApiError } from '@/services/getDataService';
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
  PatientStatsDto
} from '@/types/getdata';

interface UseDataState<T> {
  data: T[];
  loading: boolean;
  error: string | null;
  refresh: () => Promise<void>;
}

/**
 * Hook para gerenciar estados
 */
export function useEstados(): UseDataState<EstadoDto> {
  const [data, setData] = useState<EstadoDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const estados = await GetDataService.getEstados();
      setData(estados);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar estados';
      setError(errorMessage);
      console.error('Erro ao buscar estados:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para gerenciar municípios
 */
export function useMunicipios(codigoUf?: string): UseDataState<MunicipioDto> {
  const [data, setData] = useState<MunicipioDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const municipios = codigoUf 
        ? await GetDataService.getMunicipiosByEstado(codigoUf)
        : await GetDataService.getMunicipios();
      setData(municipios);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar municípios';
      setError(errorMessage);
      console.error('Erro ao buscar municípios:', err);
    } finally {
      setLoading(false);
    }
  }, [codigoUf]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para gerenciar pacientes
 */
export function usePacientes(): UseDataState<PacienteDto> {
  const [data, setData] = useState<PacienteDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const pacientes = await GetDataService.getPacientes();
      setData(pacientes);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar pacientes';
      setError(errorMessage);
      console.error('Erro ao buscar pacientes:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para gerenciar médicos
 */
export function useMedicos(): UseDataState<MedicoDto> {
  const [data, setData] = useState<MedicoDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const medicos = await GetDataService.getMedicos();
      setData(medicos);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar médicos';
      setError(errorMessage);
      console.error('Erro ao buscar médicos:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para gerenciar hospitais
 */
export function useHospitais(): UseDataState<HospitalDto> {
  const [data, setData] = useState<HospitalDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const hospitais = await GetDataService.getHospitais();
      setData(hospitais);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar hospitais';
      setError(errorMessage);
      console.error('Erro ao buscar hospitais:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para verificar saúde da API
 */
export function useHealthCheck() {
  const [isHealthy, setIsHealthy] = useState<boolean | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const checkHealth = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      await GetDataService.healthCheck();
      setIsHealthy(true);
    } catch (err) {
      setIsHealthy(false);
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'API não está respondendo';
      setError(errorMessage);
      console.error('Erro no health check:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    checkHealth();
  }, [checkHealth]);

  return {
    isHealthy,
    loading,
    error,
    checkHealth,
  };
}

/**
 * Hook para buscar lista de hospitais
 */
export function useHospitalsList(): UseDataState<HospitalListDto> {
  const [data, setData] = useState<HospitalListDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const hospitais = await GetDataService.getHospitalsList();
      setData(hospitais);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar lista de hospitais';
      setError(errorMessage);
      console.error('Erro ao buscar lista de hospitais:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para buscar detalhes de um hospital específico
 */
export function useHospitalDetails(hospitalId: number) {
  const [data, setData] = useState<HospitalDetailsDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    if (!hospitalId) return;
    
    setLoading(true);
    setError(null);
    try {
      const hospital = await GetDataService.getHospitalDetails(hospitalId);
      setData(hospital);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar detalhes do hospital';
      setError(errorMessage);
      console.error('Erro ao buscar detalhes do hospital:', err);
    } finally {
      setLoading(false);
    }
  }, [hospitalId]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para buscar especialidades de um hospital específico
 */
export function useHospitalSpecialties(hospitalId: number): UseDataState<HospitalSpecialtyDto> {
  const [data, setData] = useState<HospitalSpecialtyDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    if (!hospitalId) return;
    
    setLoading(true);
    setError(null);
    try {
      const especialidades = await GetDataService.getHospitalSpecialties(hospitalId);
      setData(especialidades);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar especialidades do hospital';
      setError(errorMessage);
      console.error('Erro ao buscar especialidades do hospital:', err);
    } finally {
      setLoading(false);
    }
  }, [hospitalId]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para gerenciar dados demográficos de pacientes
 */
export function usePatientDemographics(): UseDataState<PatientDemographicDto> {
  const [data, setData] = useState<PatientDemographicDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const demographics = await GetDataService.getPatientDemographics();
      setData(demographics);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar dados demográficos de pacientes';
      setError(errorMessage);
      console.error('Erro ao buscar dados demográficos de pacientes:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para gerenciar estatísticas de pacientes
 */
export function usePatientStats() {
  const [data, setData] = useState<PatientStatsDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const stats = await GetDataService.getPatientStats();
      setData(stats);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar estatísticas de pacientes';
      setError(errorMessage);
      console.error('Erro ao buscar estatísticas de pacientes:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para gerenciar estatísticas de médicos por especialidade
 */
export function useDoctorSpecialtyStats(): UseDataState<DoctorSpecialtyStatsDto> {
  const [data, setData] = useState<DoctorSpecialtyStatsDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const stats = await GetDataService.getDoctorSpecialtyStats();
      setData(stats);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao carregar estatísticas de médicos';
      setError(errorMessage);
      console.error('Erro ao buscar estatísticas de médicos:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

/**
 * Hook para busca de médicos
 */
export function useDoctorSearch(searchTerm?: string, specialty?: string) {
  const [data, setData] = useState<DoctorSearchDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const doctors = await GetDataService.searchDoctors(searchTerm, specialty);
      setData(doctors);
    } catch (err) {
      const errorMessage = err instanceof GetDataApiError 
        ? err.message 
        : 'Erro ao buscar médicos';
      setError(errorMessage);
      console.error('Erro ao buscar médicos:', err);
    } finally {
      setLoading(false);
    }
  }, [searchTerm, specialty]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}