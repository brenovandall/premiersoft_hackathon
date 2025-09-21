using ClosedXML.Excel;
using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Engines.Interfaces;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hackathon.Premiersoft.API.Engines.Parsers.Xlxs
{
    public class XlsxParser : IXlsxParser
    {
        private IEntityFactory EntityFactory { get; set; }

        public XlsxParser(IEntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
        }

        /// <summary>
        /// Lê um arquivo XLSX do S3 e extrai as células para FileXlsxDto.
        /// </summary>
        /// <param name="import">Objeto de importação com a URL pré-assinada do S3</param>
        /// <returns>Lista de FileXlsxDto</returns>
        public async Task ParseXlsxAsync(Import import)
        {
            var s3Service = new S3Service();

            using var reader = await s3Service.ObterLeitorDoArquivoAsync(import.S3PreSignedUrl);
            using var stream = reader.BaseStream;
            using var workbook = new XLWorkbook(stream);

            foreach (var worksheet in workbook.Worksheets)
            {
                var rows = worksheet.RowsUsed();
                foreach (var row in rows)
                {
                    foreach (var cell in row.CellsUsed())
                    {
                        string columnName = cell.Address.ColumnLetter;
                        int rowNumber = cell.Address.RowNumber;

                        if (!string.IsNullOrWhiteSpace(cell.GetString()))
                        {
                            EntityFactory.CreateEntity(new FileXlsxDto
                            {
                                Import = import,
                                NumeroLinha = rowNumber,
                                Campo = columnName,
                                Valor = cell.GetString()
                            });
                        }
                    }
                }
            }
        }

    }
}
