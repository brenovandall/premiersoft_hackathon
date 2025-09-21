import { FieldMapping, FieldMappingBackend, BackendProcessingData, DataType, FileFormat } from '@/types/import';

/**
 * Transforma os field mappings do formato interno para o formato esperado pelo backend
 * Formato: [{ "campo_origem": "campo_destino" }]
 */
export function transformFieldMappings(fieldMappings: FieldMapping[]): FieldMappingBackend[] {
  return fieldMappings
    .filter(mapping => mapping.sourceField && mapping.targetField)
    .map(mapping => ({
      [mapping.sourceField]: mapping.targetField
    }));
}

/**
 * Cria o objeto completo para envio ao backend
 */
export function createBackendProcessingData(
  fileUrl: string,
  presignedUrl: string,
  fileName: string,
  dataType: DataType,
  fileFormat: FileFormat,
  fieldMappings: FieldMapping[],
  fileSize: number,
  bucketName: string,
  s3Key: string
): BackendProcessingData {
  return {
    fileUrl: presignedUrl || fileUrl, // Preferir URL pré-assinada
    fileName,
    dataType,
    fileFormat,
    fieldMappings: transformFieldMappings(fieldMappings),
    fileSize,
    bucketName,
    s3Key
  };
}

/**
 * Valida se os dados estão prontos para envio ao backend
 */
export function validateBackendData(data: BackendProcessingData): { isValid: boolean; errors: string[] } {
  const errors: string[] = [];

  if (!data.fileUrl) {
    errors.push('URL do arquivo é obrigatória');
  }

  if (!data.fileName) {
    errors.push('Nome do arquivo é obrigatório');
  }

  if (!data.dataType) {
    errors.push('Tipo de dados é obrigatório');
  }

  if (!data.fileFormat) {
    errors.push('Formato do arquivo é obrigatório');
  }

  if (!data.s3Key) {
    errors.push('Chave S3 é obrigatória');
  }

  if (!data.bucketName) {
    errors.push('Nome do bucket é obrigatório');
  }

  if (data.fileSize <= 0) {
    errors.push('Tamanho do arquivo deve ser maior que zero');
  }

  // Verificar se há pelo menos um mapeamento válido (para arquivos que suportam mapeamento)
  if (data.fieldMappings.length === 0) {
    console.warn('Nenhum mapeamento de campo definido - o backend processará com campos padrão');
  }

  return {
    isValid: errors.length === 0,
    errors
  };
}