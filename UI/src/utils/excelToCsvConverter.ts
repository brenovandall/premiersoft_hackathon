import * as XLSX from 'xlsx';

export interface ExcelToCsvOptions {
  sheetName?: string; // Nome da planilha específica (se não fornecido, usa a primeira)
  skipEmptyRows?: boolean; // Pular linhas vazias
  dateFormat?: string; // Formato para datas
  header?: boolean; // Se deve incluir cabeçalho
}

export interface ConversionResult {
  success: boolean;
  csvContent?: string;
  csvBlob?: Blob;
  csvFile?: File;
  error?: string;
  sheetNames?: string[];
  rowCount?: number;
  columnCount?: number;
}

export class ExcelToCsvConverter {
  
  /**
   * Converte um arquivo Excel (.xlsx/.xls) para CSV mantendo headers e estrutura
   */
  static async convertExcelToCsv(
    file: File, 
    options: ExcelToCsvOptions = {}
  ): Promise<ConversionResult> {
    try {
      const {
        sheetName,
        skipEmptyRows = true,
        header = true
      } = options;

      // Ler o arquivo Excel
      const arrayBuffer = await file.arrayBuffer();
      const workbook = XLSX.read(arrayBuffer, { 
        type: 'array',
        cellDates: true, // Preservar datas
        cellStyles: false // Não precisamos de estilos
      });

      if (!workbook.SheetNames || workbook.SheetNames.length === 0) {
        return {
          success: false,
          error: 'Arquivo Excel não contém planilhas válidas'
        };
      }

      // Selecionar a planilha
      const targetSheetName = sheetName || workbook.SheetNames[0];
      const worksheet = workbook.Sheets[targetSheetName];

      if (!worksheet) {
        return {
          success: false,
          error: `Planilha '${targetSheetName}' não encontrada. Planilhas disponíveis: ${workbook.SheetNames.join(', ')}`
        };
      }

      // Converter para CSV com configurações específicas
      let csvContent = XLSX.utils.sheet_to_csv(worksheet, {
        FS: ',', // Field Separator (vírgula)
        RS: '\n', // Record Separator (nova linha)
        skipHidden: true, // Pular colunas/linhas ocultas
        blankrows: !skipEmptyRows, // Controle de linhas vazias
        dateNF: 'yyyy-mm-dd' // Formato de data padrão
      });

      // Verificar se o CSV está vazio
      if (!csvContent || csvContent.trim() === '') {
        return {
          success: false,
          error: `Planilha '${targetSheetName}' está vazia ou não contém dados válidos`
        };
      }

      // Limpar e validar o conteúdo CSV
      csvContent = this.cleanCsvContent(csvContent);

      // Contar linhas e colunas
      const lines = csvContent.split('\n').filter(line => line.trim() !== '');
      const rowCount = header ? lines.length - 1 : lines.length; // Subtrair header se presente
      const columnCount = lines.length > 0 ? lines[0].split(',').length : 0;

      // Criar Blob e File
      const csvBlob = new Blob([csvContent], { type: 'text/csv;charset=utf-8' });
      
      // Gerar nome do arquivo CSV baseado no original
      const originalName = file.name.replace(/\.(xlsx?|xls)$/i, '');
      const csvFileName = `${originalName}_converted.csv`;
      const csvFile = new File([csvBlob], csvFileName, { type: 'text/csv' });

      console.log(`✅ Conversão Excel→CSV concluída:`, {
        originalFile: file.name,
        sheetName: targetSheetName,
        rowCount,
        columnCount,
        csvFileName,
        size: `${(csvFile.size / 1024).toFixed(1)}KB`
      });

      return {
        success: true,
        csvContent,
        csvBlob,
        csvFile,
        sheetNames: workbook.SheetNames,
        rowCount,
        columnCount
      };

    } catch (error) {
      console.error('❌ Erro na conversão Excel→CSV:', error);
      
      return {
        success: false,
        error: error instanceof Error ? error.message : 'Erro desconhecido na conversão'
      };
    }
  }

  /**
   * Converte todas as planilhas de um arquivo Excel para múltiplos CSVs
   */
  static async convertAllSheetsToCSV(file: File): Promise<ConversionResult[]> {
    try {
      const arrayBuffer = await file.arrayBuffer();
      const workbook = XLSX.read(arrayBuffer, { type: 'array' });
      
      const results: ConversionResult[] = [];
      
      for (const sheetName of workbook.SheetNames) {
        const result = await this.convertExcelToCsv(file, { sheetName });
        results.push(result);
      }
      
      return results;
    } catch (error) {
      return [{
        success: false,
        error: error instanceof Error ? error.message : 'Erro ao processar múltiplas planilhas'
      }];
    }
  }

  /**
   * Obtém informações sobre as planilhas de um arquivo Excel
   */
  static async getExcelInfo(file: File): Promise<{
    sheetNames: string[];
    sheets: Array<{
      name: string;
      rowCount: number;
      columnCount: number;
      hasData: boolean;
    }>;
  }> {
    const arrayBuffer = await file.arrayBuffer();
    const workbook = XLSX.read(arrayBuffer, { type: 'array' });
    
    const sheets = workbook.SheetNames.map(sheetName => {
      const worksheet = workbook.Sheets[sheetName];
      const jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 }) as any[][];
      
      return {
        name: sheetName,
        rowCount: jsonData.length,
        columnCount: jsonData.length > 0 ? Math.max(...jsonData.map(row => row.length)) : 0,
        hasData: jsonData.length > 0 && jsonData.some(row => row.some(cell => cell !== undefined && cell !== ''))
      };
    });
    
    return {
      sheetNames: workbook.SheetNames,
      sheets
    };
  }

  /**
   * Limpa e normaliza o conteúdo CSV
   */
  private static cleanCsvContent(csvContent: string): string {
    return csvContent
      // Remover caracteres de controle problemáticos
      .replace(/[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]/g, '')
      // Normalizar quebras de linha
      .replace(/\r\n/g, '\n')
      .replace(/\r/g, '\n')
      // Remover linhas completamente vazias do final
      .replace(/\n+$/, '\n')
      // Garantir que termina com uma quebra de linha
      + (csvContent.endsWith('\n') ? '' : '\n');
  }

  /**
   * Valida se o arquivo é um Excel válido
   */
  static isExcelFile(file: File): boolean {
    const fileName = file.name.toLowerCase();
    const validExtensions = ['.xlsx', '.xls'];
    const validMimeTypes = [
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      'application/vnd.ms-excel'
    ];
    
    return validExtensions.some(ext => fileName.endsWith(ext)) || 
           validMimeTypes.includes(file.type);
  }
}