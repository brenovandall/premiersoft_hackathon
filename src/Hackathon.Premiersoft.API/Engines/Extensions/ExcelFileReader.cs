using Hackathon.Premiersoft.API.Engines.Factory;

namespace Hackathon.Premiersoft.API.Engines.Extensions
{
    public class ExcelFileReader : IFileReaderEngine
    {
        public string FileReaderProvider => Extensions.FileReaderProvider.ExcelReaderProvider;

        public void Run(Guid importId)
        {
            throw new NotImplementedException();
        }
    }
}
