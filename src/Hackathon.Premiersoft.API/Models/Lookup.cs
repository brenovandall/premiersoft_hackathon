using Hackathon.Premiersoft.API.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon.Premiersoft.API.Models
{
    public class Lookup : Entity<long>
    {
        public long ImportId { get; set; }

        [ForeignKey(nameof(ImportId))]
        public Import Import { get; set; } = default!;

        [Required]
        public string FileFieldName { get; set; } = default!;

        [Required]
        public string TableFieldName { get; set; } = default!;

        protected Lookup() { }

        public Lookup(long importId, Import import, string fileFieldName, string tableFieldName)
        {
            ImportId = importId;
            Import = import;
            FileFieldName = fileFieldName;
            TableFieldName = tableFieldName;
        }
    }
}
