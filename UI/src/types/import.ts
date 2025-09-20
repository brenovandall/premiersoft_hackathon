export type DataType = "hospitals" | "doctors" | "patients" | "locations" | "cid10" | "municipios" | "estados" | "medicos" | "hospitais" | "pacientes";
export type FileFormat = "xml" | "csv" | "xlsx" | "xls";
export type ImportStatus = "processing" | "success" | "warning" | "error";

export interface ImportRecord {
  id: string;
  fileName: string;
  dataType: DataType;
  fileFormat: FileFormat;
  uploadDate: Date;
  status: ImportStatus;
  summary: string;
  totalRecords?: number;
  successfulRecords?: number;
  errorRecords?: number;
  duplicateRecords?: number;
  description?: string;
  errors?: ImportError[];
}

export interface ImportError {
  line: number;
  field: string;
  message: string;
  data?: string;
}

// Estruturas de dados esperadas para cada tipo de tabela
export interface MunicipiosSchema {
  codigo_ibge: string;
  nome: string;
  latitude: string;
  longitude: string;
  capital: string;
  codigo_uf: string;
  siafi_id: string;
  ddd: string;
  fuso_horario: string;
  populacao: string;
}

export interface MedicosSchema {
  codigo: string;
  nome_completo: string;
  especialidade: string;
  cidade: string;
}

export interface HospitaisSchema {
  codigo: string;
  nome: string;
  cidade: string;
  bairro: string;
  especialidades: string;
  leitos_totais: string;
}

export interface EstadosSchema {
  codigo_uf: string;
  uf: string;
  nome: string;
  latitude: string;
  longitude: string;
  regiao: string;
}

export interface Cid10Schema {
  cid10: string;
}

export interface PacientesSchema {
  codigo: string;
  cpf: string;
  nome_completo: string;
  genero: string;
  codigo_municipio: string;
  bairro: string;
  convenio: string;
  cid10: string;
}

// Union type para todos os schemas
export type TableSchema = MunicipiosSchema | MedicosSchema | HospitaisSchema | EstadosSchema | Cid10Schema | PacientesSchema;

// Tipo para mapeamento de campos
export interface FieldMapping {
  sourceField: string;
  targetField: string;
  dataType: DataType;
}

// Tipo para dados do mapeamento - formato simplificado: {campo_origem: 'campo_destino'}
export type FieldMappingBackend = Record<string, string>;

// Interface para dados que serão enviados ao backend após upload
export interface BackendProcessingData {
  fileUrl: string;
  fileName: string;
  dataType: DataType;
  fileFormat: FileFormat;
  fieldMappings: FieldMappingBackend;
  fileSize: number;
  bucketName: string;
  s3Key: string;
}

// Interface para dados do cabeçalho do arquivo
export interface FileHeader {
  fields: string[];
  rowCount?: number;
  sampleData?: any[];
}

// Configuração de mapeamento para cada tipo de dados
export const TABLE_SCHEMAS: Record<DataType, { label: string; fields: string[]; description: string }> = {
  municipios: {
    label: "Municípios",
    fields: ["codigo_ibge", "nome", "latitude", "longitude", "capital", "codigo_uf", "siafi_id", "ddd", "fuso_horario", "populacao"],
    description: "Dados dos municípios brasileiros"
  },
  medicos: {
    label: "Médicos",
    fields: ["codigo", "nome_completo", "especialidade", "cidade"],
    description: "Cadastro de médicos"
  },
  hospitais: {
    label: "Hospitais",
    fields: ["codigo", "nome", "cidade", "bairro", "especialidades", "leitos_totais"],
    description: "Cadastro de hospitais"
  },
  estados: {
    label: "Estados",
    fields: ["codigo_uf", "uf", "nome", "latitude", "longitude", "regiao"],
    description: "Estados brasileiros"
  },
  cid10: {
    label: "CID-10",
    fields: ["cid10"],
    description: "Códigos da Classificação Internacional de Doenças"
  },
  pacientes: {
    label: "Pacientes",
    fields: ["codigo", "cpf", "nome_completo", "genero", "codigo_municipio", "bairro", "convenio", "cid10"],
    description: "Cadastro de pacientes"
  },
  // Mantendo compatibilidade com os tipos existentes
  hospitals: {
    label: "Hospitais",
    fields: ["codigo", "nome", "cidade", "bairro", "especialidades", "leitos_totais"],
    description: "Cadastro de hospitais"
  },
  doctors: {
    label: "Médicos",
    fields: ["codigo", "nome_completo", "especialidade", "cidade"],
    description: "Cadastro de médicos"
  },
  patients: {
    label: "Pacientes",
    fields: ["codigo", "cpf", "nome_completo", "genero", "codigo_municipio", "bairro", "convenio", "cid10"],
    description: "Cadastro de pacientes"
  },
  locations: {
    label: "Localidades",
    fields: ["codigo_ibge", "nome", "latitude", "longitude", "capital", "codigo_uf"],
    description: "Dados de localidades"
  }
};