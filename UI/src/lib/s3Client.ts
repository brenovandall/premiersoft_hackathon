import { S3Client } from '@aws-sdk/client-s3';

// Configuração do cliente S3
const s3Client = new S3Client({
  region: import.meta.env.VITE_AWS_REGION || 'us-east-1',
  credentials: {
    accessKeyId: import.meta.env.VITE_AWS_ACCESS_KEY_ID || '',
    secretAccessKey: import.meta.env.VITE_AWS_SECRET_ACCESS_KEY || '',
  },
  // Configurações para compatibilidade com navegador
  forcePathStyle: false, // Mudando para false para melhor compatibilidade
  requestHandler: {
    requestTimeout: 1800000, // 30 minutos para arquivos grandes
    connectionTimeout: 60000, // 1 minuto para conectar
  },
});

export { s3Client };

// Constantes para configuração
export const S3_CONFIG = {
  bucketName: import.meta.env.VITE_AWS_S3_BUCKET_NAME || '',
  region: import.meta.env.VITE_AWS_REGION || 'us-east-1',
  maxFileSize: parseInt(import.meta.env.VITE_MAX_FILE_SIZE || '999999999999999999'), 
} as const;

// Função para gerar chave única para o S3
export function generateS3Key(dataType: string, fileName: string): string {
  const timestamp = Date.now();
  const date = new Date().toISOString().split('T')[0];
  const fileExtension = fileName.substring(fileName.lastIndexOf('.'));
  const cleanFileName = fileName.substring(0, fileName.lastIndexOf('.')).replace(/[^a-zA-Z0-9-_]/g, '_');
  
  return `uploads/${dataType}/${date}/${timestamp}-${cleanFileName}${fileExtension}`;
}

// Validação de configuração
export function validateS3Config(): { isValid: boolean; missingVars: string[] } {
  const missingVars: string[] = [];
  
  if (!import.meta.env.VITE_AWS_ACCESS_KEY_ID) {
    missingVars.push('VITE_AWS_ACCESS_KEY_ID');
  }
  
  if (!import.meta.env.VITE_AWS_SECRET_ACCESS_KEY) {
    missingVars.push('VITE_AWS_SECRET_ACCESS_KEY');
  }
  
  if (!import.meta.env.VITE_AWS_S3_BUCKET_NAME) {
    missingVars.push('VITE_AWS_S3_BUCKET_NAME');
  }
  
  return {
    isValid: missingVars.length === 0,
    missingVars
  };
}

// Gerar URL pública para arquivo no S3
export function getS3PublicUrl(key: string): string {
  return `https://${S3_CONFIG.bucketName}.s3.${S3_CONFIG.region}.amazonaws.com/${key}`;
}

// Função para testar conectividade S3
export async function testS3Connection(): Promise<{ success: boolean; message: string }> {
  try {
    const { HeadBucketCommand } = await import('@aws-sdk/client-s3');
    const command = new HeadBucketCommand({ Bucket: S3_CONFIG.bucketName });
    await s3Client.send(command);
    return { success: true, message: 'Conexão S3 OK' };
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Erro desconhecido';
    return { success: false, message: `Erro na conexão S3: ${message}` };
  }
}
