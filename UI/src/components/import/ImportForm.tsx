import { useState } from "react";
import { useDropzone } from "react-dropzone";
import { Upload, FileText, AlertCircle, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Card } from "@/components/ui/card";
import { useToast } from "@/hooks/use-toast";
import { DataType, FileFormat } from "@/types/import";
import { useS3Upload } from "@/hooks/useS3Upload";

const dataTypeOptions: { value: DataType; label: string }[] = [
  { value: "hospitals", label: "Hospitais" },
  { value: "doctors", label: "Médicos" },
  { value: "patients", label: "Pacientes" },
  { value: "locations", label: "Estados e Municípios" },
  { value: "cid10", label: "Tabela CID-10" },
];

const fileFormatOptions: { value: FileFormat; label: string }[] = [
  { value: "json", label: "JSON" },
  { value: "xml", label: "XML" },
  { value: "csv", label: "CSV / Excel" },
];

export const ImportForm = () => {
  const [dataType, setDataType] = useState<DataType | "">("");
  const [fileFormat, setFileFormat] = useState<FileFormat | "">("");
  const [description, setDescription] = useState("");
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const { toast } = useToast();
  const { uploadFile, isUploading, progress, error } = useS3Upload();

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    accept: {
      'application/json': ['.json'],
      'application/xml': ['.xml'],
      'text/xml': ['.xml'],
      'text/csv': ['.csv'],
      'application/vnd.ms-excel': ['.xls'],
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': ['.xlsx'],
    },
    maxFiles: 1,
    onDrop: (acceptedFiles) => {
      if (acceptedFiles.length > 0) {
        setSelectedFile(acceptedFiles[0]);
      }
    },
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!dataType || !fileFormat || !selectedFile) {
      toast({
        title: "Campos obrigatórios",
        description: "Selecione o tipo de dado, formato e arquivo para continuar.",
        variant: "destructive",
      });
      return;
    }

    try {
      const result = await uploadFile(selectedFile, dataType, fileFormat, description);
      
      if (result.success) {
        toast({
          title: "Upload realizado com sucesso!",
          description: `Arquivo ${selectedFile.name} enviado para o S3. Chave: ${result.data?.key}`,
        });

        // Reset form
        setSelectedFile(null);
        setDataType("");
        setFileFormat("");
        setDescription("");
      } else {
        throw new Error(result.message || 'Erro no upload');
      }
    } catch (error) {
      toast({
        title: "Erro no upload",
        description: error instanceof Error ? error.message : "Ocorreu um erro ao processar o arquivo. Tente novamente.",
        variant: "destructive",
      });
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="dataType">Tipo de Dado *</Label>
          <Select value={dataType} onValueChange={(value: DataType) => setDataType(value)}>
            <SelectTrigger>
              <SelectValue placeholder="Selecione o tipo de dado" />
            </SelectTrigger>
            <SelectContent>
              {dataTypeOptions.map((option) => (
                <SelectItem key={option.value} value={option.value}>
                  {option.label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="space-y-2">
          <Label htmlFor="fileFormat">Formato do Arquivo *</Label>
          <Select value={fileFormat} onValueChange={(value: FileFormat) => setFileFormat(value)}>
            <SelectTrigger>
              <SelectValue placeholder="Selecione o formato" />
            </SelectTrigger>
            <SelectContent>
              {fileFormatOptions.map((option) => (
                <SelectItem key={option.value} value={option.value}>
                  {option.label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="description">Descrição / Origem (Opcional)</Label>
        <Textarea
          id="description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          placeholder="Ex: Dados do Hospital da Criança - Setembro/2025"
          rows={2}
        />
      </div>

      <div className="space-y-2">
        <Label>Arquivo *</Label>
        <Card
          {...getRootProps()}
          className={`border-2 border-dashed p-8 text-center cursor-pointer transition-colors ${
            isDragActive
              ? "border-primary bg-primary/5"
              : selectedFile
              ? "border-success bg-success/5"
              : "border-muted-foreground/30 hover:border-primary/50"
          }`}
        >
          <input {...getInputProps()} />
          <div className="flex flex-col items-center space-y-2">
            {selectedFile ? (
              <>
                <FileText className="h-12 w-12 text-success" />
                <div>
                  <p className="font-medium text-success">{selectedFile.name}</p>
                  <p className="text-sm text-muted-foreground">
                    {(selectedFile.size / 1024 / 1024).toFixed(2)} MB
                  </p>
                </div>
              </>
            ) : (
              <>
                <Upload className="h-12 w-12 text-muted-foreground" />
                <div>
                  <p className="font-medium">
                    {isDragActive ? "Solte o arquivo aqui..." : "Arraste um arquivo ou clique para selecionar"}
                  </p>
                  <p className="text-sm text-muted-foreground">
                    Formatos suportados: JSON, XML, CSV, Excel
                  </p>
                </div>
              </>
            )}
          </div>
        </Card>
        {selectedFile && (
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() => setSelectedFile(null)}
          >
            Remover arquivo
          </Button>
        )}
      </div>

      <Button type="submit" disabled={isUploading || !selectedFile} className="w-full">
        {isUploading ? (
          <>
            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            Enviando... {progress?.percentage || 0}%
          </>
        ) : (
          "Enviar Arquivo"
        )}
      </Button>

      {/* Progress Bar */}
      {isUploading && progress && (
        <div className="w-full bg-gray-200 rounded-full h-2 mt-4">
          <div
            className="bg-blue-600 h-2 rounded-full transition-all duration-300"
            style={{ width: `${progress.percentage}%` }}
          />
        </div>
      )}
    </form>
  );
};