import { useState, useEffect, useCallback } from 'react';
import { GetDataService, GetDataApiError } from '@/services/getDataService';
import { 
  EstadoDto, 
  MunicipioDto, 
  PacienteDto, 
  MedicoDto, 
  HospitalDto 
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