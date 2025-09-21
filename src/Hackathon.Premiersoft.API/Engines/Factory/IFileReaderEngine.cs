namespace Hackathon.Premiersoft.API.Engines.Factory
{
    public interface IFileReaderEngine
    {
        string FileReaderProvider { get; }

        Task Run(Guid importId);
    }
}
