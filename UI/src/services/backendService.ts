import { ImportFilesRequest, KeyPair } from '@/types/import';

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
  baseUrl: import.meta.env.VITE_API_BASE_URL || 'http://localhost:6000/v1',
  timeout: 30000, // 30 segundos
};

/**
 * Envia os dados do arquivo e mapeamentos para o backend processar
 */
export async function sendFileToBackend(data: ImportFilesRequest): Promise<BackendProcessingResponse> {
  try {
    console.log('📤 Enviando dados para o backend via POST:');
    console.log('🌐 URL:', `${API_CONFIG.baseUrl}/ImportFiles`);
    console.log('📊 Dados:', data);

    // Validar dados antes de enviar
    if (!data.fileName || !data.s3PreSignedUrl) {
      throw new Error('Nome do arquivo e URL do S3 são obrigatórios');
    }

    // Preparar payload conforme o ImportFilesRequest
    const payload: ImportFilesRequest = {
      dataType: data.dataType,
      fileFormat: data.fileFormat,
      description: data.description || '',
      fileName: data.fileName,
      s3PreSignedUrl: data.s3PreSignedUrl,
      status: data.status,
      fieldMappings: data.fieldMappings || []
    };

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