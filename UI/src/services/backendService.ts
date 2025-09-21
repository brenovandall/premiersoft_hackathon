import { ImportFilesRequest, KeyPair, BackendProcessingData, DataType, FileFormat, ImportRecord, ImportStatus } from '@/types/import';

// Interfaces para resposta do backend
export interface BackendProcessingResponse {
  success: boolean;
  message: string;
  data?: {
    id: number;
    processingId: string;
    estimatedTime?: number;
    status: string;
  };
  error?: string;
}

export interface ImportStatusResponse {
  id: number;
  dataType: number;
  fileFormat: number;
  description?: string;
  fileName: string;
  s3PreSignedUrl: string;
  status: number;
  fieldMappings: KeyPair[];
  createdAt: string;
  updatedAt?: string;
}

// Configuração da API do backend
const API_CONFIG = {
  baseUrl: import.meta.env.VITE_API_BASE_URL || 'http://localhost:3001/v1',
  timeout: 30000, // 30 segundos
};

/**
 * Converte DataType para ImportDataTypes (enum do backend)
 */
function convertDataType(dataType: DataType): number {
  const mapping: Record<DataType, number> = {
    "municipios": 1,
    "estados": 2, 
    "medicos": 3,
    "hospitais": 4,
    "pacientes": 5,
    "cid10": 6,
    "hospitals": 4,
    "doctors": 3,
    "patients": 5,
    "locations": 1
  };
  return mapping[dataType] || 1;
}

/**
 * Converte FileFormat para ImportFileFormat (enum do backend)
 */
function convertFileFormat(fileFormat: FileFormat): number {
  const mapping: Record<FileFormat, number> = {
    "csv": 1,
    "xlsx": 2,
    "xls": 2,
    "xml": 4
  };
  return mapping[fileFormat] || 1;
}

/**
 * Converte FieldMappingBackend para KeyPair[]
 */
function convertFieldMappings(fieldMappings: Record<string, string>[]): KeyPair[] {
  return fieldMappings.flatMap(mapping => 
    Object.entries(mapping).map(([from, to]) => ({ from, to }))
  );
}

/**
 * Envia os dados do arquivo e mapeamentos para o backend processar
 */
export async function sendFileToBackend(data: BackendProcessingData): Promise<BackendProcessingResponse> {
  try {
    console.log('📤 Enviando dados para o backend via POST:');
    console.log('🌐 URL:', `${API_CONFIG.baseUrl}/ImportFiles`);
    console.log('📊 Dados recebidos:', data);

    // Validar dados antes de enviar
    if (!data.fileName || !data.fileUrl) {
      throw new Error('Nome do arquivo e URL do arquivo são obrigatórios');
    }

    // Converter BackendProcessingData para ImportFilesRequest
    const payload: ImportFilesRequest = {
      dataType: convertDataType(data.dataType),
      fileFormat: convertFileFormat(data.fileFormat),
      description: `Importação de ${data.dataType} - ${data.fileName}`,
      fileName: data.fileName,
      s3PreSignedUrl: data.s3Key,
      status: 1,
      fieldMappings: convertFieldMappings(data.fieldMappings)
    };

    console.log('📊 Payload convertido:', payload);

    const response = await fetch(`${API_CONFIG.baseUrl}/ImportFiles`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      body: JSON.stringify(payload),
      signal: AbortSignal.timeout(API_CONFIG.timeout),
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error('❌ Erro HTTP:', response.status, errorText);
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }

    // Verificar se há conteúdo na resposta antes de tentar fazer parse JSON
    let result = null;
    const contentLength = response.headers.get('content-length');
    const contentType = response.headers.get('content-type');
    
    if (response.status !== 204 && contentLength !== '0' && contentType?.includes('application/json')) {
      const responseText = await response.text();
      if (responseText.trim()) {
        try {
          result = JSON.parse(responseText);
        } catch (parseError) {
          console.warn('⚠️ Resposta não é JSON válido:', responseText);
          result = { message: responseText };
        }
      }
    }
    
    console.log('✅ Resposta do backend:', { status: response.status, data: result });
    
    return {
      success: true,
      message: 'Importação criada com sucesso',
      data: {
        id: result?.id || Date.now(),
        processingId: result?.id?.toString() || `import_${Date.now()}`,
        estimatedTime: 30,
        status: result?.status || 'pending'
      },
    };

  } catch (error) {
    console.error('❌ Erro ao enviar dados para o backend:', error);

    if (error instanceof Error) {
      if (error.name === 'TimeoutError') {
        return {
          success: false,
          message: 'Timeout na comunicação com o backend',
          error: 'A requisição demorou mais que o esperado para ser processada',
        };
      }

      if (error.message.includes('fetch')) {
        return {
          success: false,
          message: 'Erro de conectividade',
          error: 'Não foi possível conectar ao backend. Verifique se o serviço está rodando.',
        };
      }

      return {
        success: false,
        message: 'Erro no processamento',
        error: error.message,
      };
    }

    return {
      success: false,
      message: 'Erro desconhecido',
      error: 'Ocorreu um erro inesperado durante o processamento',
    };
  }
}

/**
 * Envia dados diretamente no formato ImportFilesRequest (para uso avançado)
 */
export async function sendImportFilesRequest(data: ImportFilesRequest): Promise<BackendProcessingResponse> {
  try {
    console.log('📤 Enviando ImportFilesRequest diretamente para o backend:');
    console.log('🌐 URL:', `${API_CONFIG.baseUrl}/ImportFiles`);
    console.log('📊 Dados:', data);

    // Validar dados antes de enviar
    if (!data.fileName || !data.s3PreSignedUrl) {
      throw new Error('Nome do arquivo e URL do S3 são obrigatórios');
    }

    const response = await fetch(`${API_CONFIG.baseUrl}/ImportFiles`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      body: JSON.stringify(data),
      signal: AbortSignal.timeout(API_CONFIG.timeout),
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error('❌ Erro HTTP:', response.status, errorText);
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }

    const result = await response.json();
    console.log('✅ Resposta do backend:', result);
    
    return {
      success: true,
      message: 'Importação criada com sucesso',
      data: {
        id: result.id,
        processingId: result.id?.toString() || `import_${Date.now()}`,
        estimatedTime: 30,
        status: result.status || 'pending'
      },
    };

  } catch (error) {
    console.error('❌ Erro ao enviar ImportFilesRequest para o backend:', error);

    if (error instanceof Error) {
      if (error.name === 'TimeoutError') {
        return {
          success: false,
          message: 'Timeout na comunicação com o backend',
          error: 'A requisição demorou mais que o esperado para ser processada',
        };
      }

      if (error.message.includes('fetch') || error.message.includes('NetworkError')) {
        return {
          success: false,
          message: 'Erro de conectividade',
          error: 'Não foi possível conectar ao backend. Verifique se o serviço está rodando.',
        };
      }

      return {
        success: false,
        message: 'Erro no processamento',
        error: error.message,
      };
    }

    return {
      success: false,
      message: 'Erro desconhecido',
      error: 'Ocorreu um erro inesperado durante o processamento',
    };
  }
}

/**
 * Verifica o status de processamento de um arquivo pelo ID
 */
export async function checkProcessingStatus(importId: number): Promise<{
  success: boolean;
  data?: ImportStatusResponse;
  error?: string;
}> {
  try {
    console.log(`🔍 Verificando status da importação: ${importId}`);
    
    const response = await fetch(`${API_CONFIG.baseUrl}/ImportFiles/${importId}`, {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
      },
      signal: AbortSignal.timeout(10000),
    });

    if (!response.ok) {
      if (response.status === 404) {
        throw new Error('Importação não encontrada');
      }
      const errorText = await response.text();
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }

    const result = await response.json();
    console.log('✅ Status da importação:', result);
    
    return {
      success: true,
      data: result,
    };

  } catch (error) {
    console.error('❌ Erro ao verificar status:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Erro desconhecido',
    };
  }
}

/**
 * Lista todas as importações
 */
export async function getAllImports(): Promise<{
  success: boolean;
  data?: ImportStatusResponse[];
  error?: string;
}> {
  try {
    console.log('📋 Buscando todas as importações');
    
    const response = await fetch(`${API_CONFIG.baseUrl}/ImportFiles`, {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
      },
      signal: AbortSignal.timeout(10000),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }

    const result = await response.json();
    console.log('✅ Lista de importações:', result);
    
    return {
      success: true,
      data: result,
    };

  } catch (error) {
    console.error('❌ Erro ao buscar importações:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Erro desconhecido',
    };
  }
}

/**
 * Atualiza o status de uma importação
 */
export async function updateImportStatus(
  importId: number, 
  status: number, 
  message?: string
): Promise<{ success: boolean; error?: string }> {
  try {
    console.log(`🔄 Atualizando status da importação ${importId} para ${status}`);
    
    const payload = {
      status,
      message: message || ''
    };

    const response = await fetch(`${API_CONFIG.baseUrl}/ImportFiles/${importId}/status`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      body: JSON.stringify(payload),
      signal: AbortSignal.timeout(10000),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }

    console.log('✅ Status atualizado com sucesso');
    
    return {
      success: true,
    };

  } catch (error) {
    console.error('❌ Erro ao atualizar status:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Erro desconhecido',
    };
  }
}

/**
 * Gera URL pré-assinada do S3 para upload
 */
export async function generateS3PreSignedUrl(fileName: string, contentType: string): Promise<{
  success: boolean;
  data?: { preSignedUrl: string; s3Key: string };
  error?: string;
}> {
  try {
    console.log(`🔗 Gerando URL pré-assinada para: ${fileName}`);
    
    const response = await fetch(`${API_CONFIG.baseUrl}/S3/generate-presigned-url`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      body: JSON.stringify({
        fileName,
        contentType
      }),
      signal: AbortSignal.timeout(10000),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }

    const result = await response.json();
    console.log('✅ URL pré-assinada gerada:', result);
    
    return {
      success: true,
      data: result,
    };

  } catch (error) {
    console.error('❌ Erro ao gerar URL pré-assinada:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Erro desconhecido',
    };
  }
}

/**
 * Testa a conectividade com o backend
 */
export async function testBackendConnection(): Promise<{ success: boolean; message: string }> {
  try {
    console.log('🔍 Testando conectividade com o backend');
    
    const response = await fetch(`${API_CONFIG.baseUrl}/health`, {
      method: 'GET',
      signal: AbortSignal.timeout(5000),
    });

    if (response.ok) {
      return { success: true, message: 'Backend conectado com sucesso' };
    } else {
      return { success: false, message: `Backend respondeu com status ${response.status}` };
    }
  } catch (error) {
    console.error('❌ Erro de conectividade:', error);
    return { 
      success: false, 
      message: error instanceof Error ? error.message : 'Erro de conectividade' 
    };
  }
}

/**
 * Busca o histórico de importações do backend
 */
export async function getImportHistory(): Promise<{ success: boolean; data?: any[]; error?: string }> {
  try {
    console.log('📥 Buscando histórico de importações...');

    const response = await fetch(`${API_CONFIG.baseUrl}/ImportFiles`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      signal: AbortSignal.timeout(API_CONFIG.timeout),
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error('❌ Erro HTTP ao buscar histórico:', response.status, errorText);
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }

    const data = await response.json();
    console.log('✅ Histórico de importações recebido:', data);
    
    return {
      success: true,
      data: data || []
    };

  } catch (error) {
    console.error('❌ Erro ao buscar histórico de importações:', error);
    return {
      success: false,
      error: error instanceof Error ? error.message : 'Erro desconhecido'
    };
  }
}

/**
 * Converte os dados da API para o formato ImportRecord
 */
function convertApiDataToImportRecord(apiData: any): ImportRecord {
  // Mapear dataType numérico para string
  const dataTypeMap: Record<number, DataType> = {
    1: "municipios",
    2: "estados", 
    3: "medicos",
    4: "hospitais",
    5: "pacientes",
    6: "cid10"
  };

  // Mapear fileFormat numérico para string
  const fileFormatMap: Record<number, FileFormat> = {
    1: "csv",
    2: "xlsx",
    3: "csv", // JSON mapeado para CSV por compatibilidade
    4: "xml"
  };

  // Mapear status numérico para string
  const statusMap: Record<number, ImportStatus> = {
    1: "processing",
    2: "processing", 
    3: "success",
    4: "error"
  };

  return {
    id: apiData.id?.toString() || apiData.id,
    fileName: apiData.fileName || 'Arquivo sem nome',
    dataType: dataTypeMap[apiData.dataType] || "municipios",
    fileFormat: fileFormatMap[apiData.fileFormat] || "csv",
    uploadDate: new Date(apiData.importedOn || Date.now()),
    status: statusMap[apiData.status] || "processing",
    summary: `Processamento de ${dataTypeMap[apiData.dataType] || 'dados'}`,
    totalRecords: apiData.totalRegisters || 0,
    successfulRecords: apiData.totalImportedRegisters || 0,
    errorRecords: apiData.totalFailedRegisters || 0,
    duplicateRecords: apiData.totalDuplicatedRegisters || 0,
    description: `Importação de ${apiData.fileName}`,
    errors: apiData.lineErrors?.map((error: any) => ({
      line: error.line || 0,
      field: error.field || 'Campo desconhecido',
      message: error.message || 'Erro não especificado',
      data: error.data
    })) || []
  };
}

/**
 * Busca e converte o histórico de importações
 */
export async function getImportRecords(): Promise<{ success: boolean; data?: ImportRecord[]; error?: string }> {
  const result = await getImportHistory();
  
  if (!result.success) {
    return result;
  }

  try {
    const convertedData = result.data?.map(convertApiDataToImportRecord) || [];
    return {
      success: true,
      data: convertedData
    };
  } catch (error) {
    console.error('❌ Erro ao converter dados:', error);
    return {
      success: false,
      error: 'Erro ao processar dados do histórico'
    };
  }
}