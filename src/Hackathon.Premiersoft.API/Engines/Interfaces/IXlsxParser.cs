using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IXlsxParser
    {
        Task ParseXlsxAsync(Import import);
    }
}