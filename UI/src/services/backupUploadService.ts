/**
 * Serviço específico para uploads de backup
 * Função separada para uso em casos de backup de dados
 */

export interface BackupUploadRequest {
  dataType: number;
  fileFormat: number;
  fileName: string;
  fileContent: string; // Conteúdo do arquivo em base64
}

export interface BackupUploadResponse {
  success: boolean;
  message: string;
  processedRecords?: number;
  successfulRecords?: number;
  errorRecords?: number;
  errors?: string[];
}

// Configuração da API para backup
const BACKUP_API_CONFIG = {
  baseUrl: 'http://localhost:3001', // Porta da API .NET conforme launchSettings.json
  timeout: 60000, // 60 segundos para uploads de backup
};

/**
 * Faz upload de arquivo de backup diretamente para o banco de dados
 * Esta é uma função separada para uso exclusivo em backups
 * Processa o arquivo e salva diretamente no banco, sem passar pela tabela Import
 */
export async function uploadBackupFile(file: File, request: BackupUploadRequest): Promise<BackupUploadResponse> {
  try {
    console.log('📦 Iniciando importação direta de backup:', request);

    // Converter arquivo para base64
    const fileContent = await fileToBase64(file);
    
    // Determinar endpoint baseado no tipo de dados
    let endpoint = '';
    switch (request.dataType) {
      case 3: // Estado
        endpoint = '/v1/DirectImport/estados';
        break;
      case 4: // Cidade/Município
        endpoint = '/v1/DirectImport/municipios';
        break;
      case 5: // CID-10
        endpoint = '/v1/DirectImport/cid10';
        break;
      default:
        throw new Error(`Tipo de dados ${request.dataType} não suportado para importação direta`);
    }

    const directRequest = {
      dataType: request.dataType,
      fileFormat: request.fileFormat,
      fileName: request.fileName,
      fileContent: fileContent
    };

    const response = await fetch(`${BACKUP_API_CONFIG.baseUrl}${endpoint}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      body: JSON.stringify(directRequest),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Erro HTTP ${response.status}: ${errorText}`);
    }

    const result = await response.json();
    
    console.log('✅ Importação direta concluída:', result);

    return {
      success: result.success,
      message: result.message,
      processedRecords: result.processedRecords,
      successfulRecords: result.successfulRecords,
      errorRecords: result.errorRecords,
      errors: result.errors
    };

  } catch (error) {
    console.error('❌ Erro na importação direta:', error);
    
    return {
      success: false,
      message: error instanceof Error ? error.message : 'Erro desconhecido na importação direta'
    };
  }
}

/**
 * Converte arquivo para base64
 */
function fileToBase64(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => {
      const result = reader.result as string;
      // Remover o prefixo "data:text/csv;base64," ou similar
      const base64 = result.split(',')[1];
      resolve(base64);
    };
    reader.onerror = error => reject(error);
  });
}

/**
 * Valida se o arquivo é válido para importação direta
 */
export function validateBackupFile(file: File): { valid: boolean; error?: string } {
  const maxSize = 10 * 1024 * 1024; // 10MB para importação direta
  const allowedTypes = [
    'text/csv',
    'application/vnd.ms-excel',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
  ];

  if (file.size > maxSize) {
    return {
      valid: false,
      error: 'Arquivo muito grande. Tamanho máximo permitido: 10MB'
    };
  }

  if (!allowedTypes.includes(file.type) && !file.name.match(/\.(csv|xlsx|xls)$/i)) {
    return {
      valid: false,
      error: 'Tipo de arquivo não suportado para importação direta. Use: CSV ou Excel'
    };
  }

  return { valid: true };
}

/**
 * Mapeia os tipos de dados suportados para importação direta
 */
export const DATA_TYPE_LABELS = {
  3: 'Estado',
  4: 'Cidade/Município',
  5: 'Tabela CID-10'
} as const;

/**
 * Mapeia os formatos de arquivo suportados para importação direta
 */
export const FILE_FORMAT_LABELS = {
  1: 'CSV',
  0: 'Excel'
} as const;