using Hackathon.Premiersoft.API.Models;

namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface IXmlParser
    {
        public Task ParseXmlAsync(Import import);
    }
}
