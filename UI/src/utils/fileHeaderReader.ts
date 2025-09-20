import * as XLSX from 'xlsx';
import Papa from 'papaparse';
import { FileHeader } from '@/types/import';

export class FileHeaderReader {
  static async readFileHeader(file: File): Promise<FileHeader> {
    const fileName = file.name.toLowerCase();
    
    if (fileName.endsWith('.csv')) {
      return this.readCSVHeader(file);
    } else if (fileName.endsWith('.xlsx') || fileName.endsWith('.xls')) {
      return this.readExcelHeader(file);
    } else {
      throw new Error('Formato de arquivo não suportado para leitura de cabeçalho');
    }
  }

  private static async readCSVHeader(file: File): Promise<FileHeader> {
    return new Promise((resolve, reject) => {
      Papa.parse(file, {
        preview: 5, // Ler apenas as primeiras 5 linhas
        header: true,
        skipEmptyLines: true,
        complete: (results) => {
          try {
            const fields = results.meta.fields || [];
            const sampleData = results.data.slice(0, 3); // Primeiras 3 linhas de dados
            
            resolve({
              fields,
              rowCount: results.data.length,
              sampleData
            });
          } catch (error) {
            reject(new Error('Erro ao processar cabeçalho do CSV'));
          }
        },
        error: (error) => {
          reject(new Error(`Erro ao ler arquivo CSV: ${error.message}`));
        }
      });
    });
  }

  private static async readExcelHeader(file: File): Promise<FileHeader> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      
      reader.onload = (e) => {
        try {
          const data = new Uint8Array(e.target?.result as ArrayBuffer);
          const workbook = XLSX.read(data, { type: 'array' });
          
          // Pegar a primeira planilha
          const sheetName = workbook.SheetNames[0];
          const worksheet = workbook.Sheets[sheetName];
          
          // Converter para JSON para ler os dados
          const jsonData = XLSX.utils.sheet_to_json(worksheet, { 
            header: 1,
            range: 5 // Ler apenas as primeiras 5 linhas
          }) as any[][];
          
          if (jsonData.length === 0) {
            reject(new Error('Arquivo Excel vazio'));
            return;
          }
          
          // Primeira linha como cabeçalho
          const fields = jsonData[0]?.map(field => 
            String(field || '').trim()
          ).filter(field => field !== '') || [];
          
          // Dados de exemplo (linhas 2-4)
          const sampleData = jsonData.slice(1, 4).map(row => {
            const obj: any = {};
            fields.forEach((field, index) => {
              obj[field] = row[index] || '';
            });
            return obj;
          });
          
          resolve({
            fields,
            rowCount: jsonData.length - 1, // Excluindo cabeçalho
            sampleData
          });
        } catch (error) {
          reject(new Error('Erro ao processar arquivo Excel'));
        }
      };
      
      reader.onerror = () => {
        reject(new Error('Erro ao ler arquivo'));
      };
      
      reader.readAsArrayBuffer(file);
    });
  }
}