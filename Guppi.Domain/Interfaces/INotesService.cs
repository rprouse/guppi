namespace Guppi.Domain.Interfaces
{
    public interface INotesService
    {
        void OpenNotes(bool nocreate, string filename);
        void Configure();
    }
}
