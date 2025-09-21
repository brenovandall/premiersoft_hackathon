using Hackathon.Premiersoft.API.Models.Enums;

namespace Hackathon.Premiersoft.API.Dto
{
    public class ImportFilesRequest
    {
        public ImportDataTypes DataType { get; set; }
        public ImportFileFormat FileFormat { get; set; }
        public string FileName { get; set; } = default!;
        public string S3PreSignedUrl { get; set; } = default!;
        public ImportStatus Status { get; set; }
        public KeyPair[] FieldMappings { get; set; } = [];
    }

    public class KeyPair
    {
        public string From { get; set; } = default!;
        public string To { get; set; } = default!;
    }
}
