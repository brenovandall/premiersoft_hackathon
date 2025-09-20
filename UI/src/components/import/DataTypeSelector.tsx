import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { DataType, TABLE_SCHEMAS } from "@/types/import";
import { Info } from "lucide-react";

interface DataTypeSelectorProps {
  value: DataType | undefined;
  onChange: (value: DataType) => void;
  disabled?: boolean;
}

export const DataTypeSelector = ({ value, onChange, disabled }: DataTypeSelectorProps) => {
  const primaryDataTypes: DataType[] = ["municipios", "estados", "medicos", "hospitais", "pacientes", "cid10"];

  return (
    <div className="space-y-2">
      <Label htmlFor="dataType" className="text-sm font-medium">
        Tipo de Dados
      </Label>
      <Select 
        value={value} 
        onValueChange={onChange}
        disabled={disabled}
      >
        <SelectTrigger id="dataType" className="w-full">
          <SelectValue placeholder="Selecione o tipo de dados do arquivo" />
        </SelectTrigger>
        <SelectContent>
          {primaryDataTypes.map((dataType) => {
            const schema = TABLE_SCHEMAS[dataType];
            return (
              <SelectItem key={dataType} value={dataType}>
                <div className="flex items-center justify-between w-full">
                  <div className="flex flex-col">
                    <span className="font-medium">{schema.label}</span>
                    <span className="text-xs text-muted-foreground">
                      {schema.description}
                    </span>
                  </div>
                </div>
              </SelectItem>
            );
          })}
        </SelectContent>
      </Select>
      
      {value && (
        <div className="p-2 bg-blue-50 rounded-md border border-blue-200">
          <div className="flex items-start gap-2">
            <Info className="h-4 w-4 text-blue-600 mt-0.5 flex-shrink-0" />
            <div className="text-xs text-blue-700">
              <p className="font-medium mb-1">Campos esperados:</p>
              <p className="text-blue-600">
                {TABLE_SCHEMAS[value].fields.join(", ")}
              </p>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};