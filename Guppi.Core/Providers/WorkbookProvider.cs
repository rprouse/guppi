using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

namespace Guppi.Core.Providers;

public class WorkbookProvider
{
    readonly string _filename;
    readonly IXLWorkbook _workbook;
    readonly IXLWorksheet _worksheet;
    int row = 0;

    /// <summary>
    /// Creates a new instance of the WorksheetProvider
    /// </summary>
    /// <param name="filename">The filename for the workbook</param>
    /// <param name="sheet">The name of the worksheet</param>
    /// <param name="delete">Deletes and recreates the workbook if it exists</param>
    public WorkbookProvider(string filename, string sheet, IEnumerable<string> headers, bool delete = true)
    {
        _filename = filename;

        if (delete && File.Exists(_filename))
        {
            File.Delete(_filename);
        }

        _workbook = new XLWorkbook();
        _worksheet = _workbook.Worksheets.Add("Bills");

        AddRow(headers);
    }

    public void AddRow(IEnumerable<string> values)
    {
        row++;
        int col = 1;
        foreach (string value in values) {
            _worksheet.Cell(row, col++).Value = value;
        }
    }

    public void Save()
    {
        _workbook.SaveAs(_filename);
    }
}
