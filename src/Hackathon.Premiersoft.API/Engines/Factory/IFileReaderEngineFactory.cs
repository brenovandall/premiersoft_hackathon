namespace Hackathon.Premiersoft.API.Engines.Factory
{
    public interface IFileReaderEngineFactory
    {
        IFileReaderEngine? CreateFactory(string providerName);
    }
}
