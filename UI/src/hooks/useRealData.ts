import { useMemo } from 'react';
import { 
  useEstados, 
  useMunicipios, 
  usePacientes, 
  useMedicos, 
  useHospitais 
} from '@/hooks/useGetData';

export interface KPIData {
  totalPatients: number;
  totalHospitals: number;
  totalDoctors: number;
  totalStates: number;
  totalMunicipalities: number;
  averageBedsPerHospital: number;
  patientsWithInsurance: number;
  patientsWithoutInsurance: number;
  loading: boolean;
  error: string | null;
}

export interface DiseaseData {
  name: string;
  value: number;
  code: string;
}

export interface SpecialtyData {
  specialty: string;
  doctors: number;
  patients: number;
}

export interface RegionData {
  regiao: string;
  estados: number;
  municipios: number;
  pacientes: number;
  medicos: number;
  hospitais: number;
}

export interface StateData {
  estado: string;
  uf: string;
  municipios: number;
  pacientes: number;
  medicos: number;
  hospitais: number;
}

export function useRealDataKPIs(): KPIData {
  const { data: estados, loading: estadosLoading, error: estadosError } = useEstados();
  const { data: municipios, loading: municipiosLoading, error: municipiosError } = useMunicipios();
  const { data: pacientes, loading: pacientesLoading, error: pacientesError } = usePacientes();
  const { data: medicos, loading: medicosLoading, error: medicosError } = useMedicos();
  const { data: hospitais, loading: hospitaisLoading, error: hospitaisError } = useHospitais();

  const loading = estadosLoading || municipiosLoading || pacientesLoading || medicosLoading || hospitaisLoading;
  const error = estadosError || municipiosError || pacientesError || medicosError || hospitaisError;

  const kpiData = useMemo((): KPIData => {
    const totalBedsInHospitals = hospitais.reduce((sum, hospital) => sum + hospital.leitos_totais, 0);
    const averageBedsPerHospital = hospitais.length > 0 ? Math.round(totalBedsInHospitals / hospitais.length) : 0;
    const patientsWithInsurance = pacientes.filter(p => p.convenio).length;
    const patientsWithoutInsurance = pacientes.length - patientsWithInsurance;

    return {
      totalPatients: pacientes.length,
      totalHospitals: hospitais.length,
      totalDoctors: medicos.length,
      totalStates: estados.length,
      totalMunicipalities: municipios.length,
      averageBedsPerHospital,
      patientsWithInsurance,
      patientsWithoutInsurance,
      loading,
      error,
    };
  }, [estados, municipios, pacientes, medicos, hospitais, loading, error]);

  return kpiData;
}

export function useRealDiseaseData(): DiseaseData[] {
  const { data: pacientes } = usePacientes();

  return useMemo(() => {
    const diseaseCount = pacientes.reduce((acc, paciente) => {
      const key = `${paciente.cid10}|${paciente.descricaoCid10}`;
      acc[key] = (acc[key] || 0) + 1;
      return acc;
    }, {} as Record<string, number>);

    return Object.entries(diseaseCount)
      .map(([key, value]) => {
        const [code, name] = key.split('|');
        return {
          name: name || 'Descrição não disponível',
          value,
          code,
        };
      })
      .sort((a, b) => b.value - a.value)
      .slice(0, 8); // Top 8 doenças
  }, [pacientes]);
}

export function useRealSpecialtyData(): SpecialtyData[] {
  const { data: medicos } = useMedicos();
  const { data: pacientes } = usePacientes();

  return useMemo(() => {
    const specialtyCount = medicos.reduce((acc, medico) => {
      acc[medico.especialidade] = (acc[medico.especialidade] || 0) + 1;
      return acc;
    }, {} as Record<string, number>);

    // Para este exemplo, vamos assumir uma distribuição de pacientes por especialidade
    // Em um cenário real, você teria essa relação no banco de dados
    return Object.entries(specialtyCount)
      .map(([specialty, doctors]) => ({
        specialty,
        doctors,
        patients: Math.round(doctors * 25), // Assumindo 25 pacientes por médico em média
      }))
      .sort((a, b) => b.doctors - a.doctors)
      .slice(0, 8);
  }, [medicos, pacientes]);
}

export function useRealRegionData(): RegionData[] {
  const { data: estados } = useEstados();
  const { data: municipios } = useMunicipios();
  const { data: pacientes } = usePacientes();
  const { data: medicos } = useMedicos();
  const { data: hospitais } = useHospitais();

  return useMemo(() => {
    const regionData = estados.reduce((acc, estado) => {
      const regiao = estado.regiao;
      if (!acc[regiao]) {
        acc[regiao] = {
          regiao,
          estados: 0,
          municipios: 0,
          pacientes: 0,
          medicos: 0,
          hospitais: 0,
        };
      }
      acc[regiao].estados += 1;
      return acc;
    }, {} as Record<string, RegionData>);

    // Contar municípios por região (baseado no código UF)
    municipios.forEach(municipio => {
      const estado = estados.find(e => e.codigo_uf.toString() === municipio.codigo_uf);
      if (estado && regionData[estado.regiao]) {
        regionData[estado.regiao].municipios += 1;
      }
    });

    // Para pacientes, médicos e hospitais, seria necessário ter a relação com UF
    // Por enquanto, vamos fazer uma distribuição estimada baseada no número de municípios
    Object.values(regionData).forEach(region => {
      const municipiosPercentage = region.municipios / municipios.length;
      region.pacientes = Math.round(pacientes.length * municipiosPercentage);
      region.medicos = Math.round(medicos.length * municipiosPercentage);
      region.hospitais = Math.round(hospitais.length * municipiosPercentage);
    });

    return Object.values(regionData).sort((a, b) => b.pacientes - a.pacientes);
  }, [estados, municipios, pacientes, medicos, hospitais]);
}

export function useRealStateData(): StateData[] {
  const { data: estados } = useEstados();
  const { data: municipios } = useMunicipios();
  const { data: pacientes } = usePacientes();
  const { data: medicos } = useMedicos();
  const { data: hospitais } = useHospitais();

  return useMemo(() => {
    return estados.map(estado => {
      const estadoMunicipios = municipios.filter(m => m.codigo_uf === estado.codigo_uf.toString());
      const municipiosCount = estadoMunicipios.length;
      
      // Estimativa baseada no número de municípios
      const municipiosPercentage = municipiosCount / municipios.length;
      
      return {
        estado: estado.nome,
        uf: estado.uf,
        municipios: municipiosCount,
        pacientes: Math.round(pacientes.length * municipiosPercentage),
        medicos: Math.round(medicos.length * municipiosPercentage),
        hospitais: Math.round(hospitais.length * municipiosPercentage),
      };
    }).sort((a, b) => b.pacientes - a.pacientes);
  }, [estados, municipios, pacientes, medicos, hospitais]);
}