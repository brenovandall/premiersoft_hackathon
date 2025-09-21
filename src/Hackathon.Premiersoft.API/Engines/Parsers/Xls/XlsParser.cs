using DocumentFormat.OpenXml.Spreadsheet;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Hackathon.Premiersoft.API.Engines.Parsers.Xls
{
    public class XlsParser : IXlsParser
    {
        private IEntityFactory Factory { get; set; }

        public XlsParser(IEntityFactory factory)
        {
            Factory = factory;
        }

        public async Task ParseXlsAsync(Import import)
        {
            var s3Service = new S3Service();

            using var reader = await s3Service.ObterLeitorDoArquivoAsync(import.S3PreSignedUrl);
            using var stream = reader.BaseStream;

            var workbook = new HSSFWorkbook(stream);
            
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var sheet = workbook.GetSheetAt(i);

                foreach (IRow row in sheet)
                {
                    foreach (ICell cell in row.Cells)
                    {
                        if (cell != null && cell.CellType != NPOI.SS.UserModel.CellType.Blank)
                        {
                            string columnName = GetColumnName(cell.ColumnIndex + 1); 
                            int rowNumber = row.RowNum + 1; 

                            Factory.CreateEntity(new FileXlsDto
                            {
                                Import = import,
                                NumeroLinha = rowNumber,
                                Campo = columnName,
                                Valor = cell.ToString()
                            });
                        }
                    }
                }
            }
        }

        private static string GetColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
    }
}
