using Hackathon.Premiersoft.API.Dto;
using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Services;

namespace Hackathon.Premiersoft.API.Engines.Parsers.Hl7
{
    public class Hl7Parser
    {
        private List<FileHl7Dto> FileHl7Dtos { get; set; } = new List<FileHl7Dto>();

        public async Task<List<FileHl7Dto>> ParseHl7Async(Import import)
        {
            var s3Service = new S3Service();

            using var readerS3 = await s3Service.ObterLeitorDoArquivoAsync(import.S3PreSignedUrl);

            int lineNumber = 0;

            while (!readerS3.EndOfStream)
            {
                lineNumber++;
                var line = await readerS3.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var fields = line.Split('|');
                var segment = fields[0]; 

                for (int i = 1; i < fields.Length; i++)
                {
                    var value = fields[i];
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        FileHl7Dtos.Add(new FileHl7Dto
                        {
                            Import = import,
                            NumeroLinha = lineNumber,
                            Campo = $"{segment}.{i}",
                            Valor = value
                        });

                        Console.WriteLine($"Line {lineNumber} - {segment}.{i}: {value}");
                    }
                }
            }

            return FileHl7Dtos;
        }
    }
}
