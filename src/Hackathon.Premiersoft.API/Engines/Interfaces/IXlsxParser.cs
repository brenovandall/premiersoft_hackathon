using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IXlsxParser
    {
        public Task ParseXlsxAsync(Import import);
    }
}
