namespace Hackathon.Premiersoft.API.Engines.Factory
{
    public class FileReaderEngineFactory : IFileReaderEngineFactory
    {
        private readonly IEnumerable<IFileReaderEngine> _fileReaders;

        public FileReaderEngineFactory(IEnumerable<IFileReaderEngine> fileReaders)
        {
            _fileReaders = fileReaders;
        }

        public IFileReaderEngine? CreateFactory(string providerName)
        {
            return _fileReaders.FirstOrDefault(x => x.FileReaderProvider == providerName);
        }
    }
}
