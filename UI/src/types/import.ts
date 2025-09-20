export type DataType = "hospitals" | "doctors" | "patients" | "locations" | "cid10";
export type FileFormat = "json" | "xml" | "csv";
export type ImportStatus = "processing" | "success" | "warning" | "error";

export interface ImportRecord {
  id: string;
  fileName: string;
  dataType: DataType;
  fileFormat: FileFormat;
  uploadDate: Date;
  status: ImportStatus;
  summary: string;
  totalRecords?: number;
  successfulRecords?: number;
  errorRecords?: number;
  duplicateRecords?: number;
  description?: string;
  errors?: ImportError[];
}

export interface ImportError {
  line: number;
  field: string;
  message: string;
  data?: string;
}