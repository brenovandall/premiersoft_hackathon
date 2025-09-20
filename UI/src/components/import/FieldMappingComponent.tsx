import { useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { DataType, FieldMapping, FileHeader, TABLE_SCHEMAS } from "@/types/import";
import { ArrowRight, X, CheckCircle, AlertCircle, Move } from "lucide-react";

interface FieldMappingComponentProps {
  fileHeader: FileHeader;
  selectedDataType: DataType;
  onMappingChange: (mappings: FieldMapping[]) => void;
  onComplete: () => void;
}

interface SourceFieldProps {
  fieldName: string;
  sampleData?: string;
  isUsed: boolean;
  onSelect: () => void;
}

const SourceField = ({ fieldName, sampleData, isUsed, onSelect }: SourceFieldProps) => {
  return (
    <div
      onClick={!isUsed ? onSelect : undefined}
      className={`
        p-3 border rounded-lg transition-all
        ${isUsed ? 'bg-green-50 border-green-300 cursor-not-allowed' : 'bg-white border-gray-300 hover:border-blue-400 cursor-pointer'}
      `}
    >
      <div className="flex items-center justify-between">
        <div className="flex-1">
          <p className="font-medium text-sm flex items-center gap-2">
            <Move className="h-3 w-3 text-gray-400" />
            {fieldName}
          </p>
          {sampleData && (
            <p className="text-xs text-gray-500 truncate ml-5">Ex: {sampleData}</p>
          )}
        </div>
        {isUsed && <CheckCircle className="h-4 w-4 text-green-600" />}
      </div>
    </div>
  );
};

interface TargetFieldProps {
  fieldName: string;
  mappedSourceField?: string;
  onRemove: () => void;
  onSelectSource: (sourceField: string) => void;
  availableFields: string[];
  isRequired?: boolean;
}

const TargetField = ({ 
  fieldName, 
  mappedSourceField, 
  onRemove, 
  onSelectSource, 
  availableFields,
  isRequired 
}: TargetFieldProps) => {
  return (
    <div className={`
      p-3 border-2 rounded-lg min-h-[60px] transition-all
      ${mappedSourceField ? 'border-green-400 bg-green-50' : 'border-dashed border-gray-300 bg-gray-50'}
      ${isRequired ? 'border-red-200' : ''}
    `}>
      <div className="flex items-center justify-between">
        <div className="flex-1">
          <div className="flex items-center gap-2">
            <p className="font-medium text-sm">{fieldName}</p>
            {isRequired && <Badge variant="destructive" className="text-xs">Obrigatório</Badge>}
          </div>
          {mappedSourceField ? (
            <div className="flex items-center gap-2 mt-1">
              <ArrowRight className="h-3 w-3 text-green-600" />
              <span className="text-xs text-green-700 font-medium">{mappedSourceField}</span>
            </div>
          ) : (
            <div className="mt-2">
              <Select onValueChange={onSelectSource}>
                <SelectTrigger className="h-8 text-xs">
                  <SelectValue placeholder="Selecionar campo..." />
                </SelectTrigger>
                <SelectContent>
                  {availableFields.map((field) => (
                    <SelectItem key={field} value={field} className="text-xs">
                      {field}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          )}
        </div>
        {mappedSourceField && (
          <Button
            variant="ghost"
            size="sm"
            onClick={onRemove}
            className="h-6 w-6 p-0 hover:bg-red-100"
          >
            <X className="h-3 w-3" />
          </Button>
        )}
      </div>
    </div>
  );
};

export const FieldMappingComponent = ({ 
  fileHeader, 
  selectedDataType, 
  onMappingChange, 
  onComplete 
}: FieldMappingComponentProps) => {
  const [mappings, setMappings] = useState<FieldMapping[]>([]);
  const [selectedSourceField, setSelectedSourceField] = useState<string | null>(null);
  
  const targetFields = TABLE_SCHEMAS[selectedDataType].fields;
  const requiredFields = ["codigo", "nome", "nome_completo"]; // Campos que geralmente são obrigatórios
  
  const handleMapping = (targetField: string, sourceField: string) => {
    const newMappings = mappings.filter(m => m.targetField !== targetField && m.sourceField !== sourceField);
    newMappings.push({
      sourceField,
      targetField,
      dataType: selectedDataType
    });
    setMappings(newMappings);
    onMappingChange(newMappings);
    setSelectedSourceField(null);
  };

  const handleRemove = (targetField: string) => {
    const newMappings = mappings.filter(m => m.targetField !== targetField);
    setMappings(newMappings);
    onMappingChange(newMappings);
  };

  const getMappedSourceField = (targetField: string) => {
    return mappings.find(m => m.targetField === targetField)?.sourceField;
  };

  const isSourceFieldUsed = (sourceField: string) => {
    return mappings.some(m => m.sourceField === sourceField);
  };

  const getAvailableSourceFields = () => {
    return fileHeader.fields.filter(field => !isSourceFieldUsed(field));
  };

  const getSampleData = (fieldName: string) => {
    if (!fileHeader.sampleData?.length) return undefined;
    const sample = fileHeader.sampleData[0]?.[fieldName];
    return sample ? String(sample).substring(0, 20) : undefined;
  };

  const getMappingStats = () => {
    const totalRequired = targetFields.filter(field => requiredFields.includes(field)).length;
    const mappedRequired = targetFields.filter(field => 
      requiredFields.includes(field) && getMappedSourceField(field)
    ).length;
    const totalMapped = mappings.length;
    
    return { totalRequired, mappedRequired, totalMapped, totalFields: targetFields.length };
  };

  const stats = getMappingStats();
  const canComplete = stats.mappedRequired === stats.totalRequired && stats.totalMapped > 0;

  return (
    <div className="space-y-6">
      {/* Header com estatísticas */}
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-lg font-semibold">Mapeamento de Campos</h3>
          <p className="text-sm text-gray-600">
            Configure o mapeamento entre os campos do arquivo e a tabela {TABLE_SCHEMAS[selectedDataType].label}
          </p>
        </div>
        <div className="text-right">
          <div className="flex items-center gap-2">
            {stats.mappedRequired === stats.totalRequired ? (
              <CheckCircle className="h-4 w-4 text-green-600" />
            ) : (
              <AlertCircle className="h-4 w-4 text-amber-600" />
            )}
            <span className="text-sm font-medium">
              {stats.totalMapped} de {stats.totalFields} campos mapeados
            </span>
          </div>
          <p className="text-xs text-gray-500">
            {stats.mappedRequired} de {stats.totalRequired} obrigatórios
          </p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Campos do arquivo */}
        <Card>
          <CardHeader>
            <CardTitle className="text-base">Campos do Arquivo ({fileHeader.fields.length})</CardTitle>
            <p className="text-sm text-gray-600">Clique em um campo para usar no mapeamento</p>
          </CardHeader>
          <CardContent className="space-y-2 max-h-[400px] overflow-y-auto">
            {fileHeader.fields.map((field, index) => (
              <SourceField
                key={index}
                fieldName={field}
                sampleData={getSampleData(field)}
                isUsed={isSourceFieldUsed(field)}
                onSelect={() => setSelectedSourceField(field)}
              />
            ))}
          </CardContent>
        </Card>

        {/* Campos da tabela de destino */}
        <Card>
          <CardHeader>
            <CardTitle className="text-base">
              Campos da Tabela {TABLE_SCHEMAS[selectedDataType].label}
            </CardTitle>
            <p className="text-sm text-gray-600">Use os dropdowns para mapear os campos</p>
          </CardHeader>
          <CardContent className="space-y-2 max-h-[400px] overflow-y-auto">
            {targetFields.map((field) => (
              <TargetField
                key={field}
                fieldName={field}
                mappedSourceField={getMappedSourceField(field)}
                onSelectSource={(sourceField) => handleMapping(field, sourceField)}
                onRemove={() => handleRemove(field)}
                availableFields={getAvailableSourceFields()}
                isRequired={requiredFields.includes(field)}
              />
            ))}
          </CardContent>
        </Card>
      </div>

      {/* Actions */}
      <div className="flex items-center justify-between pt-4 border-t">
        <div className="text-sm text-gray-600">
          {!canComplete && (
            <p className="text-amber-600">
              Complete o mapeamento dos campos obrigatórios para continuar
            </p>
          )}
        </div>
        <Button 
          onClick={onComplete}
          disabled={!canComplete}
          className="flex items-center gap-2"
        >
          <CheckCircle className="h-4 w-4" />
          Confirmar Mapeamento
        </Button>
      </div>
    </div>
  );
};