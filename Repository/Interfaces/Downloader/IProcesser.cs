namespace Repositories.Interfaces.Downloader
{
    public  interface IProcesser
    {
        void ProcessGz(string filepath, string saveas);
    }
}