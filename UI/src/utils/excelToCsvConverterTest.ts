import { ExcelToCsvConverter } from '@/utils/excelToCsvConverter';

/**
 * Teste do conversor Excel para CSV
 * Este arquivo pode ser usado para validar a conversão
 */
export async function testExcelToCsvConverter(file: File): Promise<void> {
  console.log('🧪 Iniciando teste do conversor Excel→CSV');
  
  if (!ExcelToCsvConverter.isExcelFile(file)) {
    console.error('❌ Arquivo não é um Excel válido');
    return;
  }

  try {
    // 1. Obter informações sobre o arquivo Excel
    console.log('📊 Obtendo informações do Excel...');
    const excelInfo = await ExcelToCsvConverter.getExcelInfo(file);
    
    console.log('📋 Informações do Excel:', {
      sheetNames: excelInfo.sheetNames,
      sheets: excelInfo.sheets
    });

    // 2. Converter primeira planilha
    console.log('🔄 Convertendo primeira planilha...');
    const conversionResult = await ExcelToCsvConverter.convertExcelToCsv(file);
    
    if (conversionResult.success) {
      console.log('✅ Conversão bem-sucedida:', {
        rowCount: conversionResult.rowCount,
        columnCount: conversionResult.columnCount,
        csvFileName: conversionResult.csvFile?.name,
        csvSize: conversionResult.csvFile?.size,
      });
      
      // Mostrar preview do CSV (primeiras 5 linhas)
      if (conversionResult.csvContent) {
        const lines = conversionResult.csvContent.split('\n').slice(0, 5);
        console.log('👀 Preview do CSV (primeiras 5 linhas):');
        lines.forEach((line, index) => {
          console.log(`  ${index + 1}: ${line}`);
        });
      }
      
    } else {
      console.error('❌ Erro na conversão:', conversionResult.error);
    }

    // 3. Se há múltiplas planilhas, converter todas
    if (excelInfo.sheetNames.length > 1) {
      console.log('📑 Convertendo todas as planilhas...');
      const allResults = await ExcelToCsvConverter.convertAllSheetsToCSV(file);
      
      allResults.forEach((result, index) => {
        const sheetName = excelInfo.sheetNames[index];
        if (result.success) {
          console.log(`✅ Planilha "${sheetName}": ${result.rowCount} linhas, ${result.columnCount} colunas`);
        } else {
          console.error(`❌ Erro na planilha "${sheetName}": ${result.error}`);
        }
      });
    }

  } catch (error) {
    console.error('❌ Erro no teste:', error);
  }
  
  console.log('🧪 Teste concluído');
}

/**
 * Função utilitária para download do CSV convertido (para testes)
 */
export function downloadCsvForTesting(csvFile: File): void {
  const url = URL.createObjectURL(csvFile);
  const a = document.createElement('a');
  a.href = url;
  a.download = csvFile.name;
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  URL.revokeObjectURL(url);
  console.log(`📥 CSV baixado: ${csvFile.name}`);
}