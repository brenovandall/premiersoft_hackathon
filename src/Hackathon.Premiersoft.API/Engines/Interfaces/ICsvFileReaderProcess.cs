namespace Hackathon.Premiersoft.API.Engines.Interfaces
{
    public interface ICsvFileReaderProcess
    {

        Task ProcessarArquivoEmBackground(string key);
    }
}
