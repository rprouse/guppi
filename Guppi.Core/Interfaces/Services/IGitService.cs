namespace Guppi.Core.Interfaces.Services;

public interface IGitService
{
    void Ammend();
    void Undo(bool hard);
    void Unstage();
    void Update(string branch);
}
