using Hackathon.Premiersoft.API.Engines.Factory;

namespace Hackathon.Premiersoft.API.Engines
{
    public class ExcelFileReaderEngine : IFileReaderEngine
    {
        public string FileReaderProvider => Extensions.FileReaderProvider.ExcelReaderProvider;

        public void Run(string preSignedUrl)
        {
            throw new NotImplementedException();
        }
    }
}
