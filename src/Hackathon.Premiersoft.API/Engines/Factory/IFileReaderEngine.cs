namespace Hackathon.Premiersoft.API.Engines.Factory
{
    public interface IFileReaderEngine
    {
        string FileReaderProvider { get; }

        void Run(long importId);
    }
}
