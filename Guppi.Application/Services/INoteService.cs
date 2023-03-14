namespace Guppi.Application.Services;

public interface INoteService
{
    void Configure();
    void AddFile(string vault, string title);
    void OpenVsCode();
    void OpenObsidian(string vault, string filename);
}
