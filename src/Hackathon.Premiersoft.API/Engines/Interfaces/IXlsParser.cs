using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IXlsParser
    {
        public Task ParseXlsAsync(Import import);
    }
}
