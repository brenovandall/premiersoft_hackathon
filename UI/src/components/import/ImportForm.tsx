import { useState } from "react";
import { useDropzone } from "react-dropzone";
import { Upload, FileText, AlertCircle, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Progress } from "@/components/ui/progress";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { useS3MultipartUpload } from "@/hooks/useS3MultipartUpload";

const ImportForm = () => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [error, setError] = useState<string>("");

  const { 
    uploadFile, 
    progress, 
    isUploading, 
    error: uploadError,
    reset
  } = useS3MultipartUpload();

  const onDrop = (acceptedFiles: File[]) => {
    setError("");
    if (acceptedFiles.length > 0) {
      const file = acceptedFiles[0];
      
      const allowedTypes = [
        'text/csv',
        'application/json',
        'text/xml',
        'application/xml',
        'application/vnd.ms-excel',
        'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
      ];
      
      if (!allowedTypes.includes(file.type) && !file.name.match(/\.(csv|json|xml|xlsx?|txt)$/i)) {
        setError("Tipo de arquivo não suportado. Aceitos: CSV, JSON, XML, Excel, TXT");
        return;
      }

      const maxSize = 5 * 1024 * 1024 * 1024;
      if (file.size > maxSize) {
        setError("Arquivo muito grande. Tamanho máximo: 5GB");
        return;
      }

      setSelectedFile(file);
    }
  };

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: {
      'text/csv': ['.csv'],
      'application/json': ['.json'],
      'text/xml': ['.xml'],
      'application/xml': ['.xml'],
      'application/vnd.ms-excel': ['.xls'],
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': ['.xlsx'],
      'text/plain': ['.txt']
    },
    multiple: false,
    maxSize: 5 * 1024 * 1024 * 1024
  });

  const handleUpload = async () => {
    if (!selectedFile) return;

    try {
      setError("");
      await uploadFile(selectedFile, "manual-import", "csv", "Upload manual via interface");
    } catch (err) {
      console.error("Erro no upload:", err);
      setError(err instanceof Error ? err.message : "Erro no upload do arquivo");
    }
  };

  const handleRemoveFile = () => {
    setSelectedFile(null);
    setError("");
    reset(); // Reset do estado do upload
  };

  const getProgressText = () => {
    if (!progress) return "Preparando...";
    
    switch (progress.phase) {
      case 'reading':
        return "Preparando arquivo para upload...";
      case 'uploading':
        return progress.totalParts && progress.totalParts > 1 
          ? `Enviando parte ${progress.currentPart || 1} de ${progress.totalParts}`
          : "Enviando arquivo...";
      case 'completing':
        return "Finalizando upload...";
      default:
        return `${progress.percentage}% completo`;
    }
  };

  const getUploadTypeIndicator = () => {
    if (!selectedFile) return null;
    
    const fileSizeMB = selectedFile.size / (1024 * 1024);
    const fileSizeGB = selectedFile.size / (1024 * 1024 * 1024);

    if (fileSizeMB > 100 && fileSizeMB <= 1024) {
      return (
        <p className="text-xs text-blue-600 mt-1">
          📦 Upload multipart - dividido em partes para melhor performance
        </p>
      );
    }
    
    if (fileSizeGB > 1 && fileSizeGB <= 3) {
      return (
        <p className="text-xs text-amber-600 mt-1">
          ⚡ Arquivo grande - upload multipart otimizado (pode demorar alguns minutos)
        </p>
      );
    }
    
    if (fileSizeGB > 3) {
      return (
        <p className="text-xs text-red-600 mt-1">
          🚀 Arquivo muito grande - upload multipart com processamento especial
        </p>
      );
    }
    
    return null;
  };

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return "0 Bytes";
    const k = 1024;
    const sizes = ["Bytes", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
  };

  return (
    <div className="max-w-2xl mx-auto p-6 space-y-6">
      <div className="text-center">
        <h2 className="text-2xl font-bold text-gray-900 mb-2">
          Importação de Dados
        </h2>
        <p className="text-gray-600">
          Faça upload de arquivos CSV, JSON, XML ou Excel para importar dados
        </p>
      </div>

      <div
        {...getRootProps()}
        className={`border-2 border-dashed rounded-lg p-8 text-center cursor-pointer transition-colors ${
          isDragActive
            ? "border-blue-400 bg-blue-50"
            : "border-gray-300 hover:border-gray-400"
        }`}
      >
        <input {...getInputProps()} />
        <Upload className="mx-auto h-12 w-12 text-gray-400 mb-4" />
        {isDragActive ? (
          <p className="text-blue-600">Solte o arquivo aqui...</p>
        ) : (
          <div>
            <p className="text-gray-600 mb-2">
              Clique para selecionar ou arraste um arquivo aqui
            </p>
            <p className="text-sm text-gray-400">
              CSV, JSON, XML, Excel (máx. 5GB)
            </p>
          </div>
        )}
      </div>

      {selectedFile && (
        <div className="bg-gray-50 rounded-lg p-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-3">
              <FileText className="h-8 w-8 text-blue-600" />
              <div>
                <p className="font-medium text-gray-900">{selectedFile.name}</p>
                <p className="text-sm text-gray-500">
                  {formatFileSize(selectedFile.size)}
                </p>
                {getUploadTypeIndicator()}
              </div>
            </div>
            
            {!isUploading && (
              <Button
                variant="outline"
                size="sm"
                onClick={handleRemoveFile}
              >
                Remover
              </Button>
            )}
          </div>

          {isUploading && (
            <div className="mt-4 space-y-2">
              <div className="flex justify-between text-sm">
                <span className="text-gray-600">{getProgressText()}</span>
                <span className="text-gray-900 font-medium">{progress?.percentage || 0}%</span>
              </div>
              <Progress value={progress?.percentage || 0} className="w-full" />
              
              <div className="flex items-center space-x-2 text-xs text-gray-500">
                <div className={`w-2 h-2 rounded-full ${progress?.phase === 'reading' ? 'bg-blue-500' : 'bg-gray-300'}`} />
                <span>Preparando</span>
                <div className={`w-2 h-2 rounded-full ${progress?.phase === 'uploading' ? 'bg-blue-500' : progress?.phase === 'completing' ? 'bg-green-500' : 'bg-gray-300'}`} />
                <span>Enviando</span>
                <div className={`w-2 h-2 rounded-full ${progress?.phase === 'completing' ? 'bg-blue-500' : progress?.percentage === 100 ? 'bg-green-500' : 'bg-gray-300'}`} />
                <span>Finalizando</span>
              </div>
            </div>
          )}

          {!isUploading && (
            <div className="mt-4">
              <Button onClick={handleUpload} className="w-full" size="lg">
                <Upload className="w-4 h-4 mr-2" />
                Iniciar Upload
              </Button>
            </div>
          )}

          {isUploading && (
            <div className="mt-4 flex items-center justify-center">
              <Loader2 className="h-5 w-5 animate-spin mr-2" />
              <span className="text-sm text-gray-600">
                Upload em progresso...
              </span>
            </div>
          )}
        </div>
      )}

      {(error || uploadError) && (
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>
            {error || uploadError}
          </AlertDescription>
        </Alert>
      )}

      {progress?.percentage === 100 && !error && !uploadError && (
        <Alert className="border-green-200 bg-green-50">
          <div className="flex items-center">
            <div className="w-4 h-4 bg-green-500 rounded-full mr-2" />
            <AlertDescription className="text-green-800">
              Upload concluído com sucesso! O arquivo foi enviado para o S3.
            </AlertDescription>
          </div>
        </Alert>
      )}

      <div className="text-center text-sm text-gray-500">
        <p>
          Arquivos suportados: CSV, JSON, XML, Excel (.xls, .xlsx), TXT
        </p>
        <p>
          Tamanho máximo: 5GB | Upload multipart automático para arquivos grandes
        </p>
      </div>
    </div>
  );
};

export default ImportForm;