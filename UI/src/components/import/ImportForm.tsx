import { useState } from "react";
import { useDropzone } from "react-dropzone";
import { Upload, FileText, AlertCircle, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Progress } from "@/components/ui/progress";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { useS3MultipartUpload } from "@/hooks/useS3MultipartUpload";
import { DataTypeSelector } from "./DataTypeSelector";
import { FieldMappingComponent } from "./FieldMappingComponent";
import { FileHeaderReader } from "@/utils/fileHeaderReader";
import { DataType, FieldMapping, FileHeader, TABLE_SCHEMAS } from "@/types/import";

type UploadStep = "select-file" | "select-type" | "map-fields" | "upload";

const ImportForm = () => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [selectedDataType, setSelectedDataType] = useState<DataType | undefined>(undefined);
  const [fileHeader, setFileHeader] = useState<FileHeader | null>(null);
  const [fieldMappings, setFieldMappings] = useState<FieldMapping[]>([]);
  const [currentStep, setCurrentStep] = useState<UploadStep>("select-file");
  const [error, setError] = useState<string>("");
  const [isReadingHeader, setIsReadingHeader] = useState(false);

  const { 
    uploadFile, 
    progress, 
    isUploading, 
    error: uploadError,
    reset
  } = useS3MultipartUpload();

  const onDrop = async (acceptedFiles: File[]) => {
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
      setCurrentStep("select-type");
    }
  };

  const handleDataTypeSelect = async (dataType: DataType) => {
    setSelectedDataType(dataType);
    
    if (!selectedFile) return;
    
    // Verificar se o arquivo suporta leitura de cabeçalho
    const fileName = selectedFile.name.toLowerCase();
    if (fileName.endsWith('.csv') || fileName.endsWith('.xlsx') || fileName.endsWith('.xls')) {
      setIsReadingHeader(true);
      try {
        const header = await FileHeaderReader.readFileHeader(selectedFile);
        setFileHeader(header);
        setCurrentStep("map-fields");
      } catch (error) {
        console.error("Erro ao ler cabeçalho:", error);
        setError(error instanceof Error ? error.message : "Erro ao ler cabeçalho do arquivo");
        setCurrentStep("upload"); // Pular mapeamento se não conseguir ler cabeçalho
      } finally {
        setIsReadingHeader(false);
      }
    } else {
      // Para outros tipos de arquivo, pular mapeamento
      setCurrentStep("upload");
    }
  };

  const handleMappingComplete = () => {
    setCurrentStep("upload");
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
    if (!selectedFile || !selectedDataType) return;

    try {
      setError("");
      const fileFormat = getFileFormat(selectedFile.name);
      const description = `Upload de ${selectedDataType} - ${fieldMappings.length} campos mapeados`;
      
      await uploadFile(selectedFile, selectedDataType, fileFormat, description);
    } catch (err) {
      console.error("Erro no upload:", err);
      setError(err instanceof Error ? err.message : "Erro no upload do arquivo");
    }
  };

  const getFileFormat = (fileName: string): string => {
    const extension = fileName.toLowerCase().split('.').pop();
    switch (extension) {
      case 'xlsx':
      case 'xls':
        return 'xlsx';
      case 'csv':
        return 'csv';
      case 'xml':
        return 'xml';
      default:
        return 'csv';
    }
  };

  const handleReset = () => {
    setSelectedFile(null);
    setSelectedDataType(undefined);
    setFileHeader(null);
    setFieldMappings([]);
    setCurrentStep("select-file");
    setError("");
    reset();
  };

  const handleBack = () => {
    switch (currentStep) {
      case "select-type":
        setCurrentStep("select-file");
        setSelectedFile(null);
        break;
      case "map-fields":
        setCurrentStep("select-type");
        setFileHeader(null);
        setFieldMappings([]);
        break;
      case "upload":
        if (fileHeader) {
          setCurrentStep("map-fields");
        } else {
          setCurrentStep("select-type");
        }
        break;
    }
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
    <div className="max-w-4xl mx-auto p-6 space-y-6">
      {/* Header com progresso */}
      <div className="text-center">
        <h2 className="text-2xl font-bold text-gray-900 mb-2">
          Importação de Dados
        </h2>
        <p className="text-gray-600">
          Faça upload de arquivos CSV, XML ou Excel para importar dados
        </p>
        
        {/* Indicador de progresso */}
        <div className="flex items-center justify-center mt-4 space-x-4">
          <div className={`flex items-center space-x-2 ${currentStep === "select-file" ? "text-blue-600" : currentStep === "select-type" || currentStep === "map-fields" || currentStep === "upload" ? "text-green-600" : "text-gray-400"}`}>
            <div className={`w-3 h-3 rounded-full ${currentStep === "select-file" ? "bg-blue-600" : currentStep === "select-type" || currentStep === "map-fields" || currentStep === "upload" ? "bg-green-600" : "bg-gray-400"}`} />
            <span className="text-sm font-medium">Arquivo</span>
          </div>
          <div className={`w-8 h-0.5 ${currentStep === "select-type" || currentStep === "map-fields" || currentStep === "upload" ? "bg-green-600" : "bg-gray-300"}`} />
          <div className={`flex items-center space-x-2 ${currentStep === "select-type" ? "text-blue-600" : currentStep === "map-fields" || currentStep === "upload" ? "text-green-600" : "text-gray-400"}`}>
            <div className={`w-3 h-3 rounded-full ${currentStep === "select-type" ? "bg-blue-600" : currentStep === "map-fields" || currentStep === "upload" ? "bg-green-600" : "bg-gray-400"}`} />
            <span className="text-sm font-medium">Tipo</span>
          </div>
          <div className={`w-8 h-0.5 ${currentStep === "map-fields" || currentStep === "upload" ? "bg-green-600" : "bg-gray-300"}`} />
          <div className={`flex items-center space-x-2 ${currentStep === "map-fields" ? "text-blue-600" : currentStep === "upload" ? "text-green-600" : "text-gray-400"}`}>
            <div className={`w-3 h-3 rounded-full ${currentStep === "map-fields" ? "bg-blue-600" : currentStep === "upload" ? "bg-green-600" : "bg-gray-400"}`} />
            <span className="text-sm font-medium">Mapeamento</span>
          </div>
          <div className={`w-8 h-0.5 ${currentStep === "upload" ? "bg-green-600" : "bg-gray-300"}`} />
          <div className={`flex items-center space-x-2 ${currentStep === "upload" ? "text-blue-600" : "text-gray-400"}`}>
            <div className={`w-3 h-3 rounded-full ${currentStep === "upload" ? "bg-blue-600" : "bg-gray-400"}`} />
            <span className="text-sm font-medium">Upload</span>
          </div>
        </div>
      </div>

      {/* Step 1: Seleção de arquivo */}
      {currentStep === "select-file" && (
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
      )}

      {/* Step 2: Seleção do tipo de dados */}
      {currentStep === "select-type" && selectedFile && (
        <div className="space-y-6">
          <div className="bg-gray-50 rounded-lg p-4">
            <div className="flex items-center space-x-3">
              <FileText className="h-8 w-8 text-blue-600" />
              <div>
                <p className="font-medium text-gray-900">{selectedFile.name}</p>
                <p className="text-sm text-gray-500">
                  {formatFileSize(selectedFile.size)}
                </p>
              </div>
            </div>
          </div>

          <DataTypeSelector
            value={selectedDataType}
            onChange={handleDataTypeSelect}
            disabled={isReadingHeader}
          />

          {isReadingHeader && (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-6 w-6 animate-spin mr-2" />
              <span className="text-sm text-gray-600">
                Lendo cabeçalho do arquivo...
              </span>
            </div>
          )}

          <div className="flex justify-between">
            <Button variant="outline" onClick={handleBack}>
              Voltar
            </Button>
          </div>
        </div>
      )}

      {/* Step 3: Mapeamento de campos */}
      {currentStep === "map-fields" && fileHeader && selectedDataType && (
        <div className="space-y-6">
          <FieldMappingComponent
            fileHeader={fileHeader}
            selectedDataType={selectedDataType}
            onMappingChange={setFieldMappings}
            onComplete={handleMappingComplete}
          />

          <div className="flex justify-between">
            <Button variant="outline" onClick={handleBack}>
              Voltar
            </Button>
          </div>
        </div>
      )}

      {/* Step 4: Upload */}
      {currentStep === "upload" && selectedFile && selectedDataType && (
        <div className="space-y-6">
          <div className="bg-gray-50 rounded-lg p-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <FileText className="h-8 w-8 text-blue-600" />
                <div>
                  <p className="font-medium text-gray-900">{selectedFile.name}</p>
                  <p className="text-sm text-gray-500">
                    {formatFileSize(selectedFile.size)} • {TABLE_SCHEMAS[selectedDataType].label}
                  </p>
                  {fieldMappings.length > 0 && (
                    <p className="text-xs text-blue-600">
                      {fieldMappings.length} campos mapeados
                    </p>
                  )}
                  {getUploadTypeIndicator()}
                </div>
              </div>
              
              {!isUploading && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={handleReset}
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
          </div>

          <div className="flex justify-between">
            <Button variant="outline" onClick={handleBack}>
              Voltar
            </Button>
            
            {!isUploading && (
              <Button onClick={handleUpload} size="lg">
                <Upload className="w-4 h-4 mr-2" />
                Iniciar Upload
              </Button>
            )}

            {isUploading && (
              <div className="flex items-center">
                <Loader2 className="h-5 w-5 animate-spin mr-2" />
                <span className="text-sm text-gray-600">
                  Upload em progresso...
                </span>
              </div>
            )}
          </div>
        </div>
      )}

      {/* Mensagens de erro */}
      {(error || uploadError) && (
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>
            {error || uploadError}
          </AlertDescription>
        </Alert>
      )}

      {/* Mensagem de sucesso */}
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

      {/* Footer com informações */}
      <div className="text-center text-sm text-gray-500">
        <p>
          Arquivos suportados: CSV, XML, Excel (.xls, .xlsx)
        </p>
        <p>
          Tamanho máximo: 5GB | Upload multipart automático para arquivos grandes
        </p>
      </div>
    </div>
  );
};

export default ImportForm;