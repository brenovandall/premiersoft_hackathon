using Hackathon.Premiersoft.API.Models.Abstractions;
using Hackathon.Premiersoft.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Hackathon.Premiersoft.API.Models
{
    public class Import : Entity<long>
    {
        [Required]
        public ImportDataTypes DataType { get; private set; }

        [Required]
        public ImportFileFormat FileFormat { get; private set; }

        [MaxLength(255)]
        public string? Description { get; private set; } = default!;

        [Required]
        [MaxLength(100)]
        public string FileName { get; private set; } = default!;

        [Required]
        public string S3PreSignedUrl { get; private set; } = default!;

        [Required]
        public ImportStatus Status { get; private set; }

        [Required]
        public int? TotalRegisters { get; private set; }

        [Required]
        public int? TotalImportedRegisters { get; private set; }

        [Required]
        public int? TotalDuplicatedRegisters { get; private set; }

        [Required]
        public int? TotalFailedRegisters { get; private set; }

        [Required]
        public DateTime ImportedOn { get; private set; }

        [Required]
        public DateTime? FinishedOn { get; private set; }

        public ICollection<LineError> LineErrors { get; private set; } = [];

        protected Import() { }

        public Import(
            ImportDataTypes dataType,
            ImportFileFormat fileFormat,
            string fileName,
            ImportStatus status,
            int? totalRegisters,
            int? totalImportedRegisters,
            int? totalDuplicatedRegisters,
            int? totalFailedRegisters,
            DateTime importedOn,
            DateTime? finishedOn = null,
            string? description = null)
        {
            DataType = dataType;
            FileFormat = fileFormat;
            FileName = fileName;
            Status = status;
            TotalRegisters = totalRegisters;
            TotalImportedRegisters = totalImportedRegisters;
            TotalDuplicatedRegisters = totalDuplicatedRegisters;
            TotalFailedRegisters = totalFailedRegisters;
            ImportedOn = importedOn;
            FinishedOn = finishedOn;
            Description = description;
        }
    }
}
