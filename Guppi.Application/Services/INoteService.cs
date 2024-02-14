namespace Guppi.Application.Services;

public interface INoteService
{
    void Configure();
    void AddFile(string title, string vault);
    void OpenVsCode();
    void OpenObsidian(string vault, string filename);
}
