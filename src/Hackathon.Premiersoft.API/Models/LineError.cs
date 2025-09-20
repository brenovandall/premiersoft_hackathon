using Hackathon.Premiersoft.API.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon.Premiersoft.API.Models
{
    public class LineError : Entity<long>
    {
        [Required]
        public long ImportId { get; private set; }

        [ForeignKey(nameof(ImportId))]
        public Import Import { get; private set; } = default!;

        [Required]
        public int Line { get; private set; }

        [Required]
        public string Field { get; private set; } = default!;

        [Required]
        public string Error { get; private set; } = default!;

        [Required]
        public string Value { get; private set; } = default!;

        protected LineError() { }

        public LineError(long importId, Import import, int line, string field, string error, string value)
        {
            ImportId = importId;
            Import = import;
            Line = line;
            Field = field;
            Error = error;
            Value = value;
        }
    }
}
